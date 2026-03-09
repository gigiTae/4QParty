using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using FQParty.Common.Persistance;

namespace FQParty.SceneManagement
{
    public class Bootstrapper : PersistanceSingleton<Bootstrapper>
    {
        //static readonly int SceneIndex = 0;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {

#if UNITY_EDITOR
            // Set the bootstrapper scene to be the play mode start scene when running in the editor
            // This will cause the bootstrapper scene to be loaded first (and only once) when entering
            // play mode from the Unity Editor, regardless of which scene is currently active.
         //   EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[sceneIndex].path);
#endif
        }

    }
}