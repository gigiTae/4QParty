using FQParty.ConnectionManagement;
using FQParty.SteamService;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;


namespace FQParty.UI
{
    public class CreateLobbyViewModel : IDisposable, IDataSourceViewHashProvider, INotifyBindablePropertyChanged
    {

        [CreateProperty]
        public string LobbyName
        {
            get => m_LobbyName;
            private set
            {
                m_LobbyName = value;
                Notify();
            }

        }
        string m_LobbyName;

        [CreateProperty]
        public bool HasLobbyName
        {
            get => m_LobbyName != "";
        }

        public async Task CreateLobbyAsync()
        {
            await SteamManager.Instance.SteamLobbyService.CreateLobbyAsync(LobbyName, false);
            ConnectionManager.Instance.StartSteamHostSession();
        }

        public void Dispose()
        {

        }

        long m_UpdateVersion;

        public long GetViewHashCode() => m_UpdateVersion;

        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;
        void Notify([CallerMemberName] string property = null) =>
              propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
    }
}