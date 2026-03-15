using FQParty.SteamService;
using Netcode.Transports;
using Steamworks;
using System.Threading.Tasks;
using Unity.Netcode;
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

        protected abstract void SetHostConnectionPayload();

        protected abstract string GetPlayerId();
    }

    /// <summary>
    /// UTP's Relay connection setup using the Session integration
    /// </summary>
    public class ConnectionMethodSteam : ConnectionMethodBase
    {
        public ConnectionMethodSteam(ConnectionManager connectionManager)
            : base(connectionManager)
        {
        }

        protected override void SetHostConnectionPayload()
        {
        }

        public override void SetupHostConnection()
        {
            if (!SteamAPI.IsSteamRunning()) return;

            ConnectionPayload payload = new()
            {
                SteamID = SteamUser.GetSteamID().m_SteamID,
                PlayerName = SteamFriends.GetPersonaName(),
            };

            string jsonPayload = JsonUtility.ToJson(payload);
            byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

            m_ConnectionManager.NetworkManager.NetworkConfig.ConnectionData = payloadBytes;
            var transport = m_ConnectionManager.NetworkManager.GetComponent<SteamNetworkingSocketsTransport>();
            transport.ConnectToSteamID = SteamUser.GetSteamID().m_SteamID;
        }

        public override void SetupClientConnection()
        {
            if (!SteamAPI.IsSteamRunning()) return;

            ConnectionPayload payload = new()
            {
                SteamID = SteamUser.GetSteamID().m_SteamID,
                PlayerName = SteamFriends.GetPersonaName(),
            };

            string jsonPayload = JsonUtility.ToJson(payload);
            byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

            m_ConnectionManager.NetworkManager.NetworkConfig.ConnectionData = payloadBytes;
            var transport = m_ConnectionManager.NetworkManager.GetComponent<SteamNetworkingSocketsTransport>();

            transport.ConnectToSteamID = SteamManager.Instance.SteamLobbyService.CurrentLobby.LobbyData.HostID;
        }

        public override async Task<(bool success, bool shouldTryAgain)> SetupClientReconnectionAsync()
        {
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
