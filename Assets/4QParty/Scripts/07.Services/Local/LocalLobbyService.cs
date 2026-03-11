using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;


namespace FQParty.Services.Local
{
    public class LocalLobbyService : IMultiplayLobbyService
    {
        public LocalLobbyService(NetworkManager manager)
        {
            m_NetworkManager = manager;
        }

        NetworkManager m_NetworkManager;

        public int ConnectedPlayerCount { get; } = 1; // 현재 접속된 플레이어 수
        public string CurrentLobbyId { get; }    // 현재 참여 중인 Steam 로비 ID

        public void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate = false) { }
        public void JoinLobby(string lobbyId) { }
        public void LeaveLobby() { }
        public void FindLobbies() { }

        public event Action<bool, string> OnLobbyCreated;      // 성공여부, 생성된 LobbyID
        public event Action<bool, string> OnLobbyJoined;       // 성공여부, 로비 ID
        public event Action<List<LobbyInfo>> OnLobbyListFetched;

    }
}