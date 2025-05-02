using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Player))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxAllowedDistance = 1.2f; // 最大允许移动距离
    
    [SerializeField] private Vector3 positionOffset = new Vector3(0, 1.9f, 0); // 偏移量
    public Vector3 PositionOffset => positionOffset;
    
    [SerializeField] private LayerMask pathLayerMask;

    private Player player;
    private Transform playerSpine;
    private StepManager _stepManager;
    
    private float currentRotation = 0f;
    private float targetRotation = 180f;
    
    private PathfindingManager pathfindingManager;
    private bool isMoving = false;
    private Queue<Vector3> pathQueue = new Queue<Vector3>(); // 路径队列
    private float originRotation = 0f;

    public bool IsPlayerRotate()
    {
        return currentRotation <= 0f;
    }

    #region 生命周期

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

        playerSpine = transform.GetChild(0).transform;
        
        player = GetComponent<Player>();

        EVENTMGR.OnEnterTargetField += HandlePlayerMoveWihoutChecking;
        EVENTMGR.OnClickMarker += HandlePlayerMove;
        
        originRotation = playerSpine.rotation.eulerAngles.y;
    }

    private void OnDestroy()
    {
        EVENTMGR.OnEnterTargetField -= HandlePlayerMoveWihoutChecking;
        EVENTMGR.OnClickMarker -= HandlePlayerMove;
    }

    void Update()
    {
        // 处理路径移动
        if (!isMoving && pathQueue.Count > 0 && !player.IsDead)
        {
            StartCoroutine(MoveAlongPath());
        }
    }    

    #endregion
    
    public void ChangeRotation(float newRotation)
    {
        playerSpine.rotation = Quaternion.Euler(0, newRotation, 0);
        originRotation = newRotation;
    }

    #region 移动操作

    public void HandlePlayerMove(Vector3 pos)
    {
        if (_stepManager.GetRemainingSteps() <= 0)
            return;
        
        if (pathfindingManager != null)
        {
            Transform targetNode = pathfindingManager.GetClosestNode(pos);
            Transform currentNode = pathfindingManager.GetClosestNode(transform.position - positionOffset);

            // 检查当前节点是否可行走
            NodeMarker currentNodeMarker = currentNode?.GetComponent<NodeMarker>();
            if (currentNodeMarker == null || !currentNodeMarker.IsWalkable || !currentNodeMarker.IsHighlighted)
                return;

            if (targetNode != null)
            {
                List<Transform> path = AStarPathfinding.FindPath(currentNode, targetNode, pathfindingManager.mapNodes);

                if (path != null)
                {
                    EVENTMGR.TriggerClickPath();

                    // 清空现有路径并添加新路径
                    pathQueue.Clear();
                    
                    foreach (var node in path)
                    {
                        Vector3 targetPos = new Vector3(node.position.x, node.position.y + positionOffset.y, node.position.z);
                        pathQueue.Enqueue(targetPos);
                        EVENTMGR.TriggerUseStep(1);
                    }

                    PlayWalkAnimation();
                }
            }
        }
    }
    
    public void HandlePlayerMoveWihoutChecking(Vector3 pos)
    {
        // 剧情用不需要检查步数
        
        if (pathfindingManager != null)
        {
            Transform targetNode = pathfindingManager.GetClosestNode(pos);
            Transform currentNode = pathfindingManager.GetClosestNode(transform.position - positionOffset);

            if (targetNode != null && currentNode != null)
            {
                List<Transform> path = AStarPathfinding.FindPath(currentNode, targetNode, pathfindingManager.mapNodes,true);

                if (path != null)
                {
                    if (path.Count - 1 > _stepManager.GetRemainingSteps())
                    {
                        Debug.Log("路径超出剩余步数，无法移动！");
                        return;
                    }

                    pathQueue.Clear();
                    foreach (var node in path)
                    {
                        Vector3 targetPos = new Vector3(node.position.x, node.position.y + positionOffset.y, node.position.z);
                        pathQueue.Enqueue(targetPos);
                        EVENTMGR.TriggerUseStep(1);
                        EVENTMGR.TriggerClickPath();
                    }

                    PlayWalkAnimation();
                }
            }
        }
    }    

    #endregion


    #region 移动的视听效果

    private void PlayWalkAnimation()
    {
        if(player.IsInvisible)
            return;
        if(player.IsInSwamp)
            return;
        
        player.PlayAnimation(player.walkAnimation);
    }

    private void PlayMoveSound()
    {
        if (player.IsInSwamp)
        {
            int index = Random.Range(1, 4);
            AudioManager.Instance.PlaySfx("swamp_walk_" + index);
        }
        else if (player.IsInvisible)
        {
            int index = Random.Range(1, 3);
            AudioManager.Instance.PlaySfx("into_grass_"+ index);
        }
        else
        {
            int index = Random.Range(1, 4);
            AudioManager.Instance.PlaySfx("step_"+ index);
        }
    }    

    #endregion
    
    private IEnumerator MoveAlongPath()
    {
        isMoving = true;

        while (pathQueue.Count > 0)
        {
            Vector3 targetPosition = pathQueue.Peek();

            targetPosition = pathQueue.Dequeue();

            Vector3 direction = targetPosition - transform.position;  // 计算旋转

            HandleRotation(direction);
            
            // 移动角色
            while ((transform.position - targetPosition).sqrMagnitude > 0.01f) // 避免浮点数误差
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                
                NodeMarker currentNodeMarker = pathfindingManager.GetClosestNode(transform.position - positionOffset)?.GetComponent<NodeMarker>();
                if (currentNodeMarker != null)
                {
                    currentNodeMarker.ShowFootPrint(direction.normalized);
                }
                
                yield return null;
            }

            EVENTMGR.TriggerPlayerStep(targetPosition - positionOffset);
            transform.position = targetPosition; // 确保精准到达目标点
            
            player.HandleDetect();
            
            PlayMoveSound();
        }
        
        isMoving = false;
    }

    public void Teleport(Vector3 targetPosition)
    {
        transform.position = targetPosition + positionOffset;
    }
    
    private void HandleRotation(Vector3 direction)
    {
        if (Mathf.Abs(direction.x) > 0.01f)
        {
            targetRotation = direction.x > 0 ? originRotation + 180f : originRotation;
            if (!Mathf.Approximately(targetRotation, currentRotation))
            {
                playerSpine.DORotate(new Vector3(0f, targetRotation, 0f), 0.3f, RotateMode.FastBeyond360);
                currentRotation = targetRotation;
            }
        }
    }
}
