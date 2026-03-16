using UnityEngine;
using UnityEngine.SceneManagement;
using FQParty.SceneManagement;
using FQParty.Common.Constant;


namespace FQParty.UI.Main
{
    public class MainMenuViewModel
    {
        public void StartLocalplay()
        {

        }

        public void StartMultiplay()
        {
            SceneLoader.Instance.LoadScene(SceneTheme.k_LobbyBrowser, false);
        }

        public void OpenSettings()
        {
            SceneLoader.Instance.LoadScene(SceneTheme.k_Setting, false);
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