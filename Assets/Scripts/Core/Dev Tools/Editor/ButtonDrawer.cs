using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(MonoBehaviour), true)]
public class ButtonDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var methods = target.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var method in methods)
        {
            var buttonAttr = method.GetCustomAttribute<ButtonAttribute>();
            if (buttonAttr != null)
            {
                string label = string.IsNullOrEmpty(buttonAttr.label) ? method.Name : buttonAttr.label;

                bool previousGUIState = GUI.enabled;

                if (buttonAttr.onlyInPlayMode)
                {
                    GUI.enabled = Application.isPlaying;
                }

                if (GUILayout.Button(label))
                {
                    method.Invoke(target, null);

                    EditorUtility.SetDirty(target);
                    var monoBehaviour = target as MonoBehaviour;
                    if (monoBehaviour != null && monoBehaviour.gameObject.scene.IsValid())
                    {
                        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(monoBehaviour.gameObject.scene);
                    }
                }

                GUI.enabled = previousGUIState;
            }
        }
    }
}
