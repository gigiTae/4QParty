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

        public override void StartClientSession()
        {
            var connectionMethod = new ConnectionMethodSteam(m_ConnectionManager);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_ClientConnecting.Configure(connectionMethod));
        }

        public override void StartHostSession()
        {
            var connectionMethod = new ConnectionMethodSteam(m_ConnectionManager);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_StartingHost.Configure(connectionMethod));
        }
    }
}