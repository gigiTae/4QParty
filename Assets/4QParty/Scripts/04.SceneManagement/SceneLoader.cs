using FQParty.Common.Persistance;
using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace FQParty.SceneManagement
{
    public class SceneLoader : PersistanceNetworkSingleton<SceneLoader>
    {
        [SerializeField] LoadSceneGroupEvent m_LoadSceneGroupEvent;
        [SerializeField] SceneGroupListSO m_SceneGroupList;
        [SerializeField] LoadingScreen m_LoadingScreen;

        bool m_IsLoading;

        public readonly SceneGroupManager m_SceneGroupManager = new SceneGroupManager();

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            m_SceneGroupManager.OnNetworkSpawn(NetworkManager.SceneManager);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            m_SceneGroupManager.OnNetworkDespawn();
        }

        protected override void Awake()
        {
            base.Awake();
            m_LoadSceneGroupEvent.Subscribe(OnLoadSceneGroup);

            m_SceneGroupManager.OnSceneLoaded += sceneName => Debug.Log("Loaded: " + sceneName);
            m_SceneGroupManager.OnSceneUnloaded += sceneName => Debug.Log("Unloaded: " + sceneName);
        }

        void OnLoadSceneGroup(LoadSceneGroupContext context)
        {
            LoadSceneGroup(context);
        }

        public void LoadSceneGroup(LoadSceneGroupContext context)
        {
            if (m_IsLoading)
            {
                Debug.LogWarning("현재 다른 씬그룹을 로딩하고 있습니다");
                return;
            }

            var index = m_SceneGroupList.SceneGroups.FindIndex(g => g.GroupName == context.GroupName);

            if (index == -1) 
            {
                Debug.LogWarning($"{context.GroupName}의 씬 그룹을 찾을 수 없습니다");
                return;
            }


            if(context.UseNetworkSceneManager)
            {
                _ = LoadSceneGroupAsHost(index);
            }
            else 
            {
                _ = LoadSceneGroup(index);
            }
        }

        async void Start()
        {
            await LoadSceneGroup(0);
        }

        private async Task LoadSceneGroupAsHost(int index)
        {
            if (!NetworkManager.IsServer) return;

            EnableLoadingScreenClientRpc(true);
            await m_SceneGroupManager.LoadSceneGroupAsync(m_SceneGroupList.SceneGroups[index]);
            EnableLoadingScreenClientRpc(false);
        }

        private async Task LoadSceneGroup(int index)
        {
            EnableLoadingScreen(true);
            await m_SceneGroupManager.LoadSceneGroupAsync(m_SceneGroupList.SceneGroups[index]);
            EnableLoadingScreen(false);
        }

        void EnableLoadingScreen(bool enable = true)
        {
            m_IsLoading = enable;
            m_LoadingScreen.EnableLoadingScreen(enable);
        }

        [ClientRpc]
        void EnableLoadingScreenClientRpc(bool enable = true)
        {
            m_IsLoading = enable;
            m_LoadingScreen.EnableLoadingScreen(enable);
        }
    }

}