using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathFinder))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NPCController))]
public class FollowPath : MonoBehaviour
{
    [Header("Path Following Settings")]
    public float nodeReachThreshold = 0.1f;
    public GameObject startNode;
    public GameObject endNode;
    public bool autoFindStartNode = true;
    public float FindStartRange = 10f;

    [Header("Movement Settings")]
    public float speed = 2f;

    private PathFinder pathFinder;
    private NPCController npcController;
    private List<GameObject> currentPath = new List<GameObject>();
    private int currentIndex = 0;

    private void Awake()
    {
        pathFinder = GetComponent<PathFinder>();
        npcController = GetComponent<NPCController>();
    }

    private void Start()
    {
        if (endNode != null)
        {
            FindNewPath();
        }
    }

    private void Update()
    {
        if (currentPath.Count == 0 || endNode == null)
        {
            return;
        }

        var targetNode = currentPath[currentIndex];
        var direction = targetNode.transform.position - transform.position;
        direction.z = 0;

        if (direction.magnitude < nodeReachThreshold)
        {
            currentIndex++;
            if (currentIndex >= currentPath.Count)
            {
                // Reached the end of the path
                currentPath.Clear();
                npcController.movement = Vector2.zero;
                return;
            }
            targetNode = currentPath[currentIndex];
            direction = targetNode.transform.position - transform.position;
        }

        Debug.DrawLine(transform.position, targetNode.transform.position, Color.black);
        npcController.movement = direction.normalized * speed;
    }

    [Button("Find New Path")]
    public void FindNewPath()
    {
        if (autoFindStartNode)
        {
            startNode = FindClosestNode();
        }

        if (startNode == null || endNode == null)
        {
            Debug.LogWarning("FollowPath: StartNode or EndNode is not set or found.");
            return;
        }

        currentPath.Clear();
        currentPath = pathFinder.FindPath(startNode, endNode);
        currentIndex = (currentPath.Count > 0) ? 0 : -1;

        if (currentPath.Count == 0 || endNode == null)
        {
            Debug.LogWarning($"FollowPath: No path found from StartNode to EndNode. Path count: {currentPath.Count}, start Node: {startNode}, endNode: {endNode}");
        }
    }

    private GameObject FindClosestNode()
    {
        float minDist = float.MaxValue;
        GameObject closest = null;

        // Find the closest node within the FindStartRange range
        var allNodes = GameObject.FindObjectsByType<PathFindNode>(FindObjectsSortMode.None);
        foreach (var node in allNodes)
        {
            float dist = Vector3.Distance(transform.position, node.transform.position);
            if (dist < minDist && dist <= FindStartRange)
            {
                minDist = dist;
                closest = node.gameObject;
            }
        }
        return closest;
    }
}