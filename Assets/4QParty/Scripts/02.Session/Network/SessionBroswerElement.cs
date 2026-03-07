using FQParty.Common.Constant;
using FQParty.Session.Common;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;


namespace FQParty.Session.Network
{
    [UxmlElement]
    public partial class SessionBrowserElement : ListView
    {
        const string k_SessionNameLabel = "SessionNameLabel";
        const string k_SessionPlayerCountLabel = "SessionPlayerCountLabel";

        [CreateProperty, UxmlAttribute]
        public string JointButtonText
        {
            get => m_JoinButtonText;
            set
            {
                if (m_JoinButtonText == value)
                    return;

                m_JoinButtonText = value;

                if (m_JoinSessionButton != null)
                {
                    m_JoinSessionButton.text = value;
                }
            }
        }
        string m_JoinButtonText = "JOIN";

        [CreateProperty, UxmlAttribute]
        public string RefreshButtonText
        {
            get => m_RefreshButtonText;
            set
            {
                if (m_RefreshButtonText == value) return;

                m_RefreshButton.text = value;

                if (m_RefreshButton != null)
                {
                    m_RefreshButton.text = value;
                }
            }
        }
        string m_RefreshButtonText = "REFRESH LIST";

        [CreateProperty, UxmlAttribute]
        public string NoSessionFoundText
        {
            get => m_NoSessionFoundText;
            set
            {
                if (value == m_NoSessionFoundText)
                    return;

                m_NoSessionFoundText = value;
            }
        }
        string m_NoSessionFoundText = "No Session found";
        
        SessionSettingSO m_SessionSettings;
        SessionBrowserViewModel m_ViewModel;
        List<DataBinding> m_DataBindings;

        Button m_RefreshButton;
        Button m_JoinSessionButton;

        [CreateProperty, UxmlAttribute]
        public SessionSettingSO SessionSettings
        {
            get => m_SessionSettings;
            set
            {
                if (m_SessionSettings == value)
                    return;

                m_SessionSettings = value;
                if (panel != null)
                {
                    UpdateBindingSources();
                }
            }
        }

        [CreateProperty, UxmlAttribute]
        public int MaxSessionsDisplayed
        {
            get => m_MaxSessionsDisplayed;
            set => m_MaxSessionsDisplayed = value;
        }
        int m_MaxSessionsDisplayed = 20;

        // »ý¼ºÀÚ
        public SessionBrowserElement()
        {
            virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
            fixedItemHeight = 56f;

            bindingSourceSelectionMode = BindingSourceSelectionMode.AutoAssign;

            AddToClassList(UITheme.ScrollView);
            AddToClassList(UITheme.SpaceBottom);

            makeNoneElement = MakeNoneElement;
            makeItem = MakeDefaultItem;
            makeFooter = MakeFooter;

            RegisterCallback<AttachToPanelEvent>(OnAttachToPanelEvent);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanelEvent);
        }

        private VisualElement MakeFooter()
        {
            var buttonsContainer = new VisualElement();
            buttonsContainer.AddToClassList(UITheme.ContainerHorizontal);
            buttonsContainer.AddToClassList(UITheme.ContainerAlignedRight);

            m_JoinSessionButton = new Button { text = m_JoinButtonText };
            m_JoinSessionButton.AddToClassList(UITheme.Button);
            m_JoinSessionButton.AddToClassList(UITheme.SpaceRight);
            buttonsContainer.Add(m_JoinSessionButton);

            m_RefreshButton = new Button { text = m_RefreshButtonText };
            m_RefreshButton.AddToClassList(UITheme.Button);
            buttonsContainer.Add(m_RefreshButton);

            return buttonsContainer;
        }

        private void OnRefreshButtonClicked()
        {
            ClearSelection();
            _ = m_ViewModel?.UpdateSessionListAsync(MaxSessionsDisplayed);
        }

        private void JoinSession()
        {
            if (!m_ViewModel.SelectedAndAvailable)
            {
                Debug.LogError("Selected session is no longer selected.");
                return;
            }

            _ = m_ViewModel.JoinSessionAsync(SessionSettings.ToJoinSessionOptions());
        }

        private void UpdateBindingSources()
        {
            CleanupBindings();

            m_ViewModel = new SessionBrowserViewModel(SessionSettings?.SessionType);
            foreach (var dataBinding in m_DataBindings)
            {
                dataBinding.dataSource = m_ViewModel;
            }
        }

