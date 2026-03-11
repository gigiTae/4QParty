using FQParty.Services;
using FQParty.SceneManagement;
using FQParty.Common.Constant;

namespace FQParty.ConnectionManagement
{
    /// <summary>
    /// 
    /// </summary>
    class OfflineState : ConnectionState
    {
        ServiceProvider m_ServiceProvider;

        public override void Enter()
        {
            m_ConnectionManager.NetworkManager.Shutdown();

            LoadSceneGroupContext contex = new() { GroupName = SceneGroupTheme.k_MainGroup, UseNetworkSceneManager = false };
            SceneLoader.Instance.LoadSceneGroup(contex);
        }

        public override void Exit() { }

        public override void StartClientSession(string playerName)
        {
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_ClientConnecting);
        }

        public override void StartHostSession(string playerName)
        {
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_StartingHost);
        }
    }
}