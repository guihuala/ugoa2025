using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathfindingManager : MonoBehaviour
{
    public float highlightRadius = 5f; // 高亮半径

    private GameObject player;
    private Vector3 startPositionDiatance = new Vector3(0, 1.9f, 0);
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

        foreach (var node in mapNodes)
        {
            float distance = Vector3.Distance(player.transform.position, node.position);
            NodeMarker nodeMarker = node.GetComponent<NodeMarker>();

            if (distance - 1 <= highlightRadius && nodeMarker != null)
            {
                nodeMarker.ShowHighlight();
            }
            else if (nodeMarker != null)
            {
                nodeMarker.HideHighlight();
            }
        }
    }

}
