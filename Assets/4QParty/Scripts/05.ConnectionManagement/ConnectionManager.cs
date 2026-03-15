using FQParty.Common.Persistance;
using FQParty.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using Unity.Netcode;
using UnityEngine;
using VContainer;


namespace FQParty.ConnectionManagement
{
    public enum ConnectStatus
    {
        // 정의되지 않음 (기본값)
        Undefined,
        // 클라이언트가 성공적으로 연결됨. (재연결 성공 포함)
        Success,
        // 서버 인원이 가득 차서 참가할 수 없음.
        ServerFull,
        // 다른 클라이언트에서 동일한 계정으로 로그인하여 현재 세션에서 튕김.
        LoggedInAgain,
        // 사용자가 직접 '연결 종료'를 선택하여 의도적으로 연결을 끊음.
        UserRequestedDisconnect,
        // 서버와의 연결이 끊겼으나, 구체적인 이유를 알 수 없음.
        GenericDisconnect,
        // 클라이언트의 연결이 유실되어 재연결을 시도 중임.
        Reconnecting,
        // 클라이언트의 빌드 타입(버전 등)이 서버와 호환되지 않음.
        IncompatibleBuildType,
        // 호스트가 의도적으로 세션을 종료함.
        HostEndedSession,
        // 서버(호스트) 시작 실패 (포트 바인딩 실패 등).
        StartHostFailed,
        // 서버 연결 실패 또는 유효하지 않은 네트워크 엔드포인트(주소).
        StartClientFailed
    }

    public struct ReconnectMessage
    {
        public int CurrentAttempt;
        public int MaxAttempt;

        public ReconnectMessage(int currentAttempt, int maxAttempt)
        {
            CurrentAttempt = currentAttempt;
            MaxAttempt = maxAttempt;
        }
    }

    public struct ConnectionEventMessage : INetworkSerializeByMemcpy
    {
        public ConnectStatus ConnectStatus;
        public string PlayerName;
    }

    [Serializable]
    public class ConnectionPayload
    {
        public ulong SteamID;
        public string PlayerName;
    }

    public class TempPublisher : IPublisher<ConnectStatus>
    {
        public void Publish(ConnectStatus message)
        {
           
        }
    }



    public class ConnectionManager : PersistanceSingleton<ConnectionManager>
    {
        ConnectionState m_CurrentState;

        [SerializeField]
        NetworkManager m_NetworkManager = null;
        public NetworkManager NetworkManager => m_NetworkManager;

        TempPublisher m_TempPublisher = new();

        internal readonly OfflineState m_Offline = new OfflineState();
        internal readonly ClientConnectingState m_ClientConnecting = new ClientConnectingState();
        internal readonly ClientConnectedState m_ClientConnected = new ClientConnectedState();
        internal readonly ClientReconnectingState m_ClientReconnecting = new ClientReconnectingState();
        internal readonly StartingHostState m_StartingHost = new StartingHostState();
        internal readonly HostingState m_Hosting = new HostingState();

        internal void ChangeState(ConnectionState nextState)
        {
            Debug.Log($"{name}: Changed connection state from {m_CurrentState.GetType().Name} to {nextState.GetType().Name}.");

            if (m_CurrentState != null)
            {
                m_CurrentState.Exit();
            }
            m_CurrentState = nextState;
            m_CurrentState.Enter();
        }

        protected override void Awake()
        {
            base.Awake();

            List<ConnectionState> states = new() { m_Offline, m_ClientConnecting, m_ClientConnected, m_ClientReconnecting, m_StartingHost, m_Hosting };
            foreach (var connectionState in states)
            {
                connectionState.m_ConnectionManager = this;
                connectionState.m_ConnectStatusPublisher = m_TempPublisher;
            }

            NetworkManager.OnConnectionEvent += OnConnectionEvent;
            NetworkManager.OnServerStarted += OnServerStarted;
            NetworkManager.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.OnTransportFailure += OnTransportFailure;
            NetworkManager.OnServerStopped += OnServerStopped;
        }

        public void Start()
        {
            m_CurrentState = m_Offline;
        }

        private void OnDestroy()
        {
            NetworkManager.OnConnectionEvent -= OnConnectionEvent;
            NetworkManager.OnServerStarted -= OnServerStarted;
            NetworkManager.ConnectionApprovalCallback -= ApprovalCheck;
            NetworkManager.OnTransportFailure -= OnTransportFailure;
            NetworkManager.OnServerStopped -= OnServerStopped;
        }

        void OnConnectionEvent(NetworkManager networkManager, ConnectionEventData connectionEventData)
        {

            switch (connectionEventData.EventType)
            {
                case ConnectionEvent.ClientConnected:
                    m_CurrentState.OnClientConnected(connectionEventData.ClientId);
                    break;
                case ConnectionEvent.ClientDisconnected:
                    m_CurrentState.OnClientDisconnect(connectionEventData.ClientId);
                    break;
            }
        }

        void OnServerStarted()
        {
            m_CurrentState.OnServerStarted();
        }

        void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            m_CurrentState.ApprovalCheck(request, response);
        }

        void OnTransportFailure()
        {
            m_CurrentState.OnTransportFailure();
        }

        void OnServerStopped(bool _) // we don't need this parameter as the ConnectionState already carries the relevant information
        {
            m_CurrentState.OnServerStopped();
        }

        public void StartClientSession()
        {
            m_CurrentState.StartClientSession();
        }

        public void StartHostSession()
        {
            m_CurrentState.StartHostSession();
        }

        public void RequestShutdown()
        {
            m_CurrentState.OnUserRequestedShutdown();
        }

    }
}