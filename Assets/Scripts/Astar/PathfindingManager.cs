using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathfindingManager : MonoBehaviour
{
    public float highlightRadius = 5f; // 高亮半径

    private GameObject player;
    private Vector3 startPositionDiatance = new Vector3(0, 1.5f, 0);
    public List<Transform> mapNodes = new List<Transform>();

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

        Debug.Log($"共找到 {mapNodes.Count} 个可行走节点！");

        // 初始化玩家位置到最近节点
        Transform closestNode = GetClosestNode(player.transform.position);
        if (closestNode != null)
        {
            player.transform.position = closestNode.position + startPositionDiatance;
        }

        EVENTMGR.ChangeSteps += UpdateHighlightRadius;
    }

    void Update()
    {
        HighlightNearbyNodes();
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


    void HighlightNearbyNodes()
    {
        if (player == null) return;

        // 遍历所有节点，找到步数范围内的节点
        foreach (var node in mapNodes)
        {
            NodeMarker nodeMarker = node.GetComponent<NodeMarker>();
            if (nodeMarker == null) continue;

            // 使用 A* 寻找从玩家到当前节点的路径
            Transform currentNode = GetClosestNode(player.transform.position - startPositionDiatance);
            List<Transform> path = AStarPathfinding.FindPath(currentNode, node, mapNodes);

            // 如果路径步数小于等于剩余步数，显示高亮；否则隐藏高亮
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
}