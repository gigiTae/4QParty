using UnityEngine;
using UnityEngine.SceneManagement;
using FQParty.SceneManagement;
using FQParty.Common.Constant;


namespace FQParty.UI.Main
{
    public class MainMenuViewModel
    {
        private LoadSceneGroupEvent m_LoadSceneGroupEvent;

        public MainMenuViewModel(LoadSceneGroupEvent evt)
        {
            m_LoadSceneGroupEvent = evt;
        }

        public void StartLocalplay()
        {

        }

        public void StartMultiplay()
        {
            LoadSceneGroupContext context = new()
            {
                GroupName = SceneGroupTheme.k_LobbyBrowser
            };

            m_LoadSceneGroupEvent.Raise(context);
        }

        public void OpenSettings()
        {
            LoadSceneGroupContext context = new()
            {
                GroupName = SceneGroupTheme.k_SettingGroup
            };
            m_LoadSceneGroupEvent.Raise(context);
        }

        public void EndGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif

        }
    }
}