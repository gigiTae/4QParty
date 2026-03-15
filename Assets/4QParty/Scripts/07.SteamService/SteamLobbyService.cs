using NUnit.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace FQParty.SteamService
{
    public class SteamLobbyService
    {
        SteamSettingsSO m_SteamSettings;

        public SteamLobby CurrentLobby
        {
            get => m_CurrentLobby;
        }
        SteamLobby m_CurrentLobby;

        public SteamLobbyService(SteamSettingsSO steamSettings)
        {
            m_SteamSettings = steamSettings;
        }

        public void LeaveLobby()
        {
            if(m_CurrentLobby !=null)
            {
                m_CurrentLobby.Dispose();
                m_CurrentLobby = null;
                Debug.Log($"{nameof(SteamLobbyService)} : LeaveLobby");
            }
        }

        public async Task<SteamLobby> CreateLobbyAsync(string lobbyName, bool isPrivate)
        {
            var tcs = new TaskCompletionSource<SteamLobby>();
            Callback<LobbyCreated_t> createdCallback = null;

            createdCallback = Callback<LobbyCreated_t>.Create(callback =>
            {
                if (callback.m_eResult != EResult.k_EResultOK)
                {
                    Debug.LogError($"로비 생성 실패! 에러 코드: {callback.m_eResult}");

                    createdCallback.Dispose();
                    tcs.SetResult(null); 
                    return;
                }

                CSteamID lobbyID = new CSteamID(callback.m_ulSteamIDLobby);
                SteamMatchmaking.SetLobbyData(lobbyID, SteamConstant.k_LobbyNameKey, lobbyName);
                SteamMatchmaking.SetLobbyData(lobbyID, SteamConstant.k_GameNameKey, SteamConstant.k_GameName);

                m_CurrentLobby = new SteamLobby(lobbyID);

                createdCallback.Dispose();
                tcs.SetResult(m_CurrentLobby);
            });

            ELobbyType lobbyType = isPrivate ? ELobbyType.k_ELobbyTypePrivate : ELobbyType.k_ELobbyTypePublic;
            SteamMatchmaking.CreateLobby(lobbyType, m_SteamSettings.MaxPlayer);

            return await tcs.Task;
        }

        public async Task<List<SteamLobbyData>> GetLobbyListAsync()
        {
            var tcs = new TaskCompletionSource<List<SteamLobbyData>>();

            Callback<LobbyMatchList_t> matchListCallback = null;
            matchListCallback = Callback<LobbyMatchList_t>.Create((Callback<LobbyMatchList_t>.DispatchDelegate)(callback =>
            {
                List<SteamLobbyData> lobbies = new List<SteamLobbyData>();

                for (int i = 0; i < callback.m_nLobbiesMatching; i++)
                {
                    CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);

                    lobbies.Add(new SteamLobbyData
                    {
                        LobbyID = lobbyID.m_SteamID,
                        LobbyName = SteamMatchmaking.GetLobbyData(lobbyID, SteamConstant.k_LobbyNameKey),
                        MaxPlayers = SteamMatchmaking.GetLobbyMemberLimit(lobbyID),
                        CurrentPlayers = SteamMatchmaking.GetNumLobbyMembers(lobbyID),
                    });
                }
                Debug.Log($"Lobby list retrieved: {lobbies.Count} lobbies found.");

                matchListCallback.Dispose(); 
                tcs.SetResult(lobbies);
            }));

            SteamMatchmaking.AddRequestLobbyListStringFilter(
                SteamConstant.k_GameNameKey,
                SteamConstant.k_GameName,
                ELobbyComparison.k_ELobbyComparisonEqual);

            SteamMatchmaking.RequestLobbyList();

            return await tcs.Task;
        }

        public async Task<SteamLobby> JoinLobbyAsync(ulong lobbyID)
        {
            var tcs = new TaskCompletionSource<SteamLobby>();
            CSteamID steamLobbyID = new CSteamID(lobbyID);

            Callback<LobbyEnter_t> enterCallback = null;
            enterCallback = Callback<LobbyEnter_t>.Create((Callback<LobbyEnter_t>.DispatchDelegate)(callback =>
            {
                if (callback.m_ulSteamIDLobby != lobbyID)
                {
                    tcs.SetResult(null);
                }

                bool success = (EChatRoomEnterResponse)callback.m_EChatRoomEnterResponse ==
                EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess;

                if (!success)
                {
                    Debug.LogError($"Failed to join lobby. Response code: {callback.m_rgfChatPermissions}");
                    tcs.SetResult(null);
                }

                m_CurrentLobby = new SteamLobby(steamLobbyID);

                enterCallback.Dispose();
                tcs.SetResult(m_CurrentLobby);
            }));

            SteamMatchmaking.JoinLobby(steamLobbyID);

            return await tcs.Task;
        }

    }
}