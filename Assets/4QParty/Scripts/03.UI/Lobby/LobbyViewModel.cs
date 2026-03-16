using FQParty.Common.Constant;
using FQParty.ConnectionManagement;
using FQParty.SceneManagement;
using FQParty.SteamService;
using System;
using UnityEngine;
using UnityEngine.UIElements;


namespace FQParty.UI.Lobby
{
    public class LobbyViewModel : MonoBehaviour
    {
        [SerializeField]
        public UIDocument m_UIDocument;
        LobbyElement m_LobbyElement;
        SteamLobby m_SteamLobby;

        public void Awake()
        {
            if (m_UIDocument == null) return;

            m_LobbyElement = m_UIDocument.rootVisualElement.Q<LobbyElement>();

            if (m_LobbyElement != null)
            {
                m_LobbyElement.OnLeaveClicked += LeaveLobby;
                m_LobbyElement.OnStartGameClicked += StartGame;
            }

            if (SteamManager.Instance?.SteamLobbyService?.CurrentLobby != null)
            {
                m_SteamLobby = SteamManager.Instance.SteamLobbyService.CurrentLobby;
                m_SteamLobby.UpdateLobbyEvent += RefreshElement;
                RefreshElement();
            }
            else
            {
                Debug.Log("참여한 로비를 찾을 수 없습니다");
            }
        }
        private void OnDisable()
        {
            if (m_LobbyElement != null)
            {
                m_LobbyElement.OnLeaveClicked -= LeaveLobby;
                m_LobbyElement.OnStartGameClicked -= StartGame;
            }

            if (SteamManager.Instance?.SteamLobbyService != null)
            {
                m_SteamLobby.UpdateLobbyEvent -= RefreshElement;
            }
        }
        private void RefreshElement()
        {
            var lobbyData = m_SteamLobby.LobbyData;

            var playerDataList = lobbyData.PlayerDataList;

            if (playerDataList != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (playerDataList.Count > i)
                    {
                        m_LobbyElement.UpdatePlayerLabel(i, playerDataList[i].Name);
                    }
                    else
                    {
                        m_LobbyElement.UpdatePlayerLabel(i, "");
                    }
                }
            }
        }

        public void StartGame()
        {
            SceneLoader.Instance.LoadScene(SceneTheme.k_GamePlay, true);
        }

        public void LeaveLobby()
        {
            SteamManager.Instance.SteamLobbyService.LeaveLobby();
            ConnectionManager.Instance.RequestShutdown();
        }
    }

}