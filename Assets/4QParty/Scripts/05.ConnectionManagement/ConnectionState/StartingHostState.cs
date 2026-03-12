using FQParty.Common.Session;
using FQParty.SteamService;
using System;
using Unity.Netcode;
using UnityEngine;
using VContainer;


namespace FQParty.ConnectionManagement
{
    class StartingHostState : OnlineState
    {
        ConnectionMethodBase m_ConnectionMethod;
        public StartingHostState Configure(ConnectionMethodBase baseConnectionMethod)
        {
            m_ConnectionMethod = baseConnectionMethod;
            return this;
        }

        public override void Enter()
        {
            StartHost();
        }

        public override void OnServerStarted()
        {
            //m_ConnectStatusPublisher.Publish(ConnectStatus.Success);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_Hosting);
        }

        public override void Exit() { }

        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            var connectionData = request.Payload;
            var clientId = request.ClientNetworkId;

            // LocalClientId와 현재 접속 요청을 보낸 ClientId가 같다면 자기 자신(호스트)입니다.
            if (clientId == m_ConnectionManager.NetworkManager.LocalClientId)
            {
                var payloadJson = System.Text.Encoding.UTF8.GetString(connectionData);
                var connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payloadJson);

                SessionManager<SessionPlayerData>.Instance.SetupConnectingPlayerSessionData(
                    clientId,
                    connectionPayload.Id.ToString(),
                    new SessionPlayerData()
                );

                // 승인 설정
                response.Approved = true;
                response.CreatePlayerObject = false;
            }
        }

        void StartHost()
        {
            m_ConnectionMethod.SetupHostConnection();
            if (!m_ConnectionManager.NetworkManager.StartHost())
            {
                StartHostFailed();
            }
        }

        public override void OnServerStopped()
        {
            StartHostFailed();
        }

        void StartHostFailed()
        {
            // m_ConnectStatusPublisher.Publish(ConnectStatus.StartHostFailed);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_Offline);
        }
    }

}