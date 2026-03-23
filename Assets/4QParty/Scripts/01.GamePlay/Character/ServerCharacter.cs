using FQParty.GamePlay.Abilities;
using FQParty.GamePlay.Character.Movement;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;


namespace FQParty.GamePlay.Character
{
    public class ServerCharacter : NetworkBehaviour
    {
        public ClientCharacter ClientCharacter => m_ClientCharacter;
        [SerializeField] ClientCharacter m_ClientCharacter;
        public CharacterMovement CharacterMovement => m_Movement;
        [SerializeField] CharacterMovement m_Movement;

        public CharacterStatus CharacterStatus=> m_CharacterStatus;
        [SerializeField] CharacterStatus m_CharacterStatus;

        public NetworkAnimator NetworkAnimator => m_NetworkAnimator;
        [SerializeField]
        NetworkAnimator m_NetworkAnimator;
       
        public ServerAbilityPlayer AbilityPlayer => m_ServerAbilityPlayer;
        ServerAbilityPlayer m_ServerAbilityPlayer;

        private void Awake()
        {
            m_ServerAbilityPlayer = new ServerAbilityPlayer(this);
        }

        void Update()
        {
            m_ServerAbilityPlayer.OnUpdateAbility();
        }

        [Rpc(SendTo.Server)]
        public void RequestAbilityServerRpc(AbilityRequestData data)
        {
            m_ServerAbilityPlayer.RequestAbility(data);
        }

    }

}