using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PathFinder))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NPCController))]
public class FollowPath : MonoBehaviour
{
    [Header("Path Events")]
    public UnityEvent<GameObject, GameObject> onPathCompleted; // Event triggered when the path is completed, passing start and end nodes

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
                onPathCompleted?.Invoke(startNode, endNode);

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
            startNode = FindClosestNode().gameObject;
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

    public PathFindNode FindClosestNode(float range = -1f)
    {
        float minDist = float.MaxValue;
        PathFindNode closest = null;

        // Use provided range if specified, otherwise use FindStartRange
        float effectiveRange = (range > 0f) ? range : FindStartRange;

        var allNodes = GameObject.FindObjectsByType<PathFindNode>(FindObjectsSortMode.None);
        foreach (var node in allNodes)
        {
            float dist = Vector3.Distance(transform.position, node.transform.position);
            if (dist < minDist && (effectiveRange < 0f || dist <= effectiveRange))
            {
                minDist = dist;
                closest = node;
            }
        }
        return closest;
    }
}