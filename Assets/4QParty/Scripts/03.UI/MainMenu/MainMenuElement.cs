using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace FQParty.UI.Main
{
    [UxmlElement]
    public partial class MainMenuElement : VisualElement
    {
        private MainMenuViewModel m_ViewModel;
        private Button m_StartLocalplayButton = new();
        private Button m_StarMultiplayButton = new();
        private Button m_SettingButton = new();
        private Button m_EndButton = new();

        [CreateProperty, UxmlAttribute]
        public string StartLocalplayButtonText
        {
            get => m_StartLocalplayButton.text;
            set
            {
                m_StartLocalplayButton.text = value;
            }
        }

        [CreateProperty, UxmlAttribute]
        public string StartMultiplayButtonText
        {
            get => m_StarMultiplayButton.text;
            set
            {
                m_StarMultiplayButton.text = value;
            }
        }

    
        [CreateProperty, UxmlAttribute]
        public string SettingButtonText
        {
            get => m_SettingButton.text;
            set
            {
                m_SettingButton.text = value;
            }
        }

        [CreateProperty, UxmlAttribute]
        public string EndButtonText
        {
            get => m_EndButton.text;
            set
            {
                m_EndButton.text = value;
            }
        }

        public MainMenuElement()
        {
            InitializeUI();

            RegisterCallback<DetachFromPanelEvent>(OnDetach);
        }

        private void InitializeUI()
        {
            Add(m_StartLocalplayButton);
            Add(m_StarMultiplayButton);
            Add(m_SettingButton);
            Add(m_EndButton);
        }

        private void OnDetach(DetachFromPanelEvent evt)
        {
            if (m_ViewModel != null)
            {
                m_StartLocalplayButton.clicked -= m_ViewModel.StartLocalplay;
                m_StarMultiplayButton.clicked -= m_ViewModel.StartMultiplay;
                m_SettingButton.clicked -= m_ViewModel.OpenSettings;
                m_EndButton.clicked -= m_ViewModel.EndGame;
                m_ViewModel = null;
            }
        }

        public void SetViewModel(MainMenuViewModel viewModel)
        {
            if (viewModel == null)
            {
                Debug.LogWarning("Attempted to set a null ViewModel on MainMenuElement.");
                return;
            }

            m_ViewModel = viewModel;
            m_StartLocalplayButton.clicked += m_ViewModel.StartLocalplay;
            m_StarMultiplayButton.clicked += m_ViewModel.StartMultiplay;
            m_SettingButton.clicked += m_ViewModel.OpenSettings;
            m_EndButton.clicked += m_ViewModel.EndGame;
        }
    }
}