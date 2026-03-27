using FQParty.GamePlay.Character;
using FQParty.GamePlay.Character.Movement;
using System;
using UnityEngine;

namespace FQParty.GamePlay.Abilities.Effects
{
    [Serializable]
    public class PlayerMovementLockEffect : ClientAbilityEffect
    {
        [SerializeField] float LockDuration = 1f;

        public override void OnStart(ClientCharacter clientCharacter, Ability ability)
        {
            if (clientCharacter.CharacterMovement is PlayerCharacterMovement movement)
            {
                Debug.Log("Lock");
                movement.LockMovement();
            }

        }
        public override void OnUpdate(ClientCharacter clientCharacter, Ability ability)
        {
            if (ability.TimeRunning >= LockDuration &&
                clientCharacter.CharacterMovement is PlayerCharacterMovement movement)
            {
                Debug.Log("UnLock");
                IsActive = false;
                movement.UnlockMovement();
            }
        }

        public override void Cancel(ClientCharacter clientCharacter, Ability ability)
        {
            if (clientCharacter.CharacterMovement is PlayerCharacterMovement movement)
            {
                movement.UnlockMovement();
            }
        }
    }

}