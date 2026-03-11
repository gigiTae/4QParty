using FQParty.Infrastructure;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace FQParty.ConnectionManagement
{
    /// <summary>
    /// 연결 상태 머신(Connection State Machine)에서 각 연결 단계(Offline, Connecting, Hosting 등)를 나타내는 기본 클래스입니다. 
    /// </summary>
    abstract class ConnectionState
    {
        [Inject]
        protected ConnectionManager m_ConnectionManager =null; // 상태 전환 및 전반적인 연결 관리를 담당하는 매니저 참조 

        [Inject]
        protected IPublisher<ConnectStatus> m_ConnectStatusPublisher; // 연결 성공, 실패, 끊김 등의 상태를 시스템 전반(UI 등)에 알리는 발행자 

        /// <summary>
        /// 해당 상태에 진입할 때 호출됩니다. 초기화 로직이나 특정 네트워크 시작 명령을 수행합니다. 
        /// </summary>
        public abstract void Enter();

        /// <summary>
        /// 해당 상태에서 나갈 때 호출됩니다. 리소스 정리나 이벤트 구독 해제 등을 수행합니다. 
        /// </summary>
        public abstract void Exit();

        /// <summary>
        /// 새로운 클라이언트가 서버에 성공적으로 접속했을 때 호출되는 콜백입니다. 
        /// </summary>
        public virtual void OnClientConnected(ulong clientId) { }

        /// <summary>
        /// 클라이언트의 연결이 끊어졌을 때(의도적 혹은 에러) 호출되는 콜백입니다. 
        /// </summary>
        public virtual void OnClientDisconnect(ulong clientId) { }

        /// <summary>
        /// 서버(혹은 호스트)가 성공적으로 시작되어 수신 대기 상태가 되었을 때 호출됩니다. 
        /// </summary>
        public virtual void OnServerStarted() { }

        /// <summary>
        /// SerivceProvider 를 사용하여 세션 기반 클라이언트 연결을 시작합니다. 
        /// </summary>
        public virtual void StartClientSession(string playerName) { }

        /// <summary>
        /// SerivceProvider를 통해 세션을 생성하고 호스트 모드를 시작합니다. 
        /// </summary>
        public virtual void StartHostSession(string playerName) { }

        /// <summary>
        /// 사용자가 UI 등을 통해 명시적으로 연결 종료나 게임 나가기를 요청했을 때 호출됩니다. 
        /// </summary>
        public virtual void OnUserRequestedShutdown() { }

        /// <summary>
        /// [서버 전용] 새로운 클라이언트의 접속 허가 여부를 결정합니다. 인원 초과 확인 등을 수행합니다. 
        /// </summary>
        public virtual void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) { }

        /// <summary>
        /// 네트워크 전송 레이어(Transport)에서 치명적인 에러가 발생했을 때 호출됩니다. [1, 3]
        /// </summary>
        public virtual void OnTransportFailure() { }

        /// <summary>
        /// 서버가 완전히 중지되었을 때 호출됩니다. 
        /// </summary>
        public virtual void OnServerStopped() { }
    }
}