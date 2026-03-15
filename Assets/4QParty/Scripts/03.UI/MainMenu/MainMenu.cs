using FQParty.SceneManagement;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace FQParty.UI.Main
{
    /// <summary>
    /// MainMenu UI
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private UIDocument m_Document;

        [SerializeField]
        private LoadSceneGroupEvent m_LoadSceneGroupEvent;

        private MainMenuViewModel m_ViewModel;
        private MainMenuElement m_View;
        void Awake()
        {
            m_ViewModel = new MainMenuViewModel(m_LoadSceneGroupEvent);
            m_View = m_Document.rootVisualElement.Q<MainMenuElement>();
            m_View?.SetViewModel(m_ViewModel);
        }
    }
}