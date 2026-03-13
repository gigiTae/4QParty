using FQParty.SteamService;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;


namespace FQParty.UI
{
    public class LobbyDataViewModel : INotifyBindablePropertyChanged, IDataSourceViewHashProvider, IDisposable
    {
        LobbyData m_LobbyData;

        public LobbyDataViewModel(LobbyData data)
        {
            m_LobbyData = data;
        }

        public void Refresh()
        {
            m_UpdateVersion++;
            Notify(nameof(LobbyName));
        }

        public string LobbyName
        {
            get => m_LobbyData.LobbyName;
        }

        public int CurrenPlayers
        {
            get => m_LobbyData.CurrentPlayers;
        }

        public int MaxPlayers
        {
            get => m_LobbyData.MaxPlayers;
        }

        public ulong HostID
        {
            get => m_LobbyData.HostID;
        }

        public void Dispose()
        {

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