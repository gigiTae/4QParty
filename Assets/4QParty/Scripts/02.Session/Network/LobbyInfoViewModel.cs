using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Properties;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UIElements;


namespace FQParty.Session.Network
{

    public class LobbyInfoViewModel : ISessionInfo,
         INotifyBindablePropertyChanged, IDataSourceViewHashProvider, IDisposable
    {
        const string m_Unavailalble = "N/A";

        ISession m_Session;
        ISessionInfo m_SessionInfo;

        long m_UpdateVersion;

        public LobbyInfoViewModel(ISessionInfo sessionInfo)
        {
            m_SessionInfo = sessionInfo;
        }

        public LobbyInfoViewModel(ISession session)
        {
            m_Session = session;

            m_Session.Changed += OnSessionChanged;
            m_Session.SessionHostChanged += OnSessionHostChanged;
            m_Session.SessionPropertiesChanged += OnSessionPropertiesChanged;

            // should we attempt to query the associated info?
        }

        /// <inheritdoc/>
        [CreateProperty]
        public string Name
            => m_SessionInfo?.Name ?? m_Session?.Name;

        /// <inheritdoc/>
        [CreateProperty]
        public string Id
            => m_SessionInfo?.Id ?? m_Session?.Id;

        /// <inheritdoc/>
        [CreateProperty]
        public string Upid
            => m_SessionInfo?.Upid ?? m_Unavailalble;

        /// <inheritdoc/>
        [CreateProperty]
        public string HostId
            => m_SessionInfo?.HostId ?? m_Session?.Host;

        /// <inheritdoc/>
        [CreateProperty]
        public int AvailableSlots
            => m_SessionInfo?.AvailableSlots ?? m_Session?.AvailableSlots ?? 0;

        /// <inheritdoc/>
        [CreateProperty]
        public int MaxPlayers
            => m_SessionInfo?.MaxPlayers ?? m_Session?.MaxPlayers ?? 0;

        /// <inheritdoc/>
        [CreateProperty]
        public bool IsLocked
            => m_SessionInfo?.IsLocked ?? m_Session?.IsLocked ?? true;

        /// <inheritdoc/>
        [CreateProperty]
        public bool HasPassword
            => m_SessionInfo?.HasPassword ?? m_Session?.HasPassword ?? true;

        /// <inheritdoc/>
        [CreateProperty]
        public DateTime LastUpdated
            => m_SessionInfo?.LastUpdated ?? DateTime.UnixEpoch;

        /// <inheritdoc/>
        [CreateProperty]
        public DateTime Created
            => m_SessionInfo?.Created ?? DateTime.UnixEpoch;

        /// <inheritdoc/>
        [CreateProperty]
        public IReadOnlyDictionary<string, SessionProperty> Properties
            => m_SessionInfo?.Properties ?? m_Session?.Properties;

        private void OnSessionHostChanged(string obj)
        {
            m_UpdateVersion++;
            Notify(nameof(HostId));
        }

        private void OnSessionPropertiesChanged()
        {
            m_UpdateVersion++;
            Notify(nameof(Properties));
        }

        private void OnSessionChanged()
        {
            m_UpdateVersion++;
            Notify(nameof(Name));
            Notify(nameof(LastUpdated));
            Notify(nameof(HasPassword));
            Notify(nameof(IsLocked));
            Notify(nameof(MaxPlayers));
            Notify(nameof(AvailableSlots));
        }

        public void Dispose()
        {
            if (m_Session != null)
            {
                m_Session.Changed -= OnSessionChanged;
                m_Session.SessionHostChanged -= OnSessionHostChanged;
                m_Session.SessionPropertiesChanged -= OnSessionPropertiesChanged;
            }

            m_Session = null;
            m_SessionInfo = null;
        }

        /// <summary>
        /// This method is used by UIToolkit to determine if any data bound to the UI has changed.
        /// Instead of hashing the data, an m_UpdateVersion counter is incremented when changes occur.
        /// </summary>
        public long GetViewHashCode() => m_UpdateVersion;

        /// <summary>
        /// Suggested implementation of INotifyBindablePropertyChanged from UIToolkit.
        /// </summary>
        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

        private void Notify([CallerMemberName] string property = null)
        {
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
        }
    }

}