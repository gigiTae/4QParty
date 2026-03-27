using FQParty.GamePlay.Character;
using FQParty.GamePlay.Character.Movement;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace FQParty.GamePlay.Abilities.Effects
{
    [Serializable]
    public class PlayerDashEffect : ClientAbilityEffect
    {
        [SerializeField] float DashDuration = 0.25f;
        [SerializeField] float DashSpeed = 20f;

        public override void OnStart(ClientCharacter clientCharacter, Ability ability)
        {
            if (clientCharacter.CharacterMovement is IDashable dash)
            {
                dash.StartDash(DashSpeed); 
            }
        }
        public override void OnUpdate(ClientCharacter clientCharacter, Ability ability)
        {
            if(ability.TimeRunning >= DashDuration)
            {
                IsActive = false;
                Cancel(clientCharacter, ability);
            }
        }

        public override void Cancel(ClientCharacter clientCharacter, Ability ability)
        {
            if (clientCharacter.CharacterMovement is IDashable dash)
            {
                dash.CancelDash();
            }
        }
    }

}