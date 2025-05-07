using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class AStarPathfinding
{
    public static List<Transform> FindPath(Transform startNode, Transform targetNode, List<Transform> nodes, bool allowNonWalkable = false)
    {
        // 首先检查起点和终点是否有效
        if (startNode == null || !startNode.gameObject.activeSelf || 
            targetNode == null || !targetNode.gameObject.activeSelf)
        {
            return null;
        }

        SortedList<float, Queue<Transform>> openSet = new SortedList<float, Queue<Transform>>();
        HashSet<Transform> closedSet = new HashSet<Transform>();
        Dictionary<Transform, Transform> cameFrom = new Dictionary<Transform, Transform>();
        Dictionary<Transform, float> gCost = new Dictionary<Transform, float>();
        Dictionary<Transform, float> fCost = new Dictionary<Transform, float>();

        // 初始化所有节点成本，只处理激活的节点
        foreach (var node in nodes)
        {
            if (!node.gameObject.activeSelf) continue;
            
            gCost[node] = float.MaxValue;
            fCost[node] = float.MaxValue;
        }

        gCost[startNode] = 0;
        fCost[startNode] = Vector3.SqrMagnitude(startNode.position - targetNode.position);

        Enqueue(openSet, startNode, fCost[startNode]);

        while (openSet.Count > 0)
        {
            Transform currentNode = Dequeue(openSet);
            if (currentNode == targetNode)
                return ReconstructPath(cameFrom, currentNode);

            closedSet.Add(currentNode);

            foreach (var neighbor in GetFastNeighbors(currentNode, nodes, allowNonWalkable))
            {
                // 确保邻居节点是激活的
                if (!neighbor.gameObject.activeSelf || closedSet.Contains(neighbor)) 
                    continue;

                float tentativeGCost = gCost[currentNode] + Vector3.Distance(currentNode.position, neighbor.position);

                if (tentativeGCost < gCost[neighbor])
                {
                    cameFrom[neighbor] = currentNode;
                    gCost[neighbor] = tentativeGCost;
                    fCost[neighbor] = tentativeGCost + Vector3.SqrMagnitude(neighbor.position - targetNode.position);

                    Enqueue(openSet, neighbor, fCost[neighbor]);
                }
            }
        }
        return null;
    }

    private static List<Transform> GetFastNeighbors(Transform node, List<Transform> nodes, bool allowNonWalkable)
    {
        float threshold = allowNonWalkable ? 2f : 1.2f;

        List<Transform> neighbors = new List<Transform>();
        foreach (var n in nodes)
        {
            // 只考虑激活的节点
            if (!n.gameObject.activeSelf)
                continue;
            
            if (Vector3.SqrMagnitude(node.position - n.position) <= threshold * threshold)
                neighbors.Add(n);
        }
        return neighbors;
    }

    private static List<Transform> ReconstructPath(Dictionary<Transform, Transform> cameFrom, Transform currentNode)
    {
        List<Transform> path = new List<Transform>();
        while (cameFrom.ContainsKey(currentNode))
        {
            path.Add(currentNode);
            currentNode = cameFrom[currentNode];
        }
        path.Reverse();
        return path;
    }
    
    private static void Enqueue(SortedList<float, Queue<Transform>> openSet, Transform item, float priority)
    {
        if (!openSet.ContainsKey(priority))
        {
            openSet[priority] = new Queue<Transform>();
        }
        openSet[priority].Enqueue(item);
    }
    
    private static Transform Dequeue(SortedList<float, Queue<Transform>> openSet)
    {
        if (openSet.Count == 0)
            return null;

        var firstEntry = openSet.First();
        var queue = firstEntry.Value;
        var item = queue.Dequeue();
        if (queue.Count == 0)
        {
            openSet.Remove(firstEntry.Key);
        }
        return item;
    }
}