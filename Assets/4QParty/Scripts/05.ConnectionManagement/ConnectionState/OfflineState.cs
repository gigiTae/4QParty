using System.Data;
using Unity.Services.Multiplayer;
using UnityEngine;
using VContainer;

namespace FQParty.ConnectionManagement
{
    /// <summary>
    /// 
    /// </summary>
    class OfflineState : ConnectionState
    {
        public override void Enter()
        {

        }

        public override void Exit() { }

        public override void StartClientSession(string playerName)
        {

        }

        public override void StartHostSession(string playerName)
        {
        }
    }
}