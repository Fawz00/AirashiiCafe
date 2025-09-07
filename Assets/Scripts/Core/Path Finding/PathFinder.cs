using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    [Header("Pathfinder Settings")]
    public bool efficientMode = true; // toggle: false = BFS, true = Dijkstra

    public List<GameObject> FindPath(GameObject startNode, GameObject endNode)
    {
        var endComp = endNode.GetComponent<PathFindNode>();

        // Jika endNode bukan node graph, cari node graf terdekat
        GameObject actualEnd = endNode;
        bool appendEndLater = false;

        if (endComp == null)
        {
            actualEnd = FindClosestNode(endNode);
            appendEndLater = true;
        }

        List<GameObject> path;
        if (efficientMode)
        {
            path = FindEfficientPath(startNode, actualEnd);
        }
        else
        {
            path = FindBlindPath(startNode, actualEnd);
        }

        // Tambahkan target asli kalau bukan PathFindNode
        if (appendEndLater && path.Count > 0)
        {
            path.Add(endNode);
        }

        return path;
    }

    // ========== MODE 1: BFS (jumlah langkah paling sedikit) ==========
    private List<GameObject> FindBlindPath(GameObject startNode, GameObject endNode)
    {
        var queue = new Queue<GameObject>();
        var visited = new HashSet<GameObject>();
        var cameFrom = new Dictionary<GameObject, GameObject>();

        queue.Enqueue(startNode);
        visited.Add(startNode);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == endNode)
                return ReconstructPath(cameFrom, current);

            var nodeComp = current.GetComponent<PathFindNode>();
            if (nodeComp == null) continue;

            foreach (var neighbor in nodeComp.GetNeighbors())
            {
                if (visited.Contains(neighbor)) continue;

                visited.Add(neighbor);
                cameFrom[neighbor] = current;
                queue.Enqueue(neighbor);
            }
        }

        return new List<GameObject>(); // tidak ketemu path
    }

    // ========== MODE 2: Dijkstra (jalur fisik terpendek) ==========
    private List<GameObject> FindEfficientPath(GameObject startNode, GameObject endNode)
    {
        var openSet = new List<GameObject> { startNode };
        var cameFrom = new Dictionary<GameObject, GameObject>();
        var gScore = new Dictionary<GameObject, float>();

        gScore[startNode] = 0;

        while (openSet.Count > 0)
        {
            // ambil node dengan jarak terpendek sejauh ini
            GameObject current = GetLowestGScoreNode(openSet, gScore);
            if (current == endNode)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);

            var nodeComp = current.GetComponent<PathFindNode>();
            if (nodeComp == null) continue;

            foreach (var neighbor in nodeComp.GetNeighbors())
            {
                if (neighbor == null)
                {
                    Debug.LogWarning("PathFinder: Neighbor node is null, skipping.");
                    continue;
                }
                float tentativeG = gScore[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    gScore[neighbor] = tentativeG;
                    cameFrom[neighbor] = current;
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return new List<GameObject>(); // tidak ketemu path
    }

    private GameObject GetLowestGScoreNode(List<GameObject> nodes, Dictionary<GameObject, float> gScore)
    {
        GameObject lowestNode = null;
        float lowestScore = float.MaxValue;
        foreach (var node in nodes)
        {
            float score = gScore.ContainsKey(node) ? gScore[node] : float.MaxValue;
            if (score < lowestScore)
            {
                lowestScore = score;
                lowestNode = node;
            }
        }
        return lowestNode;
    }

    // ========== Reconstruct path (dipakai di kedua mode) ==========
    private List<GameObject> ReconstructPath(Dictionary<GameObject, GameObject> cameFrom, GameObject current)
    {
        var totalPath = new List<GameObject> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        return totalPath;
    }

    // ========== Find closest node to a target (dipakai di FollowPath) ==========
    private GameObject FindClosestNode(GameObject target)
    {
        var allNodes = FindObjectsByType<PathFindNode>(FindObjectsSortMode.None);
        GameObject closest = null;
        float minDist = float.MaxValue;

        foreach (var node in allNodes)
        {
            float dist = Vector3.Distance(target.transform.position, node.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = node.gameObject;
            }
        }

        return closest;
    }
}
