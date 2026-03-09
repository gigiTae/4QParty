using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace FQParty.UI.Main
{
    [UxmlElement]
    public partial class MainMenuElement : VisualElement
    {
        private MainMenuViewModel m_ViewModel;
        private Button m_StartButton;
        private Button m_SettingButton;
        private Button m_EndButton;

        [CreateProperty, UxmlAttribute]
        public string StartButtonText
        {
            get => m_StartButtonText;
            set
            {
                if (m_StartButton != null)
                {
                    m_StartButton.text = value;
                }
            }
        }
        private string m_StartButtonText;

        [CreateProperty, UxmlAttribute]
        public string SettingButtonText
        {
            get => m_SettingButtonText;
            set
            {
                if (m_SettingButton != null)
                {
                    m_SettingButton.text = value;
                }
            }
        }
        private string m_SettingButtonText;

        [CreateProperty, UxmlAttribute ]
        public string EndButtonText
        {
            get => m_EndButtonText;
            set
            {
                if (m_EndButton != null)
                {
                    m_EndButton.text = value;
                }
            }
        }
        private string m_EndButtonText;


        public MainMenuElement()
        {
            InitializeUI();

            RegisterCallback<DetachFromPanelEvent>(OnDetach);
        }

        private void InitializeUI()
        {
            m_StartButton = new Button { text = m_StartButtonText };
            m_SettingButton = new Button { text = m_SettingButtonText };
            m_EndButton = new Button { text = m_EndButtonText };

            Add(m_StartButton);
            Add(m_SettingButton);
            Add(m_EndButton);
        }

        private void OnDetach(DetachFromPanelEvent evt)
        {
            if (m_ViewModel != null)
            {
                m_StartButton.clicked -= m_ViewModel.StartGame;
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
            m_StartButton.clicked += m_ViewModel.StartGame;
            m_SettingButton.clicked += m_ViewModel.OpenSettings;
            m_EndButton.clicked += m_ViewModel.EndGame;
        }
    }
}