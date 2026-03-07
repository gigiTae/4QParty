using Codice.Client.Common;
using FQParty.Common.Constant;
using FQParty.Session.Common;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;


namespace FQParty.Session.Network
{
    [UxmlElement]
    public partial class CreateSessionElement : VisualElement
    {
        private CreateSessionViewModel m_ViewModel;
        private TextField m_SessionNameTextField;
        private Button m_CreateSessionButton;

        
        [CreateProperty, UxmlAttribute]
        public string CreateButtonText
        {
            get => m_CreateButtonText;
            set
            {
                m_CreateButtonText = value;
                if (m_CreateSessionButton != null)
                {
                    m_CreateSessionButton.text = value;
                }
            }
        }
        string m_CreateButtonText = "CREATE";

        [CreateProperty, UxmlAttribute]
        public string EnterSessionNamePlaceholder
        {
            get => m_EnterSessionNamePlaceholder;
            set
            {
                m_EnterSessionNamePlaceholder = value;

                if (m_SessionNameTextField != null)
                {
                    m_SessionNameTextField.textEdition.placeholder = value;
                }
            }
        }
        string m_EnterSessionNamePlaceholder = "Enter Session Name";

        readonly List<DataBinding> m_ViewModelBindings = new();

        [CreateProperty, UxmlAttribute]
        public SessionSettingSO SessionSettings
        {
            get => m_SessionSettings;
            set
            {
                if (m_SessionSettings == value) return;

                m_SessionSettings = value;
                if (panel != null)
                {
                    UpdateBindings();
                }
            }
        }
        private SessionSettingSO m_SessionSettings;

        public CreateSessionElement()
        {
            AddToClassList(UITheme.ContainerHorizontal);

            SetEnabledBinding();
            MakeSessionNameTextField();
            MakeCreateSessionButton();

            // _ => : 전달되는 인수가 필요없음 
            RegisterCallback<AttachToPanelEvent>(_ => UpdateBindings());
            RegisterCallback<DetachFromPanelEvent>(_ => CleanupBindings());
        }

        void SetEnabledBinding()
        {
            var enabledBinding = new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(m_ViewModel.CanRegisterSession)),
                bindingMode = BindingMode.ToTarget
            };

            SetBinding(new BindingId(nameof(enabledSelf)), enabledBinding);
            m_ViewModelBindings.Add(enabledBinding);
        }

        void MakeSessionNameTextField()
        {
            var sessionNameTextField = new TextField
            {
                textEdition =
                {
                    placeholder = m_EnterSessionNamePlaceholder,
                    hidePlaceholderOnFocus = true
                }
            };

            sessionNameTextField.AddToClassList(UITheme.TextField);
            sessionNameTextField.AddToClassList(UITheme.SpaceRight);

            // session name binding
            var sessionNameBinding = new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(m_ViewModel.SessionName)),
                bindingMode = BindingMode.ToSource
            };
            sessionNameTextField.SetBinding("value", sessionNameBinding);
            Add(sessionNameTextField);
            m_ViewModelBindings.Add(sessionNameBinding);

            m_SessionNameTextField = sessionNameTextField;
        }

        void MakeCreateSessionButton()
        {
            var createSessionButton = new Button
            {
                text = CreateButtonText,
            };
            createSessionButton.AddToClassList(UITheme.Button);
            var createSessionBinding = new DataBinding
            {
                dataSource = new PropertyPath(nameof(m_ViewModel.HasSessionName)),
                bindingMode = BindingMode.ToTarget
            };
            createSessionButton.SetBinding(new BindingId(nameof(enabledSelf)), createSessionBinding);
            createSessionButton.clicked += CreateSession;
            Add(createSessionButton);
            m_ViewModelBindings.Add(createSessionBinding);

            m_CreateSessionButton = createSessionButton;
        }

        void CreateSession()
        {
            if (!SessionSettings)
            {
                Debug.LogError("SessionSettings is null, it needs to be assigned in the uxml.");
                return;
            }
            if (!m_ViewModel.AreMultiplayerServicesInitialized())
            {
                Debug.LogError("Multiplayer Services are not initialized. You can initialize them with default settings by adding a ServicesInitialization and PlayerAuthentication components in your scene.");
                return;
            }

            _ = m_ViewModel.CreateSessionAsync(SessionSettings.ToSessionOptions());
        }


        void UpdateBindings()
        {
            CleanupBindings();

            m_ViewModel = new CreateSessionViewModel(SessionSettings?.SessionType);
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