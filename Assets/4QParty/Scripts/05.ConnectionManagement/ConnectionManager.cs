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
        //public FixedPlayerName PlayerName;
    }
    

    public class ConnectionManager : MonoBehaviour
    {
        ConnectionState m_CurrentState;

        [Inject]
        NetworkManager m_NetworkManager;
        public NetworkManager NetworkManager => m_NetworkManager;

        [SerializeField]
        int m_NbReconnectAttempts = 2;
        public int NbReconnectAttempts => m_NbReconnectAttempts;

        [Inject]
        IObjectResolver m_Resolver;

        public int MaxConnectedPlayers = 4;


    }

}