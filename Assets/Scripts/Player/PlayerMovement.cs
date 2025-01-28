using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// todo:修复一下点击移动的逻辑 现在比较容易点错
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Animator playerAnimator;
    
    private Queue<Vector3> pathQueue = new Queue<Vector3>(); // 路径队列
    private bool isMoving = false;
    [SerializeField] private Vector3 positionOffset = new Vector3(0, 1.9f, 0); // 偏移量

    private float horizontalInput;
    private float verticalInput;

    private float currentRotation = 0f;
    private float targetRotation = 180f;
    
    private StepManager _stepManager;

    void Start()
    {
        transform.rotation = Quaternion.Euler(0f, currentRotation, 0f);
        
        _stepManager = FindObjectOfType<StepManager>();
        if (_stepManager == null)
        {
            Debug.LogError("StepManager not found.");
        }
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
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                PathfindingManager pathfinding = FindObjectOfType<PathfindingManager>();
                if (pathfinding != null)
                {
                    Transform targetNode = pathfinding.GetClosestNode(hit.point);
                    Transform currentNode = pathfinding.GetClosestNode(transform.position - positionOffset);

                    if(!currentNode.GetComponent<NodeMarker>().IsWalkable)
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

                            // 如果步数足够，清空现有路径并添加新路径
                            pathQueue.Clear();
                            foreach (var node in path)
                            {
                                pathQueue.Enqueue(node.position + positionOffset);
                                EVENTMGR.TriggerUseStep(1);
                            }
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
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        isMoving = false;
    }
    

    private void HandleRotation(float horizontal)
    {
        if (horizontal > 0)
        {
            targetRotation = 180f;
        }
        else if (horizontal < 0)
        {
            targetRotation = 0f;
        }

        // 平滑过渡旋转
        if (Mathf.Abs(targetRotation - currentRotation) > 0.1f) // 防止频繁触发过渡
        {
            transform.DORotate(new Vector3(0f, targetRotation, 0f), 0.3f, RotateMode.FastBeyond360); // 使用 DOTween 来平滑旋转
            currentRotation = targetRotation; // 更新当前旋转角度
        }
    }


    private void HandleAnimations(Vector3 moveDirection)
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetFloat("Speed", moveDirection.magnitude); // 设置动画参数控制行走
        }
    }
}
