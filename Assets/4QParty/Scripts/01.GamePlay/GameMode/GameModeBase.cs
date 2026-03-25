using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace FQParty.GamePlay.GameMode
{
    public abstract class GameModeBase : NetworkBehaviour
    {
        [SerializeField] protected GameObject m_SpwanPlayer;
        [SerializeField] protected Transform[] m_SpawnPoints;

        protected Dictionary<ulong, NetworkObject> m_PlayerObjects = new();

        private void Awake()
        {
        }

        void OnEnable()
        {
        }

        void OnDisable()
        {
        }


        public override void OnNetworkSpawn()
        {

        }

        public override void OnNetworkDespawn()
        {

        }
        void OnSynchronizeComplete(ulong clientId)
        {
            Debug.Log("SynchornizeComplete");
        }

    }
}