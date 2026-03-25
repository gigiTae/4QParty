using FQParty.SceneManagement;
using FQParty.Common.Constant;

namespace FQParty.ConnectionManagement
{
    /// <summary>
    /// 
    /// </summary>
    class OfflineState : ConnectionState
    {
        public override void Enter()
        {
            m_ConnectionManager.NetworkManager.Shutdown();

            SceneLoader.Instance.LoadScene(SceneTheme.k_Main, false);
        }

        public override void Exit() { }

        public override void StartSteamClientSession()
        {
            var connectionMethod = new ConnectionMethodSteam(m_ConnectionManager);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_ClientConnecting.Configure(connectionMethod));
        }

        public override void StartSteamHostSession()
        {
            var connectionMethod = new ConnectionMethodSteam(m_ConnectionManager);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_StartingHost.Configure(connectionMethod));
        }

        public override void StartUnityClientSession()
        {
            var connectionMethod = new ConnectionMethodUnityEditor(m_ConnectionManager);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_ClientConnecting.Configure(connectionMethod));
        }

        public override void StartUnityHostSession()
        {
            var connectionMethod = new ConnectionMethodUnityEditor(m_ConnectionManager);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_StartingHost.Configure(connectionMethod));
        }
    }
}