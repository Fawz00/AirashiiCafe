using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager
{
    public static void LoadSubLevel(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            if (GameManager.Instance.playerData != null)
            {
                GameManager.Instance.playerData.currentScene = sceneName;
            }
            else
            {
                Debug.LogWarning("GameManager or playerData is null. Cannot set current scene.");
            }

            try
            {
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to load scene '{sceneName}': {ex.Message}");
            }
        }
    }

    public static void LoadLevel(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            if (GameManager.Instance.playerData != null)
            {
                GameManager.Instance.playerData.currentScene = sceneName;
            }
            else
            {
                Debug.LogWarning("GameManager or playerData is null. Cannot set current scene.");
            }

            try
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to load scene '{sceneName}': {ex.Message}");
            }
        }
    }

    public static void UnloadSubLevel(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            if (GameManager.Instance.playerData != null)
            {
                GameManager.Instance.playerData.currentScene = SceneManager.GetActiveScene().name;
            }
            else
            {
                Debug.LogWarning("GameManager or playerData is null. Cannot set current scene.");
            }
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }

    public static SceneActor GetSceneActor()
    {
        SceneActor levelActor = MonoBehaviour.FindFirstObjectByType<SceneActor>(FindObjectsInactive.Exclude);
        if (levelActor == null)
        {
            Debug.LogWarning("No LevelActor found in the scene.");
        }
        return levelActor;
    }

    public static void GlobalResetAndLoad(string sceneName)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.hideFlags == HideFlags.None)
            {
                MonoBehaviour.Destroy(obj);
            }
        }

        SceneManager.LoadScene(sceneName);
    }
}
