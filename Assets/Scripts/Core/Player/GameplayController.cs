using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [Header("Game Control Settings")]
    [SerializeField] public bool allowControl = true;

    private PlayerControllerBase playerControllerBase;

    private void Awake()
    {
        playerControllerBase = gameObject.GetComponent<PlayerControllerBase>();
        if (playerControllerBase == null)
        {
            Debug.LogWarning("PlayerControllerBase component not found on the GameObject. Defaulting allowControl to false.");
            allowControl = false;
        }
    }

    private void OnEnable()
    {
        allowControl = playerControllerBase != null && playerControllerBase.allowControl;
    }
    private void OnDisable()
    {
        allowControl = false;
    }

    private void Update()
    {
        PlayerInput();
    }



    #region Player Methods

    private void PlayerInput()
    {
        if (!allowControl) return;
        InputSystem_Actions inputActions = playerControllerBase.InputActions;
        
        // Some input handling logic here
    }

    #endregion Player Methods
}
