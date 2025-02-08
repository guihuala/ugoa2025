using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;


public class Poacher : EnemyBase
{
    private int currentPointIndex = 0;

    [SerializeField] private Vector3 positionOffset = new Vector3(0, 1.5f, 0);  

    private bool isMoving = false;
    private bool stopMoving = false;
    private Queue<Vector3> pathQueue = new Queue<Vector3>(); // 路径队列
    private PathfindingManager pathfindingManager;

    [Header("小弟配置")]
    [SerializeField] private EnemyFollower followerPrefab;
    [SerializeField] private int numberOfFollowers = 3;  // 小弟的数量

    private List<EnemyFollower> followers = new List<EnemyFollower>();  // 小弟列表

    [Header("动画配置")]
    [SerializeField] private string WalkAnimation = "walk";
    [SerializeField] private string scaredAnimation = "scare";
    [SerializeField] private string standAnimation = "standby";
    [SerializeField] private string blinkAnimation = "blink";

    [Header("发现示意图标")] 
    [SerializeField] private GameObject foundIcon;
    private Vector3 iconOriginalScale;

    protected override void InitializeStates()
    {
        stateMachine.ChangeState(new PatrolState(this));
    }

    protected override void Start()
    {
        base.Start();
        
        pathfindingManager = FindObjectOfType<PathfindingManager>();
        if (pathfindingManager == null)
        {
            Debug.LogError("PathfindingManager not found.");
        }

        // 创建小弟
        CreateFollowers();
        PlayOverlayAnimation(1, blinkAnimation);
        
        if (foundIcon != null)
        {
            iconOriginalScale = foundIcon.transform.localScale;
            foundIcon.transform.localScale = Vector3.zero;
        }
    }

    protected override void ClearTrack()
    {
        base.ClearTrack();
        // 清除轨道时播放站立动画
        PlayAnimation(standAnimation);
    }

    private void CreateFollowers()
    {
        for (int i = 0; i < numberOfFollowers; i++)
        {
            EnemyFollower follower = Instantiate(followerPrefab, transform.position, Quaternion.identity);
            follower.SetTargetEnemy(this);
            followers.Add(follower);
        }
    }

    public override void MoveForward()
    {
        if (!stopMoving)
        {
            StartCoroutine(PatrolRoutine());
        }
    }

    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            if (!isMoving && !stopMoving)
            {
                ComputeFullPath();
            }
            yield return new WaitForSeconds(1f);
        }
    }

    public void ComputeFullPath()
    {
        if (pathfindingManager == null) return;
        
        Transform currentNode = pathfindingManager.GetClosestNode(transform.position);
        
        if (currentNode == null) return;

        // 清空之前的路径队列
        pathQueue.Clear();

        // 遍历所有巡逻点并计算路径
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            Transform targetNode = pathfindingManager.GetClosestNode(patrolPoints[i].position);
            if (targetNode != null)
            {
                List<Transform> path = AStarPathfinding.FindPath(currentNode, targetNode, pathfindingManager.mapNodes);
                if (path != null && path.Count > 0)
                {
                    foreach (var node in path)
                    {
                        Vector3 targetPos = new Vector3(node.position.x, node.position.y + positionOffset.y, node.position.z);
                        pathQueue.Enqueue(targetPos);
                    }
                    currentNode = targetNode; // 更新当前节点
                }
            }
        }

        // 给所有小弟传递路径
        for (int i = 0; i < followers.Count; i++)
        {
            float delay = (i + 1) * 1f; // 每个小弟有不同的跟随延迟
            followers[i].FollowPath(pathQueue, delay); // 将路径队列传递给每个小弟
        }

        StartCoroutine(MoveAlongPath());
        ClearTrack();
    }

    private IEnumerator MoveAlongPath()
    {
        isMoving = true;

        while (pathQueue.Count > 0 && !stopMoving)
        {
            if (Time.timeScale == 0)
            {
                yield return new WaitUntil(() => Time.timeScale > 0);
            }
            
            Vector3 targetPosition = pathQueue.Dequeue();

            // 处理角色朝向
            HandleRotation(targetPosition - transform.position);
            
            PlayAnimation(WalkAnimation);

            // 移动角色
            while ((transform.position - targetPosition).sqrMagnitude > 0.01f && !stopMoving)
            {
                if (Time.timeScale == 0)
                {
                    yield return new WaitUntil(() => Time.timeScale > 0);
                }

                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPosition; // 确保精准到达目标点

            yield return new WaitForSeconds(2f);
        }

        isMoving = false;
    }

    private void HandleRotation(Vector3 direction)
    {
        if (direction.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            CheckPoint.DORotateQuaternion(targetRotation, 0.3f);
            
            PlayOverlayAnimation(2, scaredAnimation);
        }
    }

    public override void PerformFoundPlayer()
    {
        stopMoving = true;

        if (foundIcon != null)
        {
            foundIcon.SetActive(true);
            
            Vector3 squeezeScale = new Vector3(0.7f, 0.7f, 1f);
            
            foundIcon.transform.DOScale(squeezeScale, 0.2f)
                .SetEase(Ease.OutElastic)
                .OnComplete(() =>
                {
                    DOTween.To(() => foundIcon.transform.localScale, 
                        scale => foundIcon.transform.localScale = scale, 
                        iconOriginalScale, 0.8f);
                });
        }
        
        StartCoroutine(TriggerEventAfterDelay(1f));
    }

    private IEnumerator TriggerEventAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        EVENTMGR.TriggerPlayerDead();
    }
}
