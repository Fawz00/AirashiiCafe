using System;
using Core.Events;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] public bool isLocalDebug = false;

    [Header("References")]
    [SerializeField] public CameraController mainCamera;
    [SerializeField] public PlayerControllerBase playerControllerBase;
    [SerializeField] public PlayerData_SO playerData;

    [Header("Prefab")]
    [SerializeField] public GameObject mainCameraPrefab;
    public UIStack CurrentUIStack => UIManager.Instance.CurrentUI;

    public override void Awake()
    {
        base.Awake();

        // Initialize playerDaata if they are not set
        if (playerData == null)
        {
            playerData = new PlayerData_SO();
        }

        if (playerControllerBase == null)
        {
            Debug.LogWarning("PlayerControllerBase is not set. Please assign it in the inspector or through code.");
        }
    }

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<CameraController>();
            if (mainCamera == null)
            {
                mainCamera = Instantiate(mainCameraPrefab).GetComponent<CameraController>();

                if (mainCamera == null)
                {
                    Debug.LogError("Failed to instantiate main camera from prefab.");
                }
            }
        }
        
        if (!isLocalDebug) LevelManager.LoadSubLevel(Common.gameScene);
    }
}
