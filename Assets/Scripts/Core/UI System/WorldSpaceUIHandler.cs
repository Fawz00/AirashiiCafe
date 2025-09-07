using UnityEngine;
using UnityEngine.UIElements;

public class WorldSpaceUIHandler : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    [Header("UI Data")]
    [SerializeField] private UIAsset_SO uiAsset;

    private void OnEnable()
    {
        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("WorldSpaceUIHandler: UIDocument component is missing.");
            return;
        }

        uiDocument.panelSettings.SetScreenToPanelSpaceFunction((Vector2 screenPosition) =>
        {
            var invalidPosition = new Vector2(float.NaN, float.NaN);

            var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(cameraRay, out hit, 12f, LayerMask.GetMask("UI")))
            {
                return invalidPosition;
            }

            Vector2 pixelUV = hit.textureCoord;
            pixelUV.y = 1 - pixelUV.y; // Invert Y coordinate
            pixelUV.x *= uiDocument.panelSettings.targetTexture.width;
            pixelUV.y *= uiDocument.panelSettings.targetTexture.height;

            return pixelUV;
        });

        if (uiAsset != null)
        {
            uiDocument.visualTreeAsset = uiAsset.uiAsset;

            if (!string.IsNullOrEmpty(uiAsset.handlerClassName))
            {
                var handlerType = System.Type.GetType(uiAsset.handlerClassName);
                if (handlerType == null)
                {
                    // Try to find the type in all loaded assemblies
                    foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                    {
                        handlerType = assembly.GetType(uiAsset.handlerClassName);
                        if (handlerType != null)
                            break;
                    }
                }

                if (handlerType != null)
                {
                    uiDocument.gameObject.AddComponent(handlerType);
                }
                else
                {
                    Debug.LogError($"Handler class '{uiAsset.handlerClassName}' not found in any loaded assembly.");
                }
            }
        }
        else
        {
            Debug.LogError("WorldSpaceUIHandler: UIAsset_SO is not assigned.");
        }
    }
}
