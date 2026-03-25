using FQParty.ConnectionManagement;
using FQParty.SteamService;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Properties;
using Unity.Services.Multiplayer;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;


namespace FQParty.Session.Network
{
    public class CreateLobbyViewModel : IDisposable, IDataSourceViewHashProvider, INotifyBindablePropertyChanged
    {
        ConnectionManager m_ConnectionManager;

        SessionObserver m_SessionObserver;
        SteamLobbyData m_LobbyData;
        long m_UpdateVersion;

        [CreateProperty]
        public bool CanRegisterLobby
        {
            get => m_CanRegisterSession;
            private set
            {
                if (m_CanRegisterSession == value)
                    return;

                m_CanRegisterSession = value;
                ++m_UpdateVersion;
                Notify();
            }
        }
        bool m_CanRegisterSession = true;

        [CreateProperty]
        public bool HasSessionName
        {
            get => m_HasSessionName;
            private set
            {
                if (m_HasSessionName == value)
                    return;

                m_HasSessionName = value;
                Notify();
            }
        }
        bool m_HasSessionName;

        [CreateProperty]
        public string SessionName
        {
            get => m_SessionName;
            private set
            {
                if (m_SessionName == value)
                    return;

                m_SessionName = value;
                HasSessionName = m_SessionName != "";

                ++m_UpdateVersion;
                Notify();
            }
        }
        string m_SessionName;

        public CreateLobbyViewModel(string sessionType)
        {
            m_SessionObserver = new SessionObserver(sessionType);

            m_SessionObserver.AddingSessionStarted += OnAddingSessionStarted;
            m_SessionObserver.SessionAdded += OnSessionAdded;
            m_SessionObserver.AddingSessionFailed += OnAddingSessionFailed;

            if (m_SessionObserver.Session != null)
            {
                OnSessionAdded(m_SessionObserver.Session);
            }
        }

        void OnAddingSessionFailed(AddingSessionOptions session, SessionException exception) => CanRegisterLobby = true;
        void OnAddingSessionStarted(AddingSessionOptions session) => CanRegisterLobby = false;

        void OnSessionAdded(ISession session)
        {


            //m_Session = session;
            // m_Session.RemovedFromSession += OnSessionRemoved;
            // m_Session.Deleted += OnSessionRemoved;
            CanRegisterLobby = false;
        }

        void OnSessionRemoved()
        {
            //m_Session.RemovedFromSession -= OnSessionRemoved;
            //m_Session.Deleted -= OnSessionRemoved;
            //m_Session = null;
            CanRegisterLobby = true;
        }

        public bool AreMultiplayerServicesInitialized()
        {
            return SteamManager.Instance.IsInitialized;
        }

        public async Task CreateLobbyAsync()
        {
            await SteamManager.Instance.SteamLobbyService.CreateLobbyAsync(SessionName, false);
            ConnectionManager.Instance.StartSteamHostSession();
        }

        public void Dispose()
        {
            if (m_SessionObserver != null)
            {
                m_SessionObserver.AddingSessionStarted -= OnAddingSessionStarted;
                m_SessionObserver.SessionAdded -= OnSessionAdded;
                m_SessionObserver.AddingSessionFailed -= OnAddingSessionFailed;
                m_SessionObserver.Dispose();
                m_SessionObserver = null;
            }

            //if (m_Session != null)
            //{
            //    m_Session.RemovedFromSession -= OnSessionRemoved;
            //    m_Session.Deleted -= OnSessionRemoved;
            //    m_Session = null;
            //}
        }

        public long GetViewHashCode() => m_UpdateVersion;


        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;
        void Notify([CallerMemberName] string property = null) =>
              propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
    }

}