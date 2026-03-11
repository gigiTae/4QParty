using System;
using System.Collections.Generic;
using UnityEngine;

namespace FQParty.Common.Session
{
    /// <summary>
    /// 세션 내 플레이어 데이터를 정의하는 인터페이스
    /// </summary>
    public interface ISessionPlayerData
    {
        bool IsConnected { get; set; } 
        ulong ClientID { get; set; }   // 클라이언트 고유 ID
        void Reinitialize();
    }

    /// <summary>
    /// 플레이어의 고유 ID를 사용하여 세션과 플레이어를 바인딩하는 클래스입니다.
    /// 플레이어가 호스트에 접속하면, 호스트는 현재의 ClientID를 플레이어의 고유 ID(고정값)에 연결합니다.
    /// 플레이어가 접속을 끊었다가 다시 접속해도 이 세션 관리자를 통해 기존 데이터를 보존할 수 있습니다.
    /// </summary>
    public class SessionManager<T> where T : struct, ISessionPlayerData
    {
        // 싱글톤 생성자
        SessionManager()
        {
            m_ClientData = new Dictionary<string, T>();
            m_ClientIDToPlayerId = new Dictionary<ulong, string>();
        }

        public static SessionManager<T> Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new SessionManager<T>();
                }
                return s_Instance;
            }
        }

        static SessionManager<T> s_Instance;

        /// <summary>
        /// 플레이어 고유 ID(string)를 해당 플레이어의 데이터(T)에 매핑합니다.
        /// </summary>
        Dictionary<string, T> m_ClientData;

        /// <summary>
        /// Netcode ClientID(ulong)를 플레이어 고유 ID(string)로 빠르게 찾기 위한 매핑 테이블입니다.
        /// </summary>
        Dictionary<ulong, string> m_ClientIDToPlayerId;

        // 세션 시작 여부 (시작된 후에는 접속이 끊겨도 데이터를 유지함)
        bool m_HasSessionStarted;

        /// <summary>
        /// 클라이언트 접속 해제 처리
        /// </summary>
        public void DisconnectClient(ulong clientId)
        {
            if (m_HasSessionStarted)
            {
                // 세션이 이미 시작되었다면, 재접속을 대비해 데이터를 삭제하지 않고 '연결 끊김' 표시만 합니다.
                if (m_ClientIDToPlayerId.TryGetValue(clientId, out var playerId))
                {
                    var playerData = GetPlayerData(playerId);
                    if (playerData != null && playerData.Value.ClientID == clientId)
                    {
                        var clientData = m_ClientData[playerId];
                        clientData.IsConnected = false;
                        m_ClientData[playerId] = clientData;
                    }
                }
            }
            else
            {
                // 세션이 시작되기 전이라면 데이터를 유지할 필요가 없으므로 즉시 삭제합니다.
                if (m_ClientIDToPlayerId.TryGetValue(clientId, out var playerId))
                {
                    m_ClientIDToPlayerId.Remove(clientId);
                    var playerData = GetPlayerData(playerId);
                    if (playerData != null && playerData.Value.ClientID == clientId)
                    {
                        m_ClientData.Remove(playerId);
                    }
                }
            }
        }

        /// <summary>
        /// 중복 접속 여부 확인
        /// </summary>
        /// <param name="playerId">클라이언트의 고유 플레이어 ID</param>
        /// <returns>이미 동일한 ID의 플레이어가 연결되어 있다면 true</returns>
        public bool IsDuplicateConnection(string playerId)
        {
            return m_ClientData.ContainsKey(playerId) && m_ClientData[playerId].IsConnected;
        }

        /// <summary>
        /// 접속 중인 플레이어의 세션 데이터를 설정합니다. 새로운 접속이면 데이터를 추가하고,
        /// 재접속이면 기존 데이터를 갱신합니다.
        /// </summary>
        /// <param name="clientId">Netcode가 이번 로그인에 부여한 ID (접속할 때마다 바뀜)</param>
        /// <param name="playerId">클라이언트 고유 ID (재접속 시에도 유지됨)</param>
        /// <param name="sessionPlayerData">초기 플레이어 데이터</param>
        public void SetupConnectingPlayerSessionData(ulong clientId, string playerId, T sessionPlayerData)
        {
            var isReconnecting = false;

            // 중복 접속 테스트
            if (IsDuplicateConnection(playerId))
            {
                Debug.LogError($"플레이어 ID {playerId}가 이미 존재합니다. 중복 접속이므로 세션 데이터를 거부합니다.");
                return;
            }

            // 해당 ID의 데이터가 이미 존재한다면
            if (m_ClientData.ContainsKey(playerId))
            {
                // 그런데 연결 상태가 false라면, 이는 튕겼다가 다시 들어오는 '재접속' 상황입니다.
                if (!m_ClientData[playerId].IsConnected)
                {
                    isReconnecting = true;
                }
            }

            // 재접속 시: 기존 데이터를 새 플레이어 정보에 덮어씌웁니다.
            if (isReconnecting)
            {
                sessionPlayerData = m_ClientData[playerId];
                sessionPlayerData.ClientID = clientId; // 새로운 클라이언트 ID 부여
                sessionPlayerData.IsConnected = true;   // 연결 상태 복구
            }

            // 딕셔너리에 데이터 업데이트
            m_ClientIDToPlayerId[clientId] = playerId;
            m_ClientData[playerId] = sessionPlayerData;
        }

        /// <summary>
        /// Client ID를 통해 고유 Player ID를 가져옵니다.
        /// </summary>
        public string GetPlayerId(ulong clientId)
        {
            if (m_ClientIDToPlayerId.TryGetValue(clientId, out string playerId))
            {
                return playerId;
            }

            Debug.Log($"해당 Client ID와 매핑된 Player ID를 찾을 수 없습니다: {clientId}");
            return null;
        }

        /// <summary>
        /// Client ID를 통해 플레이어 데이터를 가져옵니다.
        /// </summary>
        public T? GetPlayerData(ulong clientId)
        {
            var playerId = GetPlayerId(clientId);
            if (playerId != null)
            {
                return GetPlayerData(playerId);
            }

            return null;
        }

        /// <summary>
        /// Player ID를 통해 플레이어 데이터를 가져옵니다.
        /// </summary>
        public T? GetPlayerData(string playerId)
        {
            if (m_ClientData.TryGetValue(playerId, out T data))
            {
                return data;
            }

            Debug.Log($"해당 Player ID와 일치하는 플레이어 데이터가 없습니다: {playerId}");
            return null;
        }

        /// <summary>
        /// 플레이어 데이터를 수동으로 업데이트합니다.
        /// </summary>
        public void SetPlayerData(ulong clientId, T sessionPlayerData)
        {
            if (m_ClientIDToPlayerId.TryGetValue(clientId, out string playerId))
            {
                m_ClientData[playerId] = sessionPlayerData;
            }
            else
            {
                Debug.LogError($"데이터를 업데이트할 Client ID를 찾을 수 없습니다: {clientId}");
            }
        }

        /// <summary>
        /// 세션 시작 시 호출됩니다. 이때부터는 플레이어가 나가도 데이터를 보존합니다.
        /// </summary>
        public void OnSessionStarted()
        {
            m_HasSessionStarted = true;
        }

        /// <summary>
        /// 세션 종료 시 호출됩니다. 접속이 끊긴 플레이어 데이터는 삭제하고, 남은 플레이어 데이터는 초기화합니다.
        /// </summary>
        public void OnSessionEnded()
        {
            ClearDisconnectedPlayersData();
            ReinitializePlayersData();
            m_HasSessionStarted = false;
        }

        /// <summary>
        /// 서버 종료 시 모든 런타임 상태를 리셋합니다.
        /// </summary>
        public void OnServerEnded()
        {
            m_ClientData.Clear();
            m_ClientIDToPlayerId.Clear();
            m_HasSessionStarted = false;
        }

        // 현재 접속 중인 플레이어들의 데이터를 게임 시작 전 상태로 재초기화합니다.
        void ReinitializePlayersData()
        {
            foreach (var id in m_ClientIDToPlayerId.Keys)
            {
                string playerId = m_ClientIDToPlayerId[id];
                T sessionPlayerData = m_ClientData[playerId];
                sessionPlayerData.Reinitialize();
                m_ClientData[playerId] = sessionPlayerData;
            }
        }

        // 접속이 끊긴 플레이어들의 데이터를 세션 종료 시 완전히 제거합니다.
        void ClearDisconnectedPlayersData()
        {
            List<ulong> idsToClear = new List<ulong>();
            foreach (var id in m_ClientIDToPlayerId.Keys)
            {
                var data = GetPlayerData(id);
                if (data is { IsConnected: false }) // 연결이 끊긴 상태라면
                {
                    idsToClear.Add(id);
                }
            }

            foreach (var id in idsToClear)
            {
                string playerId = m_ClientIDToPlayerId[id];
                var playerData = GetPlayerData(playerId);
                if (playerData != null && playerData.Value.ClientID == id)
                {
                    m_ClientData.Remove(playerId);
                }

                m_ClientIDToPlayerId.Remove(id);
            }
        }
    }
}