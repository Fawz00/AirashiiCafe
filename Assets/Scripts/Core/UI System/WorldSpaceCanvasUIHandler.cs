using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Canvas))]
public class WorldSpaceCanvasUIHandler : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    private Canvas canvas;

    void Awake()
    {
        if (cameraController == null) cameraController = GameManager.Instance.mainCamera;
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("WorldSpaceCanvasUIHandler: No Canvas component found.");
            return;
        }

        if (cameraController == null)
        {
            cameraController = FindFirstObjectByType<CameraController>();
            if (cameraController == null)
            {
                Debug.LogError("WorldSpaceCanvasUIHandler: No CameraController found in the scene.");
                return;
            }
        }

        canvas.worldCamera = cameraController.mainCamera;
    }
}