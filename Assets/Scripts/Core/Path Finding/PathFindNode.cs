using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class PathFindNode : MonoBehaviour
{
    public List<GameObject> next = new List<GameObject>();
    public List<GameObject> previous = new List<GameObject>();
    public UnityEvent<GameObject> onTouchingThis;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        onTouchingThis?.Invoke(collision.gameObject); // Invoke the event when another collider enters this node's collider
    }

    public IEnumerable<GameObject> GetNeighbors()
    {
        foreach (var n in next) yield return n;
        foreach (var p in previous) yield return p;
    }

    // Draw lines to next and previous nodes in the editor for visualization
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var node in next)
        {
            if (node != null)
            {
                Vector3 target = node.transform.position - transform.position;
                Gizmos.DrawLine(transform.position, target.normalized + transform.position);
            }
        }

        Gizmos.color = Color.red;
        foreach (var node in previous)
        {
            if (node != null)
            {
                Vector3 target = node.transform.position - transform.position;
                Gizmos.DrawLine(transform.position, target.normalized + transform.position);
            }
        }
    }
}