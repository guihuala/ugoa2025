using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Vector3 positionOffset = new Vector3(0, 1.9f, 0); // 偏移量
    [SerializeField] private LayerMask pathLayerMask;

    private Player player;
    private StepManager _stepManager;
    
    private float currentRotation = 0f;
    private float targetRotation = 180f;
    
    private PathfindingManager pathfindingManager;
    private bool isMoving = false;
    private Queue<Vector3> pathQueue = new Queue<Vector3>(); // 路径队列
    
    void Start()
    {
        _stepManager = FindObjectOfType<StepManager>();
        if (_stepManager == null)
        {
            Debug.LogError("StepManager not found.");
        }
        
        pathfindingManager = FindObjectOfType<PathfindingManager>();
        if (pathfindingManager == null)
        {
            Debug.LogError("PathfindingManager not found.");
        }
        
        player = GetComponent<Player>();

        EVENTMGR.OnEnterTargetField += HandlePlayerMoveWihoutChecking;
    }

    private void OnDestroy()
    {
        EVENTMGR.OnEnterTargetField -= HandlePlayerMoveWihoutChecking;
    }


    void Update()
    {
        HandleMouseInput();
        
        if (!isMoving && pathQueue.Count > 0)
        {
            StartCoroutine(MoveAlongPath());
        }
    }
    
    private void HandleMouseInput()
    {
        if (_stepManager.GetRemainingSteps() <= 0)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, pathLayerMask))
            {
                HandlePlayerMove(hit.point);
            }
        }
    }

    public void HandlePlayerMove(Vector3 pos)
    {
        if (pathfindingManager != null)
        {
            Transform targetNode = pathfindingManager.GetClosestNode(pos);
            Transform currentNode = pathfindingManager.GetClosestNode(transform.position - positionOffset);

            // 检查当前节点是否可行走
            if (!currentNode.GetComponent<NodeMarker>().IsWalkable || !currentNode.GetComponent<NodeMarker>().IsHighlighted)
                return;
                
            if (currentNode != null && targetNode != null)
            {
                List<Transform> path = AStarPathfinding.FindPath(currentNode, targetNode, pathfindingManager.mapNodes);

                if (path != null)
                {
                    // 检查路径长度是否超过剩余步数
                    if (path.Count - 1 > _stepManager.GetRemainingSteps())
                    {
                        Debug.Log("路径超出剩余步数，无法移动！");
                        return;
                    }

                    // 触发关闭clickUI事件
                    EVENTMGR.TriggerClickPath();
                            
                    // 清空现有路径并添加新路径
                    pathQueue.Clear();
                    foreach (var node in path)
                    {
                        Vector3 targetPos = new Vector3(node.position.x, node.position.y + positionOffset.y, node.position.z);
                        pathQueue.Enqueue(targetPos);
                        EVENTMGR.TriggerUseStep(1);
                    }

                    player.PlayAnimation(player.walkAnimation, true);
                }
            }
        }
    }
    
    public void HandlePlayerMoveWihoutChecking(Vector3 pos)
    {
        if (pathfindingManager != null)
        {
            Transform targetNode = pathfindingManager.GetClosestNode(pos);
            Transform currentNode = pathfindingManager.GetClosestNode(transform.position - positionOffset);
                
            if (currentNode != null && targetNode != null)
            {
                List<Transform> path = AStarPathfinding.FindPath(currentNode, targetNode, pathfindingManager.mapNodes);

                if (path != null)
                {
                    // 检查路径长度是否超过剩余步数
                    if (path.Count - 1 > _stepManager.GetRemainingSteps())
                    {
                        Debug.Log("路径超出剩余步数，无法移动！");
                        return;
                    }

                    // 触发关闭clickUI事件
                    EVENTMGR.TriggerClickPath();
                            
                    // 清空现有路径并添加新路径
                    pathQueue.Clear();
                    foreach (var node in path)
                    {
                        Vector3 targetPos = new Vector3(node.position.x, node.position.y + positionOffset.y, node.position.z);
                        pathQueue.Enqueue(targetPos);
                        EVENTMGR.TriggerUseStep(1);
                    }

                    player.PlayAnimation(player.walkAnimation, true);
                }
            }
        }
    }

    private IEnumerator MoveAlongPath()
    {
        isMoving = true;

        while (pathQueue.Count > 0)
        {
            Vector3 targetPosition = pathQueue.Dequeue();

            // 处理角色朝向
            HandleRotation(targetPosition - transform.position);

            // 移动角色
            while ((transform.position - targetPosition).sqrMagnitude > 0.01f) // 避免浮点数误差
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPosition; // 确保精准到达目标点
        }

        isMoving = false;
    }
    
    private void HandleRotation(Vector3 direction)
    {
        if (Mathf.Abs(direction.x) > 0.01f)
        {
            targetRotation = direction.x > 0 ? 180f : 0f;
            if (!Mathf.Approximately(targetRotation, currentRotation)) // 避免重复旋转
            {
                transform.DORotate(new Vector3(0f, targetRotation, 0f), 0.3f, RotateMode.FastBeyond360);
                currentRotation = targetRotation;
            }
        }
    }
}
