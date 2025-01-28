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
    
    private bool isMoving = false;
    private Queue<Vector3> pathQueue = new Queue<Vector3>(); // 路径队列
    
    void Start()
    {
        _stepManager = FindObjectOfType<StepManager>();
        if (_stepManager == null)
        {
            Debug.LogError("StepManager not found.");
        }
        
        player = GetComponent<Player>();
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
            // 创建射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // 使用 LayerMask 限制检测层
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, pathLayerMask))
            {
                PathfindingManager pathfinding = FindObjectOfType<PathfindingManager>();
                if (pathfinding != null)
                {
                    Transform targetNode = pathfinding.GetClosestNode(hit.point);
                    Transform currentNode = pathfinding.GetClosestNode(transform.position - positionOffset);

                    // 检查当前节点是否可行走
                    if (!currentNode.GetComponent<NodeMarker>().IsWalkable)
                        return;
                
                    if (currentNode != null && targetNode != null)
                    {
                        List<Transform> path = AStarPathfinding.FindPath(currentNode, targetNode, pathfinding.mapNodes);

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
                            
                            // 如果步数足够，清空现有路径并添加新路径
                            pathQueue.Clear();
                            foreach (var node in path)
                            {
                                pathQueue.Enqueue(node.position + positionOffset);
                                EVENTMGR.TriggerUseStep(1);
                            }

                            player.PlayAnimation(player.walkAnimation, true);
                        }
                    }
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
            
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        isMoving = false;
    }
    
    private void HandleRotation(Vector3 direction)
    {

        if (direction.x > 0) // 向右移动
        {
            targetRotation = 180f;
        }
        else if (direction.x <= 0) // 向左移动
        {
            targetRotation = 0f;
        }

        // 平滑过渡旋转
        if (Mathf.Abs(targetRotation - currentRotation) > 0.1f)
        {
            transform.DORotate(new Vector3(0f, targetRotation, 0f), 0.3f, RotateMode.FastBeyond360); // 使用 DOTween 来平滑旋转
            currentRotation = targetRotation; // 更新当前旋转角度
        }
    }
}