        private void CleanupBindings()
        {
            m_ViewModel?.Dispose();
            m_ViewModel = null;
            foreach (var dataBinding in m_DataBindings)
            {
                dataBinding.dataSource = null;
            }
        }

        private void OnDetachFromPanelEvent(DetachFromPanelEvent evt)
        {
            CleanupBindings();

            m_RefreshButton.clicked -= OnRefreshButtonClicked;
            m_RefreshButton.ClearBinding(nameof(SessionBrowserViewModel.CanRefresh));
            m_JoinSessionButton.clicked -= JoinSession;
            m_JoinSessionButton.ClearBinding(nameof(enabledSelf));

            ClearBindings();
        }

        private void OnAttachToPanelEvent(AttachToPanelEvent evt)
        {
            m_DataBindings = new List<DataBinding>();

            var listBinding = new DataBinding { dataSourcePath = new PropertyPath(nameof(SessionBrowserViewModel.Sessions)), bindingMode = BindingMode.ToTarget };
            SetBinding(new BindingId(nameof(ListView.itemsSource)), listBinding);
            m_DataBindings.Add(listBinding);

            var selectionBinding = new DataBinding { dataSourcePath = new PropertyPath(nameof(SessionBrowserViewModel.SelectedSessionIndex)), bindingMode = BindingMode.TwoWay };
            SetBinding(new BindingId(nameof(ListView.selectedIndex)), selectionBinding);
            m_DataBindings.Add(selectionBinding);

            var joinSessionBinding = new DataBinding { dataSourcePath = new PropertyPath(nameof(SessionBrowserViewModel.SelectedAndAvailable)), bindingMode = BindingMode.ToTarget };

            m_JoinSessionButton.SetBinding(new BindingId(nameof(enabledSelf)), joinSessionBinding);
            m_DataBindings.Add(joinSessionBinding);
            m_JoinSessionButton.clicked += JoinSession;

            var refreshBinding = new DataBinding { dataSourcePath = new PropertyPath(nameof(SessionBrowserViewModel.CanRefresh)), bindingMode = BindingMode.ToTarget };

            m_RefreshButton.SetBinding(new BindingId(nameof(enabledSelf)), refreshBinding);
            m_DataBindings.Add(refreshBinding);
            m_RefreshButton.clicked += OnRefreshButtonClicked;

            UpdateBindingSources();
        }

        private VisualElement MakeNoneElement()
        {
            var label = new Label(NoSessionFoundText);
            label.AddToClassList(UITheme.Label);
            label.AddToClassList(UITheme.SpaceLeft);
            return label;
        }

        private VisualElement MakeDefaultItem()
        {
            var container = new VisualElement();
            container.AddToClassList(UITheme.ContainerHorizontal);
            container.AddToClassList(UITheme.ScrollViewElement);
            container.AddToClassList(UITheme.ContainerSpaceBetween);

            var sessionNameLabel = new Label { name = k_SessionNameLabel };
            sessionNameLabel.AddToClassList(UITheme.Label);
            sessionNameLabel.AddToClassList(UITheme.SpaceLeft);
            container.Add(sessionNameLabel);

            var db = new DataBinding
            {
                dataSourcePath = PropertyPath.FromName(nameof(SessionInfoViewModel.Name)),
                bindingMode = BindingMode.ToTarget,
                updateTrigger = BindingUpdateTrigger.OnSourceChanged
            };
            sessionNameLabel.SetBinding(nameof(Label.text), db);

            var sessionPlayerCountLabel = new Label { name = k_SessionPlayerCountLabel };
            sessionPlayerCountLabel.AddToClassList(UITheme.Label);
            sessionPlayerCountLabel.AddToClassList(UITheme.SpaceRight);
            container.Add(sessionPlayerCountLabel);

            var sessionPlayerCountBinding = new DataBinding { bindingMode = BindingMode.ToTarget };

            // register a local converter to display relevant session properties as a formatted string
            sessionPlayerCountBinding.sourceToUiConverters
                .AddConverter((ref SessionInfoViewModel session) => $"{session.MaxPlayers - session.AvailableSlots}/{session.MaxPlayers} Players");

            sessionPlayerCountLabel.SetBinding(nameof(Label.text), sessionPlayerCountBinding);

            return container;
        }
    }
}

