using FQParty.Common.Constant;
using Mono.Cecil;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace FQParty.UI
{
    [UxmlElement]
    public partial class LobbyBrowserElement : ListView
    {
        LobbyBrowserViewModel m_ViewModel;
        Button m_RefreshButton;
        Button m_JoinLobbyButton;
        Button m_CreateNewLobbyButton;
        List<DataBinding> m_DataBindings = new();

        public LobbyBrowserElement()
        {
            AddToClassList(UITheme.ScrollView);

            makeFooter = MakeFooter;
            makeItem = MaekDefaultItem;

            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachPanel);
        }

        VisualElement MakeFooter()
        {
            var root = new VisualElement();

            var refreshButton = new Button { text = "Refresh" };
            refreshButton.AddToClassList(UITheme.Button);
            root.Add(refreshButton);
            m_RefreshButton = refreshButton;

            var joinButton = new Button { text = "Join" };
            joinButton.AddToClassList(UITheme.Button);
            root.Add(joinButton);
            m_JoinLobbyButton = joinButton;

            var createNewButton = new Button { text = "Create New" };
            createNewButton.AddToClassList(UITheme.Button);
            root.Add(createNewButton);
            m_CreateNewLobbyButton = createNewButton;

            return root;
        }


        VisualElement MaekDefaultItem()
        {
            var root = new VisualElement();
            root.AddToClassList(UITheme.ScrollViewElement);

            var lobbyNameLabel = new Label();
            lobbyNameLabel.AddToClassList(UITheme.Label);
            root.Add(lobbyNameLabel);

            var db = new DataBinding
            {
                dataSourcePath = PropertyPath.FromName(nameof(LobbyDataViewModel.LobbyName)),
                bindingMode = BindingMode.ToTarget,
                updateTrigger = BindingUpdateTrigger.OnSourceChanged,
            };
            lobbyNameLabel.SetBinding(nameof(Label.text), db);

            return root;
        }

        private void OnAttachToPanel(AttachToPanelEvent _)
        {
            var listBindings = new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(LobbyBrowserViewModel.LobbyDataListViewModel)),
                bindingMode = BindingMode.ToTarget,
            };
            SetBinding(new BindingId(nameof(ListView.itemsSource)), listBindings);
            m_DataBindings.Add(listBindings);

            var selectionBinding = new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(LobbyBrowserViewModel.SelectedSessionIndex)),
                bindingMode = BindingMode.TwoWay
            };
            SetBinding(new BindingId(nameof(ListView.selectedIndex)), selectionBinding);
            m_DataBindings.Add(selectionBinding);

            var joinLobbyBinding = new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(LobbyBrowserViewModel.SelectedAndAvailable)),
                bindingMode = BindingMode.ToTarget,
            };

            m_JoinLobbyButton.SetBinding(new BindingId(nameof(enabledSelf)), joinLobbyBinding);
            m_DataBindings.Add(joinLobbyBinding);
            m_JoinLobbyButton.clicked += OnJoinLobbyAsync;

            m_RefreshButton.clicked += OnRefreshButtonClicked;

            UpdateBindingSources();
        }

        public async void OnRefreshButtonClicked()
        {
            ClearSelection();
            await m_ViewModel.RefreshLobbyListAsync();
        }

        public async void OnJoinLobbyAsync()
        {
            await m_ViewModel.JoinLobbyAsync();
        }

        private void OnDetachPanel(DetachFromPanelEvent _)
        {
            CleanupBindings();

            m_RefreshButton.clicked -= OnRefreshButtonClicked;
            m_JoinLobbyButton.clicked -= OnJoinLobbyAsync;
            m_JoinLobbyButton.ClearBinding(nameof(enabledSelf));

            ClearBindings();
        }

        private void UpdateBindingSources()
        {
            CleanupBindings();

            m_ViewModel = new LobbyBrowserViewModel();
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

    }
}
