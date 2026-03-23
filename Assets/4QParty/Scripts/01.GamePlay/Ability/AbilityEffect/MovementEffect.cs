using FQParty.GamePlay.Character;
using System;
using UnityEngine;
using static FQParty.GamePlay.Character.CharacterMovement;

namespace FQParty.GamePlay.Abilities.Effects
{
    /// <summary>
    /// 이동 관련 이펙트를 설정합니다
    /// </summary>
    [Serializable]
    public class MovementEffect : AbilityEffect
    {
        public override void OnStart(ServerCharacter serverCharacter, Ability ability)
        {
            serverCharacter.CharacterMovement.SetMovementStateServerRpc(MovementState.Stop);
        }
        public override void OnUpdate(ServerCharacter serverCharacter, Ability ability)
        {
                
        }

        public override void Cancel(ServerCharacter serverCharacter, Ability ability)
        {
            serverCharacter.CharacterMovement.SetMovementStateServerRpc(MovementState.Moveable);
        }

    }

}