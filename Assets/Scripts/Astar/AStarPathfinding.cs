using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding
{
    /// <summary>
    /// 寻路 允许选择是否考虑不可行走区域
    /// </summary>
    /// <param name="startNode">起点</param>
    /// <param name="targetNode">目标点</param>
    /// <param name="nodes">所有候选节点</param>
    /// <param name="allowNonWalkable">是否允许在不可行走范围内寻路，默认为 false</param>
    /// <returns>路径节点列表，如果未找到返回 null</returns>
    public static List<Transform> FindPath(Transform startNode, Transform targetNode, List<Transform> nodes, bool allowNonWalkable = false)
    {
        // 优先队列存储 openSet
        PriorityQueue<Transform, float> openSet = new PriorityQueue<Transform, float>();
        HashSet<Transform> closedSet = new HashSet<Transform>();

        Dictionary<Transform, Transform> cameFrom = new Dictionary<Transform, Transform>();
        Dictionary<Transform, float> gCost = new Dictionary<Transform, float>();
        
        foreach (var node in nodes)
        {
            gCost[node] = Mathf.Infinity;
        }

        gCost[startNode] = 0;
        float startToTargetCost = Vector3.SqrMagnitude(startNode.position - targetNode.position);
        openSet.Enqueue(startNode, startToTargetCost);

        while (openSet.Count > 0)
        {
            Transform currentNode = openSet.Dequeue();

            // 检查是否到达目标节点
            if (currentNode == targetNode)
            {
                return ReconstructPath(cameFrom, currentNode);
            }

            closedSet.Add(currentNode);

            foreach (var neighbor in GetNeighbors(currentNode, nodes, allowNonWalkable))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                float tentativeGCost = gCost[currentNode] + Vector3.Distance(currentNode.position, neighbor.position);

                if (tentativeGCost < gCost[neighbor])
                {
                    cameFrom[neighbor] = currentNode;
                    gCost[neighbor] = tentativeGCost;

                    float priority = tentativeGCost + Vector3.SqrMagnitude(neighbor.position - targetNode.position);
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Enqueue(neighbor, priority);
                    }
                }
            }
        }

        return null; // 如果未找到路径
    }

    /// <summary>
    /// 根据当前节点获取其邻居节点。
    /// 当 allowNonWalkable 为 false 时，仅返回距离小于等于 1.2f 的节点；
    /// 当 allowNonWalkable 为 true 时，返回距离小于等于 3.6f（即 1.2f 的 3 倍）的节点。
    /// </summary>
    private static List<Transform> GetNeighbors(Transform node, List<Transform> nodes, bool allowNonWalkable)
    {
        List<Transform> neighbors = new List<Transform>();
        float threshold = allowNonWalkable ? 3.6f : 1.2f;
        
        foreach (var potentialNeighbor in nodes)
        {
            if (Vector3.Distance(node.position, potentialNeighbor.position) <= threshold)
            {
                neighbors.Add(potentialNeighbor);
            }
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
}

// 优先队列实现
public class PriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
{
    private List<KeyValuePair<TElement, TPriority>> elements = new List<KeyValuePair<TElement, TPriority>>();

    public int Count => elements.Count;

    public void Enqueue(TElement element, TPriority priority)
    {
        elements.Add(new KeyValuePair<TElement, TPriority>(element, priority));
        elements.Sort((x, y) => x.Value.CompareTo(y.Value)); // 按优先级排序
    }

    public TElement Dequeue()
    {
        var item = elements[0];
        elements.RemoveAt(0);
        return item.Key;
    }

    public bool Contains(TElement element)
    {
        foreach (var item in elements)
        {
            if (EqualityComparer<TElement>.Default.Equals(item.Key, element))
                return true;
        }
        return false;
    }
}
