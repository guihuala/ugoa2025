using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    [Header("一些配置")]
    public float highlightRadius = 5f;
    public float updateInterval = 1f;
    [SerializeField][Tooltip("初始偏移")] private Vector3 startPositionOffset = new Vector3(0, 1.5f, 0);
    [SerializeField][Tooltip("是否启用路径缓存")] private bool usePathCache = true;  // 是否启用路径缓存
    
    [Header("节点")]
    public List<Transform> mapNodes = new List<Transform>();
    
    private GameObject player;
    private bool isCharacterSelected = false;
    private HashSet<Transform> previousHighlightNodes = new HashSet<Transform>();
    private Coroutine highlightCoroutine = null;
    
    private Dictionary<(Transform, Transform), List<Transform>> pathCache = new Dictionary<(Transform, Transform), List<Transform>>();

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("玩家对象未找到");
            return;
        }

        FindWalkableNodes();

        Transform closestNode = GetClosestNode(player.transform.position);
        if (closestNode != null)
        {
            player.transform.position = closestNode.position + startPositionOffset;
        }

        EVENTMGR.ChangeSteps += UpdateHighlightRadius;
        EVENTMGR.OnClickPlayer += OnClickPlayer;
        EVENTMGR.OnPlayerDead += ClearAllHighlights;
        EVENTMGR.OnTerrainChange += FindWalkableNodes;
    }

    private void OnDestroy()
    {
        EVENTMGR.ChangeSteps -= UpdateHighlightRadius;
        EVENTMGR.OnClickPlayer -= OnClickPlayer;
        EVENTMGR.OnPlayerDead -= ClearAllHighlights;
        EVENTMGR.OnTerrainChange -= FindWalkableNodes;

        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
            highlightCoroutine = null;
        }
    }

    #region 搜索节点有关的方法

private void FindWalkableNodes()
    {
        HashSet<Transform> currentWalkableNodes = new HashSet<Transform>();
        
        // 查找场景中所有节点标记
        foreach (var node in FindObjectsOfType<NodeMarker>())
        {
            if (node.IsWalkable)
            {
                currentWalkableNodes.Add(node.transform);
            }
        }

        // 移除已不存在于场景中的节点
        mapNodes.RemoveAll(node => node == null);

        // 创建需要移除的节点列表
        List<Transform> nodesToRemove = new List<Transform>();
        
        // 检查现有节点
        foreach (var existingNode in mapNodes)
        {
            // 如果节点不在当前可行走集合中，标记为需要移除
            if (!currentWalkableNodes.Contains(existingNode))
            {
                nodesToRemove.Add(existingNode);
            }
            else
            {
                // 如果节点仍然可行走，从当前集合中移除以避免重复添加
                currentWalkableNodes.Remove(existingNode);
            }
        }

        // 移除不再可行走的节点
        foreach (var nodeToRemove in nodesToRemove)
        {
            mapNodes.Remove(nodeToRemove);
            
            // 同时从缓存中移除相关路径
            if (usePathCache)
            {
                RemoveNodeFromCache(nodeToRemove);
            }
        }

        // 添加新发现的可行走节点
        mapNodes.AddRange(currentWalkableNodes);
    }

    // 从缓存中移除与指定节点相关的所有路径
    private void RemoveNodeFromCache(Transform nodeToRemove)
    {
        List<(Transform, Transform)> keysToRemove = new List<(Transform, Transform)>();
        
        foreach (var key in pathCache.Keys)
        {
            if (key.Item1 == nodeToRemove || key.Item2 == nodeToRemove)
            {
                keysToRemove.Add(key);
            }
        }

        foreach (var key in keysToRemove)
        {
            pathCache.Remove(key);
        }
    }
    
    #endregion
    
    private void UpdateHighlightRadius(int newHighlightRadius)
    {
        if (highlightRadius != newHighlightRadius)
        {
            highlightRadius = newHighlightRadius;
            if (isCharacterSelected)
            {
                UpdateHighlightNodes();
            }
        }
    }

    private void OnClickPlayer(bool isClick)
    {
        if (isCharacterSelected == isClick) return;

        isCharacterSelected = isClick;

        if (isCharacterSelected)
        {
            UpdateHighlightNodes();
            if (highlightCoroutine == null)
            {
                highlightCoroutine = StartCoroutine(PeriodicUpdateHighlightNodes());
            }
        }
        else
        {
            if (highlightCoroutine != null)
            {
                StopCoroutine(highlightCoroutine);
                highlightCoroutine = null;
            }
            ClearAllHighlights();
        }
    }

    private IEnumerator PeriodicUpdateHighlightNodes()
    {
        while (isCharacterSelected)
        {
            yield return new WaitForSeconds(updateInterval);
            UpdateHighlightNodes();
        }
    }

    private void UpdateHighlightNodes()
    {
        if (player == null) return;

        Transform currentNode = GetClosestNode(player.transform.position - startPositionOffset);
        if (currentNode == null) return;

        HashSet<Transform> newHighlightNodes = new HashSet<Transform>();

        float checkRadius = highlightRadius * 1.2f;
        
        foreach (var node in mapNodes)
        {
            if (Vector3.Distance(currentNode.position, node.position) > checkRadius) continue;

            NodeMarker nodeMarker = node.GetComponent<NodeMarker>();
            if (nodeMarker == null) continue;
            
            List<Transform> path;
            if (usePathCache && pathCache.TryGetValue((currentNode, node), out path))
            {
                // 如果启用缓存且缓存中有路径，则使用缓存路径
                path = pathCache[(currentNode, node)];
            }
            else
            {
                // 如果缓存未启用，或没有缓存路径，则重新计算路径
                path = AStarPathfinding.FindPath(currentNode, node, mapNodes);

                if (usePathCache && path != null)
                {
                    pathCache[(currentNode, node)] = path;  // 启用缓存时存储路径
                }
            }

            if (path != null && path.Count <= highlightRadius)
            {
                newHighlightNodes.Add(node);
                if (!previousHighlightNodes.Contains(node))
                {
                    nodeMarker.ShowHighlight();
                }
            }
        }
        
        foreach (var node in previousHighlightNodes)
        {
            if (!newHighlightNodes.Contains(node))
            {
                NodeMarker nodeMarker = node.GetComponent<NodeMarker>();
                if (nodeMarker != null)
                {
                    nodeMarker.HideHighlight();
                }
            }
        }

        previousHighlightNodes = newHighlightNodes;
    }

    private void ClearAllHighlights()
    {
        foreach (var node in previousHighlightNodes)
        {
            NodeMarker nodeMarker = node.GetComponent<NodeMarker>();
            if (nodeMarker != null)
            {
                nodeMarker.HideHighlight();
            }
        }
        previousHighlightNodes.Clear();
    }

    public Transform GetClosestNode(Vector3 position)
    {
        Transform closestNode = null;
        float minDistance = Mathf.Infinity;

        foreach (var node in mapNodes)
        {
            float distance = Vector3.Distance(position, node.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestNode = node;
            }
        }
        return closestNode;
    }
}
