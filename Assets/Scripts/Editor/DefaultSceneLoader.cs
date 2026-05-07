#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class DefaultSceneLoader
{
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";

    static DefaultSceneLoader()
    {
        EditorApplication.delayCall += OpenSampleSceneIfEditorStartsEmpty;
    }

    private static void OpenSampleSceneIfEditorStartsEmpty()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.path == ScenePath)
            return;

        if (!string.IsNullOrEmpty(activeScene.path) || activeScene.isDirty)
            return;

        EditorSceneManager.OpenScene(ScenePath);
    }
}
#endif
