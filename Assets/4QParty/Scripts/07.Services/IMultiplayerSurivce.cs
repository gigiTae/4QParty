using FQParty.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace FQParty.Services
{

    public interface IMultiplayLobbyService 
    {
        string CurrentLobbyId { get; }    

        // ==========================================
        // 로비 관리 (Lobby Management)
        // ==========================================
        /// <summary> 로비 생성 (Steam Lobby Type 설정 포함) </summary>
        void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate = false);

        /// <summary> 특정 로비 ID로 직접 참여 </summary>
        void JoinLobby(string lobbyId);

        /// <summary> 친구 목록 창을 띄워 초대하거나, Steam 친구 ID로 직접 초대 </summary>
        //void InviteFriend(string friendSteamId);

        /// <summary> 현재 로비에서 퇴장 </summary>
        void LeaveLobby();

        /// <summary> 로비 리스트 검색 (Steam Matchmaking) </summary>
        void FindLobbies();

        // ==========================================
        // 이벤트 콜백 (Events)
        // ==========================================
        // 로비 생성 및 입장
        event Action<bool, string> OnLobbyCreated;      // 성공여부, 생성된 LobbyID
        event Action<bool, string> OnLobbyJoined;       // 성공여부, 로비 ID
        event Action<List<LobbyInfo>> OnLobbyListFetched;
    }

}