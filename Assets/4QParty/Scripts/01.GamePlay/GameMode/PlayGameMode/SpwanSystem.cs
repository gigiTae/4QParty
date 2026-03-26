using FQParty.GamePlay.Events;
using FQParty.GamePlay.GameplayObjects;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace FQParty.GamePlay.GameMode
{
    /// <summary>
    /// 스폰을 담당합니다 
    /// </summary>  
    public class SpawnSystem : NetworkBehaviour
    {
        [SerializeField] PlayGameModeSettings m_Settings;
        public void StartSpwan()
        {
            if (!IsServer) return;

            SpawnStartPlayerCharacters();
        }
        public override void OnNetworkSpawn()
        {
            m_Settings.RequestPossessEvent.Subscribe(OnRequestPossessEvent);
        }

        public override void OnNetworkDespawn()
        {
            m_Settings.RequestPossessEvent.Unsubscribe(OnRequestPossessEvent);
        }


        void OnRequestPossessEvent(RequestPossessContext context)
        {
            ulong ownerID = context.RequsetObject.OwnerClientId;
            NetworkObject possessObject = context.PossessableObject.PossessObject.GetComponent<NetworkObject>();
            Vector3 position = context.PossessableObject.transform.position;
            Quaternion rotation = context.PossessableObject.transform.rotation;

            context.RequsetObject.NetworkObject.Despawn(true);
            context.PossessableObject.NetworkObject.Despawn(false);
            SpwanPlayerCharacter(possessObject, ownerID, position, rotation);
        }

        void SpwanPlayerCharacter(NetworkObject networkObject, ulong clientID, Vector3 position, Quaternion rotation)
        {
            NetworkObject player = NetworkManager.SpawnManager.InstantiateAndSpawn(
             networkObject, clientID, false, true, false, position, rotation);
        }

        void SpawnStartPlayerCharacters()
        {
            if (!IsServer) return;

            var allPoints = FindObjectsByType<PlayerSpawnPoint>(FindObjectsSortMode.None);

            var sortedPoints = allPoints
                .OrderBy(p => p.PlayerIndex)
                .ToArray();

            if (sortedPoints.Length != 4)
            {
                Debug.LogError("PlayerSpawnPoint는 4개있어야합니다");
                return;
            }

            var connectedClientsIds = NetworkManager.ConnectedClientsIds;

            for (int i = 0; i < connectedClientsIds.Count; i++)
            {
                Transform pointT = sortedPoints[i].transform;
                Vector3 position = pointT.position;
                Quaternion rotation = pointT.rotation;

                NetworkObject networkObject = m_Settings.StartPlayerCharacter.GetComponent<NetworkObject>();
                SpwanPlayerCharacter(networkObject, connectedClientsIds[i], position, rotation);
            }
        }
    }
}