using FQParty.Common.Setting;
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

        private MainMenuViewModel m_ViewModel;
        private MainMenuElement m_View;

        [SerializeField]
        private BuildSceneList m_SceneList;

        void Awake()
        {
            m_ViewModel = new MainMenuViewModel(m_SceneList);

            m_View = m_Document.rootVisualElement.Q<MainMenuElement>();
            m_View?.SetViewModel(m_ViewModel);
        }
    }
}