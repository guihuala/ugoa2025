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
                MoveTo(patrolPoints[currentPointIndex].position);
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length; // 循环巡逻
            }
            yield return new WaitForSeconds(1f); // 每次巡逻间隔
        }
    }
    
    public void MoveTo(Vector3 targetPosition)
    {
        if (pathfindingManager == null) return;

        // 获取当前最近的路径点
        Transform currentNode = pathfindingManager.GetClosestNode(transform.position);
        Transform targetNode = pathfindingManager.GetClosestNode(targetPosition);

        if (currentNode != null && targetNode != null)
        {
            List<Transform> path = AStarPathfinding.FindPath(currentNode, targetNode, pathfindingManager.mapNodes);

            if (path != null && path.Count > 0)
            {
                pathQueue.Clear();
                foreach (var node in path)
                {
                    Vector3 targetPos = new Vector3(node.position.x, node.position.y + positionOffset.y, node.position.z);
                    pathQueue.Enqueue(targetPos);
                }
                
                StartCoroutine(MoveAlongPath());
            }
        }
    }
    
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

    public void StartWobblingHead()
    {
        // 头部摇摆的动画
    }

    public void StopWobblingHead()
    {
        // 停止头部摇摆的动画
    }

    public override void PerformFoundPlayer()
    {
        // 触发失败事件
        EVENTMGR.TriggerPlayerDead();
    }
}
