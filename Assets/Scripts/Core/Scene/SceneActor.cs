using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneActor : MonoBehaviour
{
    [SerializeField] public Common.PlayerControlType playerControlType = Common.PlayerControlType.UIOnly;

    public GameObject player;
    void Awake()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager instance is null. Reverting to parent level scene.");
            LevelManager.GlobalResetAndLoad(Common.startupScene);
            return;
        }
        if (player == null)
        {
            // get the player from Player tag
            player = GameManager.Instance.playerControllerBase.gameObject ?? GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Player GameObject not found. Please ensure a GameObject with the Player tag exists in the scene.");
                return;
            }
        }

        // Set the player control type
        switch (playerControlType)
        {
            case Common.PlayerControlType.GameOnly:
                GameManager.Instance.playerControllerBase.SetPlayerControlType(Common.PlayerControlType.GameOnly);
                break;
            case Common.PlayerControlType.UIOnly:
                GameManager.Instance.playerControllerBase.SetPlayerControlType(Common.PlayerControlType.UIOnly);
                break;
            case Common.PlayerControlType.Dynamic:
                Debug.LogWarning("Dynamic control type is not available in this context. Defaulting to UiOnly control type.");
                GameManager.Instance.playerControllerBase.SetPlayerControlType(Common.PlayerControlType.UIOnly);
                break;
            default:
                GameManager.Instance.playerControllerBase.SetPlayerControlType(Common.PlayerControlType.All);
                break;
        }
    }

    void Start()
    {
        // UI_Manager.Instance.AddUI(UI_Manager.Instance.GetUiFromResource("AktivitasSiswa/as_ar_scanner_page"), false, true, true);
    }

    void Update()
    {
        // Update camera here if needed
    }

    public Common.PlayerControlType GetPlayerControlType()
    {
        return playerControlType;
    }
}
