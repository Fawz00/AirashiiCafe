using UnityEngine;

[RequireComponent(typeof(NPCController))]
public class FollowTarget : MonoBehaviour
{
    [Header("Target Settings")]
    public GameObject priorityTarget;
    public string targetTag = "Player";
    public float speed = 2f;
    public float minimumDistance = 0.1f;
    public float maximumDistance = 20f;

    [Header("Teleportation Settings")]
    public bool teleportToTarget = false;
    [Tooltip("If the target is beyond this range, the NPC will teleport closer.")]
    public float teleportRange = 20f;
    public float teleportDistance = 10f;

    private GameObject currentTarget;
    private NPCController npcController;

    private void Awake()
    {
        npcController = GetComponent<NPCController>();
    }

    void Update()
    {
        FindNearestTarget();

        if (currentTarget != null && npcController != null)
        {
            Vector3 direction3 = currentTarget.transform.position - transform.position;
            Vector2 direction = new Vector2(direction3.x, direction3.y);

            if (direction.magnitude >= minimumDistance && direction.magnitude < maximumDistance)
            {
                npcController.movement = direction3.normalized * speed;
            }
            else
            {
                npcController.movement = Vector2.zero;
            }

            if (teleportToTarget && direction.magnitude >= teleportRange)
            {
                Vector3 newPosition = transform.position + (direction3.normalized * (direction3.magnitude - teleportDistance));
                transform.position = newPosition;

                npcController.movement = Vector2.zero;
            }
        }
        else
        {
            Debug.LogWarning("SimpleFollowTarget: No valid target found!");
        }
    }

    private void FindNearestTarget()
    {
        if (priorityTarget != null)
        {
            currentTarget = priorityTarget;
            return;
        }

        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        float closestDistance = float.MaxValue;
        GameObject nearest = null;

        foreach (GameObject obj in targets)
        {
            float dist = Vector3.Distance(transform.position, obj.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                nearest = obj;
            }
        }

        currentTarget = nearest;
    }
}
