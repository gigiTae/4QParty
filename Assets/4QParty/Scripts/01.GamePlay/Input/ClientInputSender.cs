using FQParty.GamePlay.Character;
using Unity.Netcode;
using UnityEngine;


namespace FQParty.GamePlay.Input
{
    /// <summary>
    /// 입력을 받아서 어빌리티를 실행하는 객체
    /// </summary>
    [RequireComponent(typeof(ServerCharacter))]
    public class ClientInputSender : NetworkBehaviour
    {
        [SerializeField]
        GamePlayInputReader m_GameInputReader;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsClient || !IsOwner)
            {
                {
                    enabled = false;
                    return;
                }
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        }



    }
}