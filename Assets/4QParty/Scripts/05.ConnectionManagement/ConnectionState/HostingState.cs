using FQParty.Common.Constant;
using FQParty.Common.Session;
using FQParty.Infrastructure;
using FQParty.SceneManagement;
using Unity.Netcode;
using UnityEngine;
using FQParty.SteamService;

namespace FQParty.ConnectionManagement
{
    class HostingState : OnlineState
    {
        IPublisher<ConnectionEventMessage> m_ConnectionEventPublisher;


        public override void Enter()
        {
            SceneLoader.Instance.LoadSceneGroup(new LoadSceneGroupContext()
            {
                GroupName = SceneGroupTheme.k_Lobby,
                UseNetworkSceneManager = false
            });
        }

        public override void Exit()
        {
            SessionManager<SessionPlayerData>.Instance.OnServerEnded();
        }

        public override void OnClientConnected(ulong clientId)
        {
            SessionPlayerData? playerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(clientId);

            if (playerData != null)
            {   
                return;
                m_ConnectionEventPublisher.Publish(new ConnectionEventMessage()
                {
                    ConnectStatus = ConnectStatus.Success,
                    PlayerName = playerData.Value.PlayerName
                });
            }
            else
            {
                Debug.LogError($"No player data associated with client {clientId}");
                var reason = JsonUtility.ToJson(ConnectStatus.GenericDisconnect);
                m_ConnectionManager.NetworkManager.DisconnectClient(clientId, reason);
            }
        }

        public override void OnUserRequestedShutdown()
        {
            var reason = JsonUtility.ToJson(ConnectStatus.HostEndedSession);
            for (var i = m_ConnectionManager.NetworkManager.ConnectedClientsIds.Count - 1; i >= 0; i--)
            {
                var id = m_ConnectionManager.NetworkManager.ConnectedClientsIds[i];
                if (id != m_ConnectionManager.NetworkManager.LocalClientId)
                {
                    m_ConnectionManager.NetworkManager.DisconnectClient(id, reason);
                }
            }
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_Offline);
        }
        public override void OnServerStopped()
        {
            m_ConnectStatusPublisher.Publish(ConnectStatus.GenericDisconnect);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_Offline);
        }

        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            var connectionData = request.Payload;
            var clientId = request.ClientNetworkId;

            var payload = System.Text.Encoding.UTF8.GetString(connectionData);
            var connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);
            var gameReturnStatus = GetConnectStatus(connectionPayload);
            
            if(gameReturnStatus == ConnectStatus.Success)
            {

                return;
            }


        }


        ConnectStatus GetConnectStatus(ConnectionPayload connectionPayload)
        {
            if (m_ConnectionManager.NetworkManager.ConnectedClientsIds.Count >= 4)
            {
                return ConnectStatus.ServerFull;
            }

            return ConnectStatus.Success;
            //    return SessionManager<SessionPlayerData>.Instance.IsDuplicateConnection(connectionPayload.Id) ?
            //        ConnectStatus.LoggedInAgain : ConnectStatus.Success;
            //}
        }
    }
}