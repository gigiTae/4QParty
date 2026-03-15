using Eflatun.SceneReference;
using FQParty.Common.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
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

        public void OnNetworkSpawn(NetworkSceneManager manager)
        {
            m_NetworkSceneManager = manager;
            m_NetworkSceneManager.OnSceneEvent += OnSceneEvent;
        }
        
        public void OnNetworkDespawn()
        {
            m_NetworkSceneManager.OnSceneEvent -= OnSceneEvent;
            m_NetworkSceneManager = null;
        }

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

        public async Task LoadSceneGroupAsServerAsync(SceneGroup group, bool reloadDupScenes = false)
        {
            m_ActiveSceneGroup = group;

            await UnloadSceneAsServerAsync();

            var loadedScenes = new List<string>();
            int currentSceneCount = SceneManager.sceneCount;
            for (int i = 0; i < currentSceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }

            foreach (var sceneData in m_ActiveSceneGroup.Scenes)
            {
                if (!reloadDupScenes && loadedScenes.Contains(sceneData.Name)) continue;

                var tcs = new TaskCompletionSource<bool>();
                NetworkSceneManager.SceneEventDelegate handler = null;

                handler = (sceneEvent) =>
                {
                    if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted &&
                        sceneEvent.SceneName == sceneData.Name)
                    {
                        m_NetworkSceneManager.OnSceneEvent -= handler;
                        tcs.TrySetResult(true);
                    }
                    else
                    {
                        tcs.TrySetResult(false);
                    }
                };

                m_NetworkSceneManager.OnSceneEvent += handler;

                // NGO 네트워크 씬 로드 명령 (Additive 모드로 쌓아감)
                var status = m_NetworkSceneManager.LoadScene(sceneData.Name, LoadSceneMode.Additive);

                if (status != SceneEventProgressStatus.Started)
                {
                    Debug.LogError($"[SceneLoader] {sceneData.Name} 로드 실패: {status}");
                    m_NetworkSceneManager.OnSceneEvent -= handler;
                    tcs.TrySetResult(false);
                }
                else
                {
                    await tcs.Task; // 모든 클라이언트가 이 씬을 다 불러올 때까지 대기
                    OnSceneLoaded?.Invoke(sceneData.Name);
                }
            }
            
            string activeSceneName = m_ActiveSceneGroup.FindSceneNameByType(SceneType.ActiveScene);
            Scene activeScene = SceneManager.GetSceneByName(activeSceneName);

            if (activeScene.IsValid())
            {
                SceneManager.SetActiveScene(activeScene);
            }

            OnSceneGroupLoaded?.Invoke();
        }

        public async Task UnloadSceneAsServerAsync()
        {
            List<string> unloadScenes = new();
            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;

            for (int i = 0; i < sceneCount; i++)
            {
                var sceneAt = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (!sceneAt.isLoaded) continue;

                var sceneName = sceneAt.name;
                if (sceneName == SceneGroupTheme.k_Bootstrapper) continue;

                unloadScenes.Add(sceneName);
            }

            foreach (var sceneName in unloadScenes)
            {
                var tcs = new TaskCompletionSource<bool>();

                NetworkSceneManager.SceneEventDelegate handler = null;
                handler = (sceneEvent) =>
                {
                    if (sceneEvent.SceneEventType == SceneEventType.UnloadEventCompleted &&
                        sceneEvent.SceneName == sceneName)
                    {
                        m_NetworkSceneManager.OnSceneEvent -= handler;
                        tcs.TrySetResult(true);
                    }
                    else
                    {
                        tcs.TrySetResult(false);
                    }
                };

                m_NetworkSceneManager.OnSceneEvent += handler;

                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
                var status = m_NetworkSceneManager.UnloadScene(scene);

                if (status != SceneEventProgressStatus.Started)
                {
                    m_NetworkSceneManager.OnSceneEvent -= handler;
                    tcs.TrySetResult(false);
                }
                else
                {
                    await tcs.Task; 
                }
            }

            await Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        public async Task UnloadScenesAsync()
        {
            List<string> unloadScenes = new();
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

        void OnSceneEvent(SceneEvent sceneEvent)
        {
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.Load: // Server told client to load a scene
                    break;
                case SceneEventType.LoadEventCompleted: // Server told client that all clients finished loading a scene
                    break;
                case SceneEventType.Synchronize: // Server told client to start synchronizing scenes
                        break;
            }
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
