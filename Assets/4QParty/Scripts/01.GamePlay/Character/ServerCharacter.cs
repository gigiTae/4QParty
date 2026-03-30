using FQParty.GamePlay.Abilities;
using FQParty.GamePlay.Character.Movement;
using NUnit.Framework;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;


namespace FQParty.GamePlay.Character
{
    public class ServerCharacter : NetworkBehaviour
    {
        [SerializeField] ClientCharacter m_ClientCharacter;
        public ClientCharacter ClientCharacter => m_ClientCharacter;
        
        [SerializeField] CharacterMovement m_Movement;
        public CharacterMovement CharacterMovement => m_Movement;
        
        [SerializeField] CharacterStatus m_CharacterStatus;
        public CharacterStatus CharacterStatus => m_CharacterStatus;
        
        [SerializeField] NetworkAnimator m_NetworkAnimator;
        public NetworkAnimator NetworkAnimator => m_NetworkAnimator;

        [SerializeField] ServerAbilityPlayer m_ServerAbilityPlayer;
        public ServerAbilityPlayer AbilityPlayer => m_ServerAbilityPlayer;
    }

}