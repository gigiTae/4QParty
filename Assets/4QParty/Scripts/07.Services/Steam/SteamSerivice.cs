using FQParty.Common.Persistance;
using FQParty.Services;
using FQParty.Services.Steam;
using Steamworks;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace FQParty.Services
{
    /// <summary>
    /// 스팀과 관련된 기능을 처리하는 매니저
    /// </summary>
    public class SteamLobbyService : MonoBehaviour, IMultiplayLobbyService
    {
        [SerializeField] private SteamSettingsSO m_Settings;
        NetworkManager m_NetworkManager;

        private bool m_IsInitialized;

        private void Awake()
        {
            Initialize();
        }

        void Initialize()
        {
            try
            {
                /// 게임이 스팀을 통해서 정상적으로 실행되었는지 확인
                if (SteamAPI.RestartAppIfNecessary((AppId_t)480)) // 480은 테스트용 AppID
                {
                    Application.Quit();
                    return;
                }
            }
            catch (System.DllNotFoundException e)
            {
                Debug.LogError("[Steamworks.NET] Steam dll을 찾을 수 없습니다. " + e);
                return;
            }

            m_IsInitialized = SteamAPI.Init();
            if (!m_IsInitialized)
            {
                Debug.LogError("[Steamworks.NET] 스팀 초기화 실패! (스팀이 꺼져있거나 AppID 설정 오류)");
            }
            else
            {
                Debug.Log("[Steamworks.NET] 스팀 연결 성공!");
            }
        }

        public void Update()
        {
            if (!m_IsInitialized) return;

            SteamAPI.RunCallbacks();
        }

        public string CurrentLobbyId { get; }

        public void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate = false)
        {
            ELobbyType type = isPrivate ? ELobbyType.k_ELobbyTypePrivate : ELobbyType.k_ELobbyTypePublic;

            SteamMatchmaking.CreateLobby(type, maxPlayers);
        }

        public void JoinLobby(string lobbyId)
        {

        }

        public void LeaveLobby()
        {
        }

        public void FindLobbies()
        {

        }
        public int ConnectedPlayerCount { get; } // 현재 접속된 플레이어 수

        public event Action<bool, string> OnLobbyCreated;      // 성공여부, 생성된 LobbyID
        public event Action<bool, string> OnLobbyJoined;       // 성공여부, 로비 ID
        public event Action<List<LobbyInfo>> OnLobbyListFetched;

        
    }

}

