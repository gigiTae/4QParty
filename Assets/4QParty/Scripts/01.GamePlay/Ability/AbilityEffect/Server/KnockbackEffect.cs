using FQParty.GamePlay.Character;
using FQParty.GamePlay.GameplayObjects;
using System;
using UnityEngine;

namespace FQParty.GamePlay.Abilities.Effects
{
    [Serializable]
    public enum KnockbackDirection
    {
        /// <summary> 시전자가 바라보는 정면 방향으로 밀려남 (직선형 스킬) </summary>
        CasterForward,

        /// <summary> 시전자 위치에서 피격자 위치로 향하는 방향 (폭발, 방사형 스킬) </summary>
        AwayFromCaster,

        /// <summary> 시전자 쪽으로 끌려옴 (그랩, 블랙홀 계열) </summary>
        TowardCaster,

        /// <summary> 월드 좌표계 기준 위쪽 방향 (에어본 효과 포함 시) </summary>
        WorldUp,
    }

    [Serializable]
    public class KnockbackEffect : ServerAbilityEffect, IInject<AbilityApplyData>
    {
        [SerializeField] KnockbackDirection m_DirectionType; 
        [SerializeField] float m_KnockbackDuration;
        [SerializeField] float m_KnockbackSpeed;

        ServerCharacter m_Caster;

        public void Inject(AbilityApplyData data)
        {
            m_Caster = data.Caster;
        }

        public override void OnStart(ServerCharacter serverCharacter, Ability ability)
        {
            // 시전자와 피격자(serverCharacter)가 유효하고, 넉백 인터페이스를 구현하고 있는지 확인
            if (m_Caster != null && serverCharacter.CharacterMovement is IKnockbackable knockbackable)
            {
                Vector3 knockbackDir = GetDirection(m_Caster.transform, serverCharacter.transform);
                knockbackable.ApplyKnockback(m_KnockbackSpeed, knockbackDir);
            }
            else
            {
                IsActive = false;
            }
        }

        public override void OnUpdate(ServerCharacter serverCharacter, Ability ability)
        {
            // 설정된 지속시간이 지나면 넉백 중단
            if (ability.TimeRunning >= m_KnockbackDuration)
            {
                if (serverCharacter.CharacterMovement is IKnockbackable knockbackable)
                {
                    knockbackable.CancelKnockback();
                }
                IsActive = false;
            }
        }

        /// <summary>
        /// Enum 타입에 따라 최종적인 넉백 방향 벡터를 계산합니다.
        /// </summary>
        private Vector3 GetDirection(Transform caster, Transform target)
        {
            switch (m_DirectionType)
            {
                case KnockbackDirection.CasterForward:
                    return caster.forward;

                case KnockbackDirection.AwayFromCaster:
                    Vector3 diff = target.position - caster.position;
                    diff.y = 0; // 바닥으로 처박히거나 하늘로 솟지 않게 Y축 보정
                    return diff.normalized;

                case KnockbackDirection.TowardCaster:
                    Vector3 toward = caster.position - target.position;
                    toward.y = 0;
                    return toward.normalized;

                case KnockbackDirection.WorldUp:
                    return Vector3.up;

                default:
                    return caster.forward;
            }
        }
    }
}