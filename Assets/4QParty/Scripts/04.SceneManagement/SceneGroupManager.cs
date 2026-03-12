using Eflatun.SceneReference;
using FQParty.Common.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        // Addressable로 로드한 씬 그룹
        readonly AsyncOperationHandleGroup m_HandleLoadGroup = new AsyncOperationHandleGroup(10);
        readonly AsyncOperationHandleGroup m_HandleUnloadGroup = new AsyncOperationHandleGroup(10);

        SceneGroup m_ActiveSceneGroup;

        public async Task LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDupScenes = false)
        {
            m_ActiveSceneGroup = group;
            var loadedScenes = new List<string>();

            await UnloadScenes();

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
                else if (sceneData.Reference.State == SceneReferenceState.Addressable)
                {
                    var sceneHandle = Addressables.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);
                    m_HandleLoadGroup.Handles.Add(sceneHandle);
                }

                OnSceneLoaded.Invoke(sceneData.Name);
            }

            while (!operationGroup.IsDone || !m_HandleLoadGroup.IsDone)
            {
                progress?.Report((operationGroup.Progress + m_HandleLoadGroup.Progress) / 2);
                await Task.Delay(100);
            }

            Scene activeScene = SceneManager.GetSceneByName(m_ActiveSceneGroup.FindSceneNameByType(SceneType.ActiveScene));

            if (activeScene.IsValid())
            {
                SceneManager.SetActiveScene(activeScene);
            }

            OnSceneGroupLoaded.Invoke();
        }

        public async Task UnloadScenes()
        {
            Scene BootstrapperScene = SceneManager.GetSceneByName(SceneGroupTheme.k_Bootstrapper);
            if (BootstrapperScene.IsValid())
            {
                SceneManager.SetActiveScene(BootstrapperScene);
            }

            m_HandleUnloadGroup.Handles.Clear();

            List<string> unloadScenes = new();
            string activeScene = SceneManager.GetActiveScene().name;
            int sceneCount = SceneManager.sceneCount;

            // 언로드 (어드레서블 씬, 부트트랩씬 예외) 씬리스트 추가
            for (int i = 0; i < sceneCount; i++)
            {
                var sceneAt = SceneManager.GetSceneAt(i);
                if (!sceneAt.isLoaded) continue;

                var sceneName = sceneAt.name;
                if (sceneName.Equals(activeScene) || sceneName == SceneGroupTheme.k_Bootstrapper) continue;
                if (m_HandleLoadGroup.Handles.Any(h => h.IsValid() && h.Result.Scene.name == sceneName)) continue;

                unloadScenes.Add(sceneName);
            }

            AsyncOperationGroup operationGroup = new(unloadScenes.Count);

            // 일반 씬 언로딩 처리
            foreach (var scene in unloadScenes)
            {
                AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
                if (operation == null) continue;

                operationGroup.Operations.Add(operation);

                OnSceneUnloaded.Invoke(scene);
            }


            // 어드레서블 씬 언로드 처리
            foreach (var handle in m_HandleLoadGroup.Handles)
            {
                if (handle.IsValid())
                {
                    AsyncOperationHandle<SceneInstance> unloadHandle = Addressables.UnloadSceneAsync(handle);
                    m_HandleUnloadGroup.Handles.Add(unloadHandle);
                }
            }
            m_HandleLoadGroup.Handles.Clear();

            // 작업이 끝날때까지 대기
            while (!operationGroup.IsDone || !m_HandleUnloadGroup.IsDone)
            {
                await Task.Delay(100); // delay to avoid tight loop
            }

            m_HandleUnloadGroup.Handles.Clear();

            // Optional: UnloadUnusedAssets - unloads all unused assets from memory
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
