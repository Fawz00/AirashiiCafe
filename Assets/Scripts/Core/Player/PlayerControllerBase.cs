using System;
using UnityEngine;

public class PlayerControllerBase : MonoBehaviour
{
    [Header("Player Control Settings")]
    [SerializeField] public bool allowControl = true;
    [NonSerialized] public GameplayController arController;

    private InputSystem_Actions inputActions;

    public InputSystem_Actions InputActions => inputActions;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        inputActions = new InputSystem_Actions();
        arController = GetComponent<GameplayController>();
        if (arController == null)
        {
            Debug.LogWarning("ARController component not found on the GameObject.");
        }

        arController.allowControl = false;
    }

    private void OnEnable()
    {
        inputActions.Enable();
        allowControl = true;
    }
    private void OnDisable()
    {
        allowControl = false;
        inputActions.Disable();
    }

    public void SetPlayerControlType(Common.PlayerControlType controlType)
    {        
        switch (controlType)
        {
            case Common.PlayerControlType.GameOnly:
                arController.allowControl = true;
                break;
            case Common.PlayerControlType.UIOnly:
                arController.allowControl = false;
                break;
            case Common.PlayerControlType.Dynamic:
                {
                    // Get Prefab with SceneActor Component from current Scene to determine control type
                    SceneActor levelActor = LevelManager.GetSceneActor();
                    if (levelActor == null)
                    {
                        Debug.LogWarning("SceneActor not found in the scene. Defaulting to UiOnly control type.");
                        SetPlayerControlType(Common.PlayerControlType.UIOnly);
                        return;
                    }
                    var controllerType = levelActor.GetPlayerControlType();
                    SetPlayerControlType(controllerType);
                    break;
                }
            case Common.PlayerControlType.All:
                arController.allowControl = true;
                break;
            case Common.PlayerControlType.None:
                arController.allowControl = false;
                break;
        }
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0f;
            SetPlayerControlType(Common.PlayerControlType.UIOnly);
        }
        else
        {
            Time.timeScale = 1f;
            SetPlayerControlType(Common.PlayerControlType.Dynamic);
        }
    }
}
