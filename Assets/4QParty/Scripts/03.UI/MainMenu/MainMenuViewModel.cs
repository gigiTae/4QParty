using FQParty.Common.Setting;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace FQParty.UI.Main
{
    public class MainMenuViewModel
    {
        public MainMenuViewModel(BuildSceneList list)
        {
            m_SceneList = list;
        }

        private BuildSceneList m_SceneList;


        public void StartGame()
        {
            SceneManager.LoadScene(m_SceneList.MainScene);
        }

        public void OpenSettings()
        {
            SceneManager.LoadScene(m_SceneList.SettingScene);
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