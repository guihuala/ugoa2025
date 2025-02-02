using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Poacher : EnemyBase
{
    private int currentPointIndex = 0;
    
    [SerializeField] private Vector3 positionOffset = new Vector3(0, 1.5f, 0);  
    
    private bool isMoving = false;
    private Queue<Vector3> pathQueue = new Queue<Vector3>(); // 路径队列
    private PathfindingManager pathfindingManager;
    
    [Header("小弟配置")]
    [SerializeField] private EnemyFollower followerPrefab;
    [SerializeField] private int numberOfFollowers = 3;  // 小弟的数量

    private List<EnemyFollower> followers = new List<EnemyFollower>();  // 小弟列表

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
    }

    private void CreateFollowers()
    {
        for (int i = 0; i < numberOfFollowers; i++)
        {
            EnemyFollower follower = Instantiate(followerPrefab, transform.position, Quaternion.identity);
            followers.Add(follower);
        }
    }

    public override void MoveForward()
    {
        StartCoroutine(PatrolRoutine());
    }
    
    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            if (!isMoving)
            {
                ComputeFullPath();
            }
            yield return new WaitForSeconds(1f);
        }
    }
    
    // 计算完整路径
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
    }

    // 沿着完整路径行走
    private IEnumerator MoveAlongPath()
    {
        isMoving = true;

        while (pathQueue.Count > 0)
        {
            if (Time.timeScale == 0)
            {
                yield return new WaitUntil(() => Time.timeScale > 0);
            }

            Vector3 targetPosition = pathQueue.Dequeue();

            // 处理角色朝向
            HandleRotation(targetPosition - transform.position);

            // 移动角色
            while ((transform.position - targetPosition).sqrMagnitude > 0.01f) // 避免浮点数误差
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
        }
    }
    
    public override void PerformFoundPlayer()
    {
        // 触发失败事件
        EVENTMGR.TriggerPlayerDead();
    }
}
