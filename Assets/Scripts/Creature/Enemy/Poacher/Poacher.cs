using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;


public class Poacher : EnemyBase
{
    private int currentPointIndex = 0;
    private bool isMoving = false;
    private bool stopMoving = false;
    private bool isPlayerFound = false; // 防止重复触发发现玩家事件

    [SerializeField] private Vector3 positionOffset = new Vector3(0, 1.5f, 0);
    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    private PathfindingManager pathfindingManager;

    [Header("小弟配置")]
    [SerializeField] private EnemyFollower followerPrefab;
    [SerializeField] private int numberOfFollowers = 3;
    private List<EnemyFollower> followers = new List<EnemyFollower>();

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
            foundIcon.transform.localScale = Vector3.zero;
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

        pathQueue.Clear();
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
                    currentNode = targetNode;
                }
            }
        }

        for (int i = 0; i < followers.Count; i++)
        {
            float delay = (i + 1) * 1f;
            followers[i].FollowPath(pathQueue, delay);
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
            HandleRotation(targetPosition - transform.position);
            
            PlayAnimation(WalkAnimation);

            while ((transform.position - targetPosition).sqrMagnitude > 0.01f && !stopMoving)
            {
                if (Time.timeScale == 0)
                {
                    yield return new WaitUntil(() => Time.timeScale > 0);
                }

                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPosition;
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
        // **防止重复触发**
        if (isPlayerFound) return;
        isPlayerFound = true;

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

        AudioManager.Instance.PlaySfx("BeFound_1");
        EVENTMGR.TriggerPlayerFound();

        StartCoroutine(TriggerEventAfterDelay(1f));
    }

    private IEnumerator TriggerEventAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EVENTMGR.TriggerPlayerDead();
    }
}
