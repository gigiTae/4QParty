using FQParty.ConnectionManagement;
using FQParty.SteamService;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace FQParty.UI
{
    public class LobbyBrowserViewModel : INotifyBindablePropertyChanged, IDataSourceViewHashProvider, IDisposable
    {
        [CreateProperty]
        public int SelectedSessionIndex
        {
            get => m_SelectedLobbyIndex;
            set
            {
                if (value >= 0 && value < LobbyDataListViewModel.Count)
                {
                    m_SelectedLobbyIndex = value;
                    SelectedAndAvailable = true;
                }
                else
                {
                    m_SelectedLobbyIndex = -1;
                    SelectedAndAvailable = false;
                }
            }
        }
        private int m_SelectedLobbyIndex = -1;

        [CreateProperty]
        public bool SelectedAndAvailable
        {
            get => m_SelectedAndAvailable;
            set
            {
                m_SelectedAndAvailable = value;
                ++m_UpdateVersion;
                Notify();
            }

        }
        private bool m_SelectedAndAvailable = false;

        public void Dispose()
        {

        }

        [CreateProperty]
        public List<LobbyDataViewModel> LobbyDataListViewModel
        {
            get => m_LobbyDataViewModelList;
            set
            {
                m_LobbyDataViewModelList = value;
                ++m_UpdateVersion;
                Notify();
            }
        }
        private List<LobbyDataViewModel> m_LobbyDataViewModelList = new();

        public async Task RefreshLobbyListAsync()
        {
            List<SteamLobbyData> lobbyDataList = await SteamManager.Instance.SteamLobbyService.GetLobbyListAsync();

            LobbyDataListViewModel.Clear();

            for (int i = 0; i < lobbyDataList.Count; i++)
            {
                var lobbyDataViewModel = new LobbyDataViewModel(lobbyDataList[i]);
                LobbyDataListViewModel.Add(lobbyDataViewModel);
            }

            ++m_UpdateVersion;
            Notify();
            SelectedSessionIndex = -1;
        }

        public async Task JoinLobbyAsync()
        {
            if (!m_SelectedAndAvailable)
            {
                return;
            }

            ulong lobbyID = m_LobbyDataViewModelList[m_SelectedLobbyIndex].LobbyID;
            SteamLobby lobby = await SteamManager.Instance.SteamLobbyService.JoinLobbyAsync(lobbyID);

            if (lobby != null)
            {
                ConnectionManager.Instance.StartSteamClientSession();
                m_SelectedAndAvailable = false;
            }
        }

        private long m_UpdateVersion;
        public long GetViewHashCode() => m_UpdateVersion;

        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;
        private void Notify([CallerMemberName] string property = null)
        {
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
        }
    }

}