using FQParty.Common.Persistance;
using Steamworks;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace FQParty.SteamService
{
    /// <summary>
    /// 스팀과 관련된 기능을 처리하는 매니저
    /// </summary>
    public class SteamManager : PersistanceSingleton<SteamManager>
    {
        [SerializeField] private SteamSettingsSO m_Settings;
        [SerializeField] NetworkManager m_NetworkManager;


        public SteamLobbyService SteamLobbyService
        {
            get => m_SteamLobbyService;
        }

        private SteamLobbyService m_SteamLobbyService;

        public bool IsInitialized { get => m_IsInitialized; }    
        private bool m_IsInitialized;


        protected override void Awake()
        {
            m_SteamLobbyService = new(m_Settings);

            base.Awake();
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
            SteamAPI.RunCallbacks();
        }


    }

}

