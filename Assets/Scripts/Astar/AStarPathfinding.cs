using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding
{
    public static List<Transform> FindPath(Transform startNode, Transform targetNode, List<Transform> nodes)
    {
        List<Transform> openSet = new List<Transform> { startNode };
        HashSet<Transform> closedSet = new HashSet<Transform>();

        Dictionary<Transform, Transform> cameFrom = new Dictionary<Transform, Transform>();
        Dictionary<Transform, float> gCost = new Dictionary<Transform, float>();
        Dictionary<Transform, float> fCost = new Dictionary<Transform, float>();

        foreach (var node in nodes)
        {
            gCost[node] = Mathf.Infinity;
            fCost[node] = Mathf.Infinity;
        }

        gCost[startNode] = 0;
        fCost[startNode] = Vector3.Distance(startNode.position, targetNode.position);

        while (openSet.Count > 0)
        {
            Transform currentNode = GetNodeWithLowestFCost(openSet, fCost);

            if (currentNode == targetNode)
            {
                return ReconstructPath(cameFrom, currentNode, startNode); // 传入 startNode
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            foreach (var neighbor in GetNeighbors(currentNode, nodes))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                float tentativeGCost = gCost[currentNode] + Vector3.Distance(currentNode.position, neighbor.position);

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeGCost >= gCost[neighbor])
                {
                    continue;
                }

                cameFrom[neighbor] = currentNode;
                gCost[neighbor] = tentativeGCost;
                fCost[neighbor] = gCost[neighbor] + Vector3.Distance(neighbor.position, targetNode.position);
            }
        }

        return null;
    }

    private static Transform GetNodeWithLowestFCost(List<Transform> openSet, Dictionary<Transform, float> fCost)
    {
        Transform lowestNode = openSet[0];
        float lowestCost = fCost[lowestNode];

        foreach (var node in openSet)
        {
            if (fCost[node] < lowestCost)
            {
                lowestNode = node;
                lowestCost = fCost[node];
            }
        }

        return lowestNode;
    }

    private static List<Transform> GetNeighbors(Transform node, List<Transform> nodes)
    {
        List<Transform> neighbors = new List<Transform>();
        foreach (var potentialNeighbor in nodes)
        {
            if (Vector3.Distance(node.position, potentialNeighbor.position) == 1f) // 确保邻居是四向
            {
                neighbors.Add(potentialNeighbor);
            }
        }
        return neighbors;
    }

    private static List<Transform> ReconstructPath(Dictionary<Transform, Transform> cameFrom, Transform currentNode, Transform startNode)
    {
        List<Transform> path = new List<Transform> { currentNode };

        while (cameFrom.ContainsKey(currentNode))
        {
            currentNode = cameFrom[currentNode];
            if (currentNode != startNode) // 排除起始节点
            {
                path.Add(currentNode);
            }
        }

        path.Reverse();
        return path;
    }
}
