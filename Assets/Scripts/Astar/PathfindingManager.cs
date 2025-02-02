using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    public float highlightRadius = 5f; // 高亮步数范围
    private GameObject player;
    private Vector3 startPositionOffset = new Vector3(0, 1.5f, 0);
    public List<Transform> mapNodes = new List<Transform>();

    private bool isCharacterSelected = false; // 记录玩家是否被选中
    private HashSet<Transform> previousHighlightNodes = new HashSet<Transform>(); // 记录上一帧高亮的节点

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("玩家对象未找到");
            return;
        }

        // 获取所有可行走的节点
        NodeMarker[] nodes = FindObjectsOfType<NodeMarker>();
        foreach (var node in nodes)
        {
            if (node.IsWalkable)
            {
                mapNodes.Add(node.transform);
            }
        }

        // 初始化玩家位置到最近的可行走节点
        Transform closestNode = GetClosestNode(player.transform.position);
        if (closestNode != null)
        {
            player.transform.position = closestNode.position + startPositionOffset;
        }

        // 监听事件
        EVENTMGR.ChangeSteps += UpdateHighlightRadius;
        EVENTMGR.OnClickPlayer += OnClickPlayer;
        EVENTMGR.OnPlayerStep += OnShowFootprintInNode;
    }

    private void Update()
    {
        if (isCharacterSelected)
        {
            UpdateHighlightNodes();
        }
    }

    private void OnDestroy()
    {
        EVENTMGR.ChangeSteps -= UpdateHighlightRadius;
        EVENTMGR.OnClickPlayer -= OnClickPlayer;
        EVENTMGR.OnPlayerStep -= OnShowFootprintInNode;
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
        }
        else
        {
            ClearAllHighlights();
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
            NodeMarker nodeMarker = node.GetComponent<NodeMarker>();
            if (nodeMarker == null) continue;

            List<Transform> path = AStarPathfinding.FindPath(currentNode, node, mapNodes);

            if (path != null && path.Count <= highlightRadius)
            {
                newHighlightNodes.Add(node);
                if (!previousHighlightNodes.Contains(node)) // 只更新新的高亮节点
                {
                    nodeMarker.ShowHighlight();
                }
            }
        }

        // 关闭不在新高亮列表的节点
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

        // 更新上一帧高亮的节点
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
