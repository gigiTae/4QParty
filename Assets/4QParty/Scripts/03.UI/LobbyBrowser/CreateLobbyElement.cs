using FQParty.Common.Constant;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;


namespace FQParty.UI
{
    [UxmlElement]
    public partial class CreateLobbyElement : VisualElement
    {
        private CreateLobbyViewModel m_ViewModel;
        private TextField m_LobbyNameTextField;
        private Button m_CreateLobbyButton;
        private Button m_BackButton;

        readonly List<DataBinding> m_ViewModelBindings = new();

        [CreateProperty, UxmlAttribute]
        public string CreateText
        {
            get => m_CreateText;
            set
            {
                m_CreateText = value;
                if (m_CreateLobbyButton != null)
                {
                    m_CreateLobbyButton.text = value;
                }
            }
        }
        private string m_CreateText = "CREATE";

        [CreateProperty, UxmlAttribute]
        public string BackText
        {
            get => m_BackText;
            set
            {
                m_BackText = value;
                if (m_BackButton != null)
                {
                    m_BackButton.text = value;
                }
            }
        }
        private string m_BackText = "BACK";


        public CreateLobbyElement()
        {
            MakeLobbyNameTextField();
            MakeBackButton();
            MakeCreateLobbyButton();    

            RegisterCallback<AttachToPanelEvent>(_ => UpdateBindings());
            RegisterCallback<DetachFromPanelEvent>(_ => CleanupBindings());
        }

        void MakeLobbyNameTextField()
        {
            var lobbyNameTextField = new TextField();

            lobbyNameTextField.AddToClassList(UITheme.TextField);

            var lobbyNameBinding = new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(m_ViewModel.LobbyName)),
                bindingMode = BindingMode.ToSource
            };

            lobbyNameTextField.SetBinding("value", lobbyNameBinding);

            Add(lobbyNameTextField);
            m_ViewModelBindings.Add(lobbyNameBinding);

        }

        void MakeCreateLobbyButton()
        {
            var createLobbyButton = new Button()
            {
                text = CreateText,
            };

            createLobbyButton.AddToClassList(UITheme.Button);
            var createLobbyBinding = new DataBinding
            {
                dataSource = new PropertyPath(nameof(m_ViewModel.HasLobbyName)),
                bindingMode = BindingMode.ToTarget
            };
            createLobbyButton.SetBinding(new BindingId(nameof(enabledSelf)), createLobbyBinding);
            createLobbyButton.clicked += OnCreateLobbyAsync;
            Add(createLobbyButton);

            m_ViewModelBindings.Add(createLobbyBinding);
        }


        public async void OnCreateLobbyAsync()
        {
            await m_ViewModel.CreateLobbyAsync();
        }

        void MakeBackButton()
        {
            var backButton = new Button()
            { text = BackText, };

            backButton.AddToClassList(UITheme.Button);

            backButton.clicked += OnBackButtonClicked;

            Add(backButton);
        }

        public void OnBackButtonClicked()
        {
            Debug.Log("Back");
        }

        void UpdateBindings()
        {
            CleanupBindings();

            m_ViewModel = new CreateLobbyViewModel();
            foreach (var binding in m_ViewModelBindings)
            {
                binding.dataSource = m_ViewModel;
            }
        }

        void CleanupBindings()
        {
            m_ViewModel?.Dispose();
            m_ViewModel = null;

            foreach (var binding in m_ViewModelBindings)
            {
                binding.dataSource = null;
            }
        }
    }

}