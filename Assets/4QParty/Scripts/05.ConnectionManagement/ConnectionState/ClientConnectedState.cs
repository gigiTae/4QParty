using FQParty.Common.Constant;
using FQParty.SceneManagement;
using UnityEngine;


namespace FQParty.ConnectionManagement
{
    class ClientConnectedState : OnlineState
    {
        public override void Enter()
        {
            SceneLoader.Instance.LoadSceneGroup(new LoadSceneGroupContext()
            {
                GroupName = SceneGroupTheme.k_Lobby,
                UseNetworkSceneManager = false
            });
        }

        public override void Exit() { }

        public override void OnClientConnected(ulong clientId)
        {
            var disconnectReason = m_ConnectionManager.NetworkManager.DisconnectReason;

            if (string.IsNullOrEmpty(disconnectReason) ||
               disconnectReason == "Disconnected due to host shutting down.")
            {
                //m_ConnectStatusPublisher.Publish(ConnectStatus.Reconnecting);
                m_ConnectionManager.ChangeState(m_ConnectionManager.m_ClientReconnecting);
            }
            else
            {
                var connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                //m_ConnectStatusPublisher.Publish(connectStatus);
                m_ConnectionManager.ChangeState(m_ConnectionManager.m_Offline);
            }
        }


    }

}