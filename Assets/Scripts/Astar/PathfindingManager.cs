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

        // 仅存储可行走节点
        foreach (var node in FindObjectsOfType<NodeMarker>())
        {
            if (node.IsWalkable)
            {
                mapNodes.Add(node.transform);
            }
        }

        Transform closestNode = GetClosestNode(player.transform.position);
        if (closestNode != null)
        {
            player.transform.position = closestNode.position + startPositionOffset;
        }

        EVENTMGR.ChangeSteps += UpdateHighlightRadius;
        EVENTMGR.OnClickPlayer += OnClickPlayer;
        EVENTMGR.OnPlayerDead += ClearAllHighlights;
    }

    private void OnDestroy()
    {
        EVENTMGR.ChangeSteps -= UpdateHighlightRadius;
        EVENTMGR.OnClickPlayer -= OnClickPlayer;
        EVENTMGR.OnPlayerDead -= ClearAllHighlights;

        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
            highlightCoroutine = null;
        }
    }

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

        foreach (var node in mapNodes)
        {
            if (Vector3.Distance(currentNode.position, node.position) > highlightRadius * 1.2f) continue;

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
