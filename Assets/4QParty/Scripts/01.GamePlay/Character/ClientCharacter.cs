using FQParty.GamePlay.Abilities;
using FQParty.GamePlay.Character.Movement;
using FQParty.GamePlay.Input;
using Unity.Netcode;
using UnityEngine;


namespace FQParty.GamePlay.Character
{
    public class ClientCharacter : NetworkBehaviour
    {
        [SerializeField] ServerCharacter m_ServerCharacter;
        public CharacterMovement CharacterMovement => m_CharacterMovement;
        [SerializeField] CharacterMovement m_CharacterMovement;

        [SerializeField] ClientAbilityPlayer m_ClientAbilityPlayer;

        [Rpc(SendTo.ClientsAndHost)]
        public void PlayAbilityClientRpc(AbilityRequestData data)
        {
            AbilityRequestData data1 = data;
            m_ClientAbilityPlayer.PlayAbility(ref data1);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void CancelAllActionClientRpc()
        {
            m_ClientAbilityPlayer.CancelAllAbilities();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsClient)
            {
                enabled = false;
                return;
            }

            if (IsOwner)
            {
                if (m_ServerCharacter.TryGetComponent(out ClientInputSender inputSender))
                {
                    inputSender.AbilityInputEvent += OnAbilityInput;
                }
            }
        }


        void OnAbilityInput(AbilityRequestData data)
        {
            if (!IsServer)
            {
                m_ClientAbilityPlayer.AnticipateAbility(ref data);
            }

            m_ClientAbilityPlayer.StartClientMoveAbility(ref data);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

        }

    }

}