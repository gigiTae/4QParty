using FQParty.Common.Session;
using UnityEngine;

namespace FQParty.ConnectionManagement
{
    public class SessionPlayerData : ISessionPlayerData
    {
        public bool IsConnected { get; set; } // 현재 연결 상태
        public ulong ClientID { get; set; }   // Netcode에서 부여한 클라이언트 고유 ID
        public void Reinitialize() { }           // 데이터 초기화 메서드
    }
}