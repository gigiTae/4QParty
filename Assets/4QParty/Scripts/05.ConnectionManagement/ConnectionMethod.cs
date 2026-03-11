using Codice.Client.Common;
using Steamworks;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace FQParty.ConnectionManagement
{
    /// <summary>
    /// ConnectionMethod contains all setup needed to setup NGO to be ready to start a connection, either host or client
    /// side.
    /// Please override this abstract class to add a new transport or way of connecting.
    /// </summary>
    public abstract class ConnectionMethodBase
    {
        protected ConnectionManager m_ConnectionManager;

        public ConnectionMethodBase(ConnectionManager connectionManager)
        {
            m_ConnectionManager = connectionManager;
        }

        /// <summary>
        /// Setup the host connection prior to starting the NetworkManager
        /// </summary>
        /// <returns></returns>
        public abstract void SetupHostConnection();

        /// <summary>
        /// Setup the client connection prior to starting the NetworkManager
        /// </summary>
        /// <returns></returns>
        public abstract void SetupClientConnection();

        /// <summary>
        /// Setup the client for reconnection prior to reconnecting
        /// </summary>
        /// <returns>
        /// success = true if succeeded in setting up reconnection, false if failed.
        /// shouldTryAgain = true if we should try again after failing, false if not.
        /// </returns>
        public abstract Task<(bool success, bool shouldTryAgain)> SetupClientReconnectionAsync();

        protected abstract void SetConnectionPayload();

        protected abstract string GetPlayerId();
    }

    /// <summary>
    /// UTP's Relay connection setup using the Session integration
    /// </summary>
    public class ConnectionMethodSteam : ConnectionMethodBase
    {
        // Steam 접속에 필요한 추가 정보
        private CSteamID m_TargetSteamID; // 호스트의 Steam ID
        private CSteamID m_LobbyID;       // 참여 중인 로비 ID

        public ConnectionMethodSteam(CSteamID targetSteamID, CSteamID lobbyID,
            ConnectionManager connectionManager)
            : base(connectionManager)
        {
            m_TargetSteamID = targetSteamID;
            m_LobbyID = lobbyID;
        }

        protected override void SetConnectionPayload()
        {
            if (!SteamAPI.IsSteamRunning()) return;

            ConnectionPayload payload = new()
            {
                Id = SteamUser.GetSteamID().m_SteamID,
                PlayerName = SteamFriends.GetPersonaName(),
                IsDebug = Debug.isDebugBuild
            };

            string jsonPayload = JsonUtility.ToJson(payload);
            byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

            m_ConnectionManager.NetworkManager.NetworkConfig.ConnectionData = payloadBytes;
        }


        public override void SetupHostConnection()
        {
            SetConnectionPayload();
        }

        public override void SetupClientConnection()
        {
            SetConnectionPayload();
        }

        public override async Task<(bool success, bool shouldTryAgain)> SetupClientReconnectionAsync()
        {
            // 1. Steam 자체가 실행 중인지 확인
            if (!SteamAPI.IsSteamRunning())
            {
                return (false, false);
            }

            // 2. 로비가 여전히 유효한지 확인 (재접속 가능 여부 판단)
            // Steam 로비 데이터를 다시 읽어와서 호스트가 여전히 세션에 있는지 체크합니다.
            bool isLobbyValid = SteamMatchmaking.RequestLobbyData(m_LobbyID);

            if (!isLobbyValid)
            {
                Debug.Log("Steam 로비가 더 이상 유효하지 않습니다.");
                return (false, false);
            }

            SetupClientConnection();

            await Task.Yield();
            return (true, true);
        }

        protected override string GetPlayerId()
        {
            if (SteamAPI.IsSteamRunning())
            {
                return SteamUser.GetSteamID().m_SteamID.ToString();
            }

            return "";
        }
    }
}
