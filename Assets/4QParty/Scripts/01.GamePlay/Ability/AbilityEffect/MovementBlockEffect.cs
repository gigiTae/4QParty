using FQParty.GamePlay.Character;
using System;
using UnityEngine;
using static FQParty.GamePlay.Character.Movement.CharacterMovement;

namespace FQParty.GamePlay.Abilities.Effects
{
    /// <summary>
    /// 이동 관련 이펙트를 설정합니다
    /// </summary>
    [Serializable]
    public class MovementBlockEffect : AbilityEffect
    {
        [SerializeField] float m_BlockDuration = 1f;
        float m_StartTime = 0f;
        bool m_IsActive;

        public override void OnStart(ServerCharacter serverCharacter, Ability ability)
        {
            m_StartTime = Time.time;
            m_IsActive = true;
            serverCharacter.CharacterMovement.SetMovementStateServerRpc(MovementState.Stop);
        }
        public override void OnUpdate(ServerCharacter serverCharacter, Ability ability)
        {
            if (Time.time - m_StartTime > m_BlockDuration) 
            {
                m_IsActive = false;
                serverCharacter.CharacterMovement.SetMovementStateServerRpc(MovementState.Moveable);
            }
        }

        public override void Cancel(ServerCharacter serverCharacter, Ability ability)
        {
            serverCharacter.CharacterMovement.SetMovementStateServerRpc(MovementState.Moveable);
        }

        public override bool IsActive => m_IsActive;
    }

}