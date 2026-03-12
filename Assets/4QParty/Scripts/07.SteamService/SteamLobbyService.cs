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
        public ulong LobbyId;
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
                    LobbyId = callback.m_ulSteamIDLobby,
                    MaxPlayers = m_SteamSettings.MaxPlayer
                };

                CSteamID lobbyID = new CSteamID(data.LobbyId);
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

            // 로비 목록 요청 결과에 대한 콜백 핸들러
            Callback<LobbyMatchList_t> matchListCallback = null;
            matchListCallback = Callback<LobbyMatchList_t>.Create(callback =>
            {
                List<LobbyData> lobbies = new List<LobbyData>();

                // 반환된 로비 개수만큼 반복하며 데이터 추출
                for (int i = 0; i < callback.m_nLobbiesMatching; i++)
                {
                    CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);

                    lobbies.Add(new LobbyData
                    {
                        IsSuccess = true,
                        LobbyId = lobbyID.m_SteamID,
                        LobbyName = SteamMatchmaking.GetLobbyData(lobbyID, k_LobbyNameKey),
                        MaxPlayers = SteamMatchmaking.GetLobbyMemberLimit(lobbyID),
                        CurrentPlayers = SteamMatchmaking.GetNumLobbyMembers(lobbyID),
                        IsPrivate = false // 목록에 나오는 것은 보통 Public/FriendsOnly 입니다.
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

        public void LeaveLobby(ulong lobbyID)
        {
            SteamMatchmaking.LeaveLobby(new CSteamID(lobbyID));
        }

    }

}