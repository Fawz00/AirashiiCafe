using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    [Header("Pathfinder Settings")]
    public bool efficientMode = true; // toggle: false = BFS, true = Dijkstra

    public List<GameObject> FindPath(GameObject startNode, GameObject endNode)
    {
        if (efficientMode)
        {
            return FindEfficientPath(startNode, endNode); // pakai Dijkstra
        }
        else
        {
            return FindBlindPath(startNode, endNode); // pakai BFS
        }
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
}
