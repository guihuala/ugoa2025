using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    public float highlightRadius = 5f;
    public float searchDistance = 6f;
    public float updateInterval = 1f;

    private GameObject player;
    private Vector3 startPositionOffset = new Vector3(0, 1.5f, 0);
    public List<Transform> mapNodes = new List<Transform>();

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
        EVENTMGR.OnPlayerStep += OnShowFootprintInNode;
    }

    private void OnDestroy()
    {
        EVENTMGR.ChangeSteps -= UpdateHighlightRadius;
        EVENTMGR.OnClickPlayer -= OnClickPlayer;
        EVENTMGR.OnPlayerStep -= OnShowFootprintInNode;

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
            if (Vector3.Distance(currentNode.position, node.position) > searchDistance) continue;

            NodeMarker nodeMarker = node.GetComponent<NodeMarker>();
            if (nodeMarker == null) continue;

            // **使用缓存路径，避免重复计算**
            List<Transform> path;
            if (!pathCache.TryGetValue((currentNode, node), out path))
            {
                path = AStarPathfinding.FindPath(currentNode, node, mapNodes);
                pathCache[(currentNode, node)] = path;
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

        // **优化：仅隐藏从高亮中移除的节点**
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

    private void OnShowFootprintInNode(Vector3 position)
    {
        NodeMarker nodeToShow = GetClosestNode(position).GetComponent<NodeMarker>();
        nodeToShow.ShowFootPrint();
    }
}
