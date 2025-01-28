using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 如果不需要玩家点击主控才显示高亮区域，就直接在update里持续更新高亮区域即可
// 目前状态：不点击主控直接点击可行走区域也能移动
// 有关点击移动的逻辑在playerMovement.cs
public class PathfindingManager : MonoBehaviour
{
    public float highlightRadius = 5f; // 高亮半径

    private GameObject player;
    private Vector3 startPositionOffset = new Vector3(0, 1.5f, 0);
    public List<Transform> mapNodes = new List<Transform>();

    private bool isCharacterSelected = false; // 是否选中了玩家

    void Start()
    {
        // 获取玩家对象
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("玩家对象未找到");
            return;
        }

        // 遍历所有节点并存储
        NodeMarker[] nodes = FindObjectsOfType<NodeMarker>();
        foreach (var node in nodes)
        {
            if (node.IsWalkable)
            {
                mapNodes.Add(node.transform);
            }
        }

        // 初始化玩家位置到最近节点
        Transform closestNode = GetClosestNode(player.transform.position);
        if (closestNode != null)
        {
            player.transform.position = closestNode.position + startPositionOffset;
        }

        EVENTMGR.ChangeSteps += UpdateHighlightRadius;
        EVENTMGR.OnClickCharacter += OnClickCharacter;
    }

    private void OnDestroy()
    {
        EVENTMGR.ChangeSteps -= UpdateHighlightRadius;
        EVENTMGR.OnClickCharacter -= OnClickCharacter;
    }

    private void UpdateHighlightRadius(int highlightRadius)
    {
        this.highlightRadius = highlightRadius;
    }

    // 获取距离最近的节点
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

    // 玩家点击事件
    void OnClickCharacter(bool isClick)
    {
        isCharacterSelected = isClick;

        if (isCharacterSelected)
        {
            HighlightNearbyNodes();
        }
        else
        {
            ClearAllHighlights();
        }
    }

    // 高亮步数范围内的节点
    void HighlightNearbyNodes()
    {
        if (player == null) return;

        Transform currentNode = GetClosestNode(player.transform.position - startPositionOffset);

        foreach (var node in mapNodes)
        {
            NodeMarker nodeMarker = node.GetComponent<NodeMarker>();
            if (nodeMarker == null) continue;

            // 计算玩家到当前节点的路径
            List<Transform> path = AStarPathfinding.FindPath(currentNode, node, mapNodes);

            // 如果路径步数小于等于剩余步数，显示高亮；否则隐藏
            if (path != null && path.Count <= highlightRadius)
            {
                nodeMarker.ShowHighlight();
            }
            else
            {
                nodeMarker.HideHighlight();
            }
        }
    }

    // 清除所有节点的高亮
    void ClearAllHighlights()
    {
        foreach (var node in mapNodes)
        {
            NodeMarker nodeMarker = node.GetComponent<NodeMarker>();
            if (nodeMarker != null)
            {
                nodeMarker.HideHighlight();
            }
        }
    }
}
