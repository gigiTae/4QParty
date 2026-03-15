using Steamworks;
using System;
using System.Collections.Generic;
using Unity.Multiplayer.PlayMode;
using UnityEngine;


namespace FQParty.SteamService
{
    public struct SteamPlayerData
    {
        public string Name;
        public ulong SteamID;
    }

    public struct SteamLobbyData
    {
        public string LobbyName;
        public ulong LobbyID;
        public ulong HostID;
        public int MaxPlayers;
        public int CurrentPlayers;
        public List<SteamPlayerData> PlayerDataList;
    }

    public class SteamLobby : IDisposable
    {
        public SteamLobbyData LobbyData
        {
            get => m_LobbyData;
        }

        SteamLobbyData m_LobbyData;

        public SteamLobby(CSteamID lobbyID)
        {
            m_LobbyData.LobbyID = lobbyID.m_SteamID;
            m_LobbyData.LobbyName = SteamMatchmaking.GetLobbyData(lobbyID, SteamConstant.k_LobbyNameKey);
            m_LobbyData.MaxPlayers = SteamMatchmaking.GetLobbyMemberLimit(lobbyID);
            m_LobbyData.CurrentPlayers = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
            m_LobbyData.HostID = SteamMatchmaking.GetLobbyOwner(lobbyID).m_SteamID;
            m_LobbyData.PlayerDataList = GetCurrentLobbyMembers(lobbyID);   

            RegisterCallback();
        }


        public event Action UpdateLobbyEvent;
        Callback<LobbyChatUpdate_t> m_LobbyChatUpdate;

        public void Dispose()
        {
            m_LobbyChatUpdate.Dispose();
            m_LobbyChatUpdate = null;

            CSteamID lobbyID = new(m_LobbyData.LobbyID);
            SteamMatchmaking.LeaveLobby(lobbyID);
        }

        void RegisterCallback()
        {
            m_LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        }

        void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
        {
            Debug.Log("Lobby Update");

            if (callback.m_ulSteamIDLobby == m_LobbyData.LobbyID)
            {
                CSteamID lobbyID = new CSteamID(LobbyData.LobbyID);

                m_LobbyData.CurrentPlayers = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
                m_LobbyData.PlayerDataList = GetCurrentLobbyMembers(lobbyID);
            }

            UpdateLobbyEvent.Invoke();
        }

        List<SteamPlayerData> GetCurrentLobbyMembers(CSteamID lobbyID)
        {
            List<SteamPlayerData> list = new List<SteamPlayerData>();

            int numMembers = SteamMatchmaking.GetNumLobbyMembers(lobbyID);

            Debug.Log($"LobbyPlayerCount : {numMembers}");

            for (int i = 0; i < numMembers; i++)
            {
                CSteamID memberID = SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i);

                string playerName = SteamFriends.GetFriendPersonaName(memberID);

                list.Add(new SteamPlayerData
                {
                    Name = string.IsNullOrEmpty(playerName) ? "Unknown" : playerName
                });
            }
            return list;
        }
    }
}
