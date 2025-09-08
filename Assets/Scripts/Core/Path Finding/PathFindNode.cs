using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[RequireComponent(typeof(Collider2D))]
[ExecuteAlways]
public class PathFindNode : MonoBehaviour
{
    public List<GameObject> next = new List<GameObject>();
    public List<GameObject> previous = new List<GameObject>();
    public UnityEvent<GameObject> onTouchingThis;

    [Header("Auto Setup")]
    public float connectionRange = 0.7f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        onTouchingThis?.Invoke(collision.gameObject); // Invoke the event when another collider enters this node's collider
    }

    public IEnumerable<GameObject> GetNeighbors()
    {
        foreach (var n in next) yield return n;
        foreach (var p in previous) yield return p;
    }

    [Button("Auto Connect Nodes")]
    public void AutoConnectNodes()
    {
        var allNodes = FindObjectsByType<PathFindNode>(FindObjectsSortMode.None);
        foreach (var node in allNodes)
        {
            if (node == this) continue;

            float distance = Vector3.Distance(transform.position, node.transform.position);
            if (distance <= connectionRange)
            {
                // Determine direction based on relative position
                if (node.transform.position.x > transform.position.x)
                {
                    // Node is to the right
                    if (!next.Contains(node.gameObject))
                        next.Add(node.gameObject);
                    if (!node.previous.Contains(this.gameObject))
                        node.previous.Add(this.gameObject);
                }
                else
                {
                    // Node is to the left
                    if (!previous.Contains(node.gameObject))
                        previous.Add(node.gameObject);
                    if (!node.next.Contains(this.gameObject))
                        node.next.Add(this.gameObject);
                }

#if UNITY_EDITOR
    EditorUtility.SetDirty(node);
#endif
            }
        }

#if UNITY_EDITOR
    EditorUtility.SetDirty(this);
    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }

    [Button("Remove Connection")]
    public void RemoveConnection()
    {
        var allNodes = FindObjectsByType<PathFindNode>(FindObjectsSortMode.None);
        foreach (var node in allNodes)
        {
            if (node == this) continue;

            float distance = Vector3.Distance(transform.position, node.transform.position);
            if (distance <= connectionRange)
            {
                if (next.Contains(node.gameObject))
                    next.Remove(node.gameObject);
                if (previous.Contains(node.gameObject))
                    previous.Remove(node.gameObject);

                if (node.next.Contains(this.gameObject))
                    node.next.Remove(this.gameObject);
                if (node.previous.Contains(this.gameObject))
                    node.previous.Remove(this.gameObject);
#if UNITY_EDITOR
                EditorUtility.SetDirty(node);
#endif
            }
        }
        
#if UNITY_EDITOR
    EditorUtility.SetDirty(this);
    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }

    [Button("Remove All Connection to this Node")]
    public void RemoveAllConnectionToThisNode()
    {
        var allNodes = FindObjectsByType<PathFindNode>(FindObjectsSortMode.None);
        foreach (var node in allNodes)
        {
            if (node.next.Contains(this.gameObject))
            {
                node.next.Remove(this.gameObject);
#if UNITY_EDITOR
    EditorUtility.SetDirty(node);
#endif
            }
            if (node.previous.Contains(this.gameObject))
            {
                node.previous.Remove(this.gameObject);
#if UNITY_EDITOR
    EditorUtility.SetDirty(node);
#endif
            }
        }

#if UNITY_EDITOR
    EditorUtility.SetDirty(this);
    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }

    [Button("Clear Connections")]
    public void ClearConnections()
    {
        next.Clear();
        previous.Clear();

#if UNITY_EDITOR
    EditorUtility.SetDirty(this);
    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }

    // Draw lines to next and previous nodes in the editor for visualization
    private void OnDrawGizmos()
    {
        // Draw a wire sphere to indicate connection range
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, connectionRange);

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