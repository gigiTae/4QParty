using FQParty.GamePlay.Abilities;
using FQParty.GamePlay.Input;
using Unity.Netcode;
using UnityEngine;


namespace FQParty.GamePlay.Character
{
    public class ClientCharacter : NetworkBehaviour
    {
        [SerializeField] ServerCharacter m_ServerCharacter;
        [SerializeField] ClientPlayerCharacterMovement m_ClientCharacterMovement;

        ClientAbilityPlayer m_ClientAbilityPlayer;

        [Rpc(SendTo.ClientsAndHost)]
        public void ClientPlayAbilityRpc(AbilityRequestData data)
        {
            AbilityRequestData data1 = data;
            m_ClientAbilityPlayer.PlayAbility(ref data1);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void ClientCancelAllActionRpc()
        {
            m_ClientAbilityPlayer.CancelAllAbilities();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if(!IsClient)
            {
                enabled = false;    
                return;
            }
            m_ClientAbilityPlayer = new ClientAbilityPlayer(this);  

            if(m_ServerCharacter.IsOwner)
            {
                if(m_ServerCharacter.TryGetComponent(out ClientInputSender inputSender))
                {
                    if(!IsServer)
                    {
                        inputSender.AbilityInputEvent += OnAbilityInput;
                    }
                }
            }
        }

        void OnAbilityInput(AbilityRequestData data)
        {
            m_ClientAbilityPlayer.AnticipateAction(ref data);   
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();    

        }

        private void Update()
        {
            m_ClientAbilityPlayer.OnUpdate();
        }
    }

}