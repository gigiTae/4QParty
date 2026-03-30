using FQParty.GamePlay.Character;
using System;
using UnityEngine;


namespace FQParty.GamePlay.Abilities.Effects
{
    [Serializable]
    public class TakeDotDamageEffect : ServerAbilityEffect, IInject<AbilityApplyData>
    {
        [SerializeField] float m_DamageMultiplier;
        [SerializeField] float m_Duration;
        [SerializeField] float m_TickInterval = 1f;

        float m_NextDotTime = 0f;

        ServerCharacter m_Caster;
        public void Inject(AbilityApplyData data)
        {
            m_Caster = data.Caster;
        }
        public override void OnStart(ServerCharacter serverCharacter, Ability ability)
        {
            if (m_Caster == null)
            {
                Debug.LogWarning("TakeDameEffect: 공격을 한 대상을 찾을 수 없습니다");
                IsActive = false;
                return;
            }

            m_NextDotTime = m_TickInterval;
        }

        public override void OnUpdate(ServerCharacter serverCharacter, Ability ability)
        {
            if(ability.TimeRunning >= m_NextDotTime)
            {
                m_NextDotTime += m_TickInterval;
                TakeDotDamage(serverCharacter);
            }

            if(ability.TimeRunning >= m_Duration)
            {
                IsActive = false;
                m_Caster = null;
            }
        }

        public override void Cancel(ServerCharacter serverCharacter, Ability ability)
        {
            m_Caster = null;
        }

        void TakeDotDamage(ServerCharacter serverCharacter)
        {
            float damage = m_DamageMultiplier * m_Caster.CharacterStatus.AttackPower;
            serverCharacter.CharacterStatus.TakeDamage(damage);
        }
    }

}