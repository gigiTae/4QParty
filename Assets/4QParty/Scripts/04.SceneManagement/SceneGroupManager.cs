using Eflatun.SceneReference;
using FQParty.Common.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace FQParty.SceneManagement
{
    public class SceneGroupManager
    {
        public event Action<string> OnSceneLoaded = delegate { };
        public event Action<string> OnSceneUnloaded = delegate { };
        public event Action OnSceneGroupLoaded = delegate { };

        SceneGroup m_ActiveSceneGroup;
        NetworkSceneManager m_NetworkSceneManager;

        public async Task LoadSceneGroupAsync(SceneGroup group, bool reloadDupScenes = false)
        {
            m_ActiveSceneGroup = group;
            var loadedScenes = new List<string>();

            await UnloadScenesAsync();

            int sceneCount = SceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }

            var totalScenesToLoad = m_ActiveSceneGroup.Scenes.Count;
            var operationGroup = new AsyncOperationGroup(totalScenesToLoad);

            for (var i = 0; i < totalScenesToLoad; i++)
            {
                var sceneData = group.Scenes[i];
                if (reloadDupScenes == false && loadedScenes.Contains(sceneData.Name)) continue;

                if (sceneData.Reference.State == SceneReferenceState.Regular)
                {
                    var operation = SceneManager.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);
                    operationGroup.Operations.Add(operation);
                }

                OnSceneLoaded.Invoke(sceneData.Name);
            }

            while (!operationGroup.IsDone)
            {
                await Task.Delay(100);
            }

            Scene activeScene = SceneManager.GetSceneByName(m_ActiveSceneGroup.FindSceneNameByType(SceneType.ActiveScene));

            if (activeScene.IsValid())  
            {
                SceneManager.SetActiveScene(activeScene);
            }



            OnSceneGroupLoaded.Invoke();
        }

        public async Task UnloadScenesAsync()
        {
            Scene BootstrapperScene = SceneManager.GetSceneByName(SceneGroupTheme.k_Bootstrapper);

            List<string> unloadScenes = new();
            string activeScene = SceneManager.GetActiveScene().name;
            int sceneCount = SceneManager.sceneCount;

            for (int i = 0; i < sceneCount; i++)
            {
                var sceneAt = SceneManager.GetSceneAt(i);
                if (!sceneAt.isLoaded) continue;

                var sceneName = sceneAt.name;
                if (sceneName == SceneGroupTheme.k_Bootstrapper) continue;

                unloadScenes.Add(sceneName);
            }

            AsyncOperationGroup operationGroup = new(unloadScenes.Count);

            // 씬 언로딩
            foreach (var scene in unloadScenes)
            {
                AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
                if (operation == null) continue;

                operationGroup.Operations.Add(operation);

                OnSceneUnloaded.Invoke(scene);
            }

            // 작업이 끝날때까지 대기
            while (!operationGroup.IsDone)
            {
                await Task.Delay(100); // delay to avoid tight loop
            }

            await Resources.UnloadUnusedAssets();
        }
    }


    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> Operations;

        public float Progress => Operations.Count == 0 ? 0 : Operations.Average(o => o.progress);
        public bool IsDone => Operations.All(o => o.isDone);

        public AsyncOperationGroup(int initialCapcity)
        {
            Operations = new List<AsyncOperation>(initialCapcity);
        }
    }

    public readonly struct AsyncOperationHandleGroup
    {
        public readonly List<AsyncOperationHandle<SceneInstance>> Handles;

        public float Progress => Handles.Count == 0 ? 0 : Handles.Average(h => h.PercentComplete);
        public bool IsDone => Handles.Count == 0 || Handles.All(o => o.IsDone);

        public AsyncOperationHandleGroup(int initialCapacity)
        {
            Handles = new List<AsyncOperationHandle<SceneInstance>>(initialCapacity);
        }
    }
}
