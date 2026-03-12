using NUnit.Framework;
using Steamworks;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace FQParty.SteamService
{
    public struct LobbyData
    {
        public bool IsSuccess;
        public bool IsPrivate;
        public string LobbyName;
        public ulong LobbyID;
        public ulong HostID;
        public int MaxPlayers;
        public int CurrentPlayers;
    }

    public class SteamLobbyService
    {
        const string k_LobbyNameKey = "LobbyName";
        const string k_GameNameKey = "GameName";
        const string k_GameName = "4QParty";

        SteamSettingsSO m_SteamSettings;

        public SteamLobbyService(SteamSettingsSO steamSettings)
        {
            m_SteamSettings = steamSettings;
        }

        public async Task<LobbyData> CreateLobby(string lobbyName, bool isPrivate)
        {
            var tcs = new TaskCompletionSource<LobbyData>();
            Callback<LobbyCreated_t> createdCallback = null;
            createdCallback = Callback<LobbyCreated_t>.Create(callback =>
            {
                LobbyData data = new()
                {
                    IsPrivate = isPrivate,
                    LobbyName = lobbyName,
                    IsSuccess = callback.m_eResult == EResult.k_EResultOK,
                    LobbyID = callback.m_ulSteamIDLobby,
                    MaxPlayers = m_SteamSettings.MaxPlayer
                };

                CSteamID lobbyID = new CSteamID(data.LobbyID);
                SteamMatchmaking.SetLobbyData(lobbyID, k_LobbyNameKey, lobbyName);
                SteamMatchmaking.SetLobbyData(lobbyID, k_GameNameKey, k_GameName);

                Debug.Log("CreateLobby");

                createdCallback.Dispose(); // 사용 후 해제
                
                tcs.SetResult(data);
            });

            // 로비 생성 시작
            SteamMatchmaking.CreateLobby(isPrivate ? ELobbyType.k_ELobbyTypePrivate : ELobbyType.k_ELobbyTypePublic, m_SteamSettings.MaxPlayer);

            // 결과를 받을 때까지 여기서 대기 (비차단)
            return await tcs.Task;
        }

        public async Task<List<LobbyData>> GetLobbyList()
        {
            var tcs = new TaskCompletionSource<List<LobbyData>>();

            Callback<LobbyMatchList_t> matchListCallback = null;
            matchListCallback = Callback<LobbyMatchList_t>.Create(callback =>
            {
                List<LobbyData> lobbies = new List<LobbyData>();

                for (int i = 0; i < callback.m_nLobbiesMatching; i++)
                {
                    CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);

                    lobbies.Add(new LobbyData
                    {
                        IsSuccess = true,
                        LobbyID = lobbyID.m_SteamID,
                        LobbyName = SteamMatchmaking.GetLobbyData(lobbyID, k_LobbyNameKey),
                        MaxPlayers = SteamMatchmaking.GetLobbyMemberLimit(lobbyID),
                        CurrentPlayers = SteamMatchmaking.GetNumLobbyMembers(lobbyID),
                        IsPrivate = false 
                    });
                }

                Debug.Log($"Lobby list retrieved: {lobbies.Count} lobbies found.");

                matchListCallback.Dispose(); // 콜백 해제
                tcs.SetResult(lobbies);
            });

            SteamMatchmaking.AddRequestLobbyListStringFilter(k_GameNameKey, k_GameName, ELobbyComparison.k_ELobbyComparisonEqual);

            // 로비 목록 요청 시작
            SteamMatchmaking.RequestLobbyList();

            return await tcs.Task;
        }

        public async Task<LobbyData> JoinLobby(ulong lobbyID)
        {
            var tcs = new TaskCompletionSource<LobbyData>();
            CSteamID steamLobbyID = new CSteamID(lobbyID);

            Callback<LobbyEnter_t> enterCallback = null;
            enterCallback = Callback<LobbyEnter_t>.Create(callback =>
            {
                // 콜백으로 받은 ID가 내가 요청한 로비 ID와 일치하는지 확인
                if (callback.m_ulSteamIDLobby != lobbyID) return;

                bool success = (EChatRoomEnterResponse)callback.m_rgfChatPermissions == EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess;

                LobbyData data = new LobbyData
                {
                    IsSuccess = success,
                    LobbyID = callback.m_ulSteamIDLobby,
                    LobbyName = SteamMatchmaking.GetLobbyData(steamLobbyID, k_LobbyNameKey),
                    MaxPlayers = SteamMatchmaking.GetLobbyMemberLimit(steamLobbyID),
                    CurrentPlayers = SteamMatchmaking.GetNumLobbyMembers(steamLobbyID),
                    HostID = SteamMatchmaking.GetLobbyOwner(steamLobbyID).m_SteamID,
                    IsPrivate = false 
                };

                if (success)
                {
                    Debug.Log($"Successfully joined lobby: {data.LobbyName}");
                }
                else
                {
                    Debug.LogError($"Failed to join lobby. Response code: {callback.m_rgfChatPermissions}");
                }

                enterCallback.Dispose(); // 콜백 해제
                tcs.SetResult(data);
            });

            // 로비 참가 요청
            SteamMatchmaking.JoinLobby(steamLobbyID);

            return await tcs.Task;
        }

        public void LeaveLobby(ulong lobbyID)
        {
            SteamMatchmaking.LeaveLobby(new CSteamID(lobbyID));
        }

    }

}