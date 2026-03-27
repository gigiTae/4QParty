using FQParty.GamePlay.Abilities;
using FQParty.GamePlay.Character.Movement;
using FQParty.GamePlay.Input;
using Unity.Netcode;
using UnityEngine;


namespace FQParty.GamePlay.Character
{
    public class ClientCharacter : NetworkBehaviour
    {
        public CharacterMovement CharacterMovement => m_CharacterMovement;
        [SerializeField] CharacterMovement m_CharacterMovement;

    }

}