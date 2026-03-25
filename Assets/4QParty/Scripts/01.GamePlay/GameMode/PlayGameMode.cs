using FQParty.GamePlay.Cam;
using FQParty.GamePlay.GameMode;
using FQParty.GamePlay.GameplayObjects;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace FQParty.GamePlay.GameMode
{
    public class PlayGameMode : GameModeBase
    {
        [SerializeField] GameObject m_SpwanPlayer;
        [SerializeField] PlayerCamera m_PlayerCamera;

        Dictionary<ulong, NetworkObject> m_Players;

        protected override void StartGameMode()
        {
            SpawnPlayers();
            SetPlayerCameraRpc();
        }

        [Rpc(SendTo.ClientsAndHost)]
        void SetPlayerCameraRpc()
        {
            Debug.Log("SetPlayerCamera");

            ulong id = NetworkManager.LocalClientId;

            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(id, out var networkClient))
            {
                NetworkObject playerObject = networkClient.PlayerObject;

                if (playerObject != null)
                {
                    Debug.Log($"플레이어 카메라 설정 완료: {playerObject.name}");
                    m_PlayerCamera.SetTarget(playerObject.transform);
                }
            }
            else
            {
                Debug.LogWarning($"ID {id}에 해당하는 플레이어를 찾을 수 없습니다.");
            }
        }

        void SpawnPlayers()
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

                NetworkObject player = NetworkManager.SpawnManager.InstantiateAndSpawn(
                    m_SpwanPlayer.GetComponent<NetworkObject>(),
                    connectedClientsIds[i],
                    false,
                    true,
                    false,
                    position,
                    rotation
                    );
            }
        }
    }

}