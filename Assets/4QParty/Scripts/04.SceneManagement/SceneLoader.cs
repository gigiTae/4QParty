using FQParty.Common.Persistance;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FQParty.SceneManagement
{
    public class SceneLoader : PersistanceNetworkSingleton<SceneLoader>
    {
        [SerializeField] LoadingScreen m_LoadingScreen;
        bool IsNetworkSceneManagementEnabled => NetworkManager != null && NetworkManager.SceneManager != null && NetworkManager.NetworkConfig.EnableSceneManagement;
        bool m_IsInitialized;

        protected override void Awake()
        {
            base.Awake();

        }

        public virtual void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            NetworkManager.OnServerStarted += OnNetworkingSessionStarted;
            NetworkManager.OnClientStarted += OnNetworkingSessionStarted;
            NetworkManager.OnServerStopped -= OnNetworkingSessionEnded;
            NetworkManager.OnClientStopped -= OnNetworkingSessionEnded;
        }

        public override void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (NetworkManager != null)
            {
                NetworkManager.OnServerStarted -= OnNetworkingSessionStarted;
                NetworkManager.OnClientStarted -= OnNetworkingSessionStarted;
                NetworkManager.OnServerStopped -= OnNetworkingSessionEnded;
                NetworkManager.OnClientStopped -= OnNetworkingSessionEnded;
            }
            base.OnDestroy();
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (!IsSpawned || NetworkManager.ShutdownInProgress)
            {
                m_LoadingScreen.EnableLoadingScreen();
            }
        }

        void OnNetworkingSessionStarted()
        {
            if (!m_IsInitialized)
            {
                if (IsNetworkSceneManagementEnabled)
                {
                    NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
                }
                m_IsInitialized = true;
            }
        }

        void OnNetworkingSessionEnded(bool unused)
        {
            if (m_IsInitialized)
            {
                if (IsNetworkSceneManagementEnabled)
                {
                    NetworkManager.SceneManager.OnSceneEvent -= OnSceneEvent;
                }

                m_IsInitialized = false;
            }
        }

        public virtual void LoadScene(string sceneName, bool useNetworkSceneManager, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (useNetworkSceneManager)
            {
                if (IsSpawned && !NetworkManager.ShutdownInProgress)
                {
                    NetworkManager.SceneManager.LoadScene(sceneName, loadSceneMode);
                }
            }
            else
            {
                var loadOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
                if (loadSceneMode == LoadSceneMode.Single)
                {
                    m_LoadingScreen.EnableLoadingScreen(true);
                }
            }
        }


        void UnloadAdditiveScenes()
        {
            var activeScene = SceneManager.GetActiveScene();
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded && scene != activeScene)
                {
                    SceneManager.UnloadSceneAsync(scene);
                }
            }
        }

        void OnSceneEvent(SceneEvent sceneEvent)
        {
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.Load: // 서버가 클라이언트에게 씬을 로드하라고 명령함
                                          // 클라이언트 또는 호스트에서만 실행됨
                    if (NetworkManager.IsClient)
                    {
                        // 싱글(Single) 모드로 로드하는 경우에만 새로운 로딩 화면을 표시, 그 외에는 정보만 업데이트
                        if (sceneEvent.LoadSceneMode == LoadSceneMode.Single)
                        {
                            m_LoadingScreen.EnableLoadingScreen(true);
                            //m_LoadingProgressManager.LocalLoadOperation = sceneEvent.AsyncOperation;
                        }
                        else
                        {
                            // 추가(Additive) 로드 시 로딩 화면 텍스트 등을 업데이트하거나 진행률 관리
                            //m_ClientLoadingScreen.UpdateLoadingScreen(sceneEvent.SceneName);
                            //m_LoadingProgressManager.LocalLoadOperation = sceneEvent.AsyncOperation;
                        }
                    }
                    break;

                case SceneEventType.LoadEventCompleted: // 모든 대상의 씬 로드가 완료됨
                    if (NetworkManager.IsClient)
                    {
                        // 로딩 화면 종료
                        m_LoadingScreen.EnableLoadingScreen(false);
                    }
                    break;

                case SceneEventType.Synchronize: // 서버가 클라이언트에게 씬 동기화를 시작하라고 명령함
                    {
                        // 호스트가 아닌 일반 클라이언트에서만 실행됨
                        if (NetworkManager.IsClient && !NetworkManager.IsHost)
                        {
                            if (NetworkManager.SceneManager.ClientSynchronizationMode == LoadSceneMode.Single)
                            {
                                // 클라이언트 동기화 모드가 Single인 경우, 현재 로드된 모든 추가(Additive) 씬을 언로드함.
                                // 이 경우 클라이언트는 서버와 동일한 씬만 유지하게 됨.
                                // NGO(Netcode For GameObjects)는 동기화 과정에서 서버에 로드된 모든 씬을 클라이언트에게 자동으로 로드해줌.
                                // 만약 서버의 메인 씬이 클라이언트와 다르다면 Single 모드로 로드하며 모든 추가 씬을 자동으로 언로드하지만,
                                // 메인 씬이 같다면 추가 씬들이 자동으로 언로드되지 않으므로 여기서 수동으로 처리함.
                                UnloadAdditiveScenes();
                            }
                        }
                        break;
                    }

                case SceneEventType.SynchronizeComplete: // 클라이언트가 서버에 동기화를 마쳤다고 보고함
                                                         // 서버에서만 실행됨
                    if (NetworkManager.IsServer)
                    {
                        // 서버가 동기화 완료 후 필요한 처리(예: 캐릭터 스폰 등)를 마칠 때까지 로딩 화면을 유지해야 하므로,
                        // 모든 처리가 끝난 뒤 서버에서 클라이언트에게 RPC를 보내 로딩 화면을 수동으로 끄게 함.
                        ClientStopLoadingScreenRpc(RpcTarget.Group(new[] { sceneEvent.ClientId }, RpcTargetUse.Temp));
                    }
                    break;
            }
        }
        [Rpc(SendTo.SpecifiedInParams)]
        void ClientStopLoadingScreenRpc(RpcParams clientRpcParams = default)
        {
            m_LoadingScreen.EnableLoadingScreen(false);
        }
    }

}