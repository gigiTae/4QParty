using FQParty.Common.Session;
using System.Globalization;
using UnityEngine;


namespace FQParty.SteamService
{
    public struct SessionPlayerData : ISessionPlayerData
    {
        public string PlayerName;
        public bool IsConnected { get; set; }
        public ulong ClientID { get; set; }   // 클라이언트 고유 ID
        public void Reinitialize() { }
    }
}
