using FQParty.GamePlay.Character;
using FQParty.GamePlay.GameplayObjects;
using System;
using UnityEngine;

namespace FQParty.GamePlay.Abilities.Effects
{
    [Serializable]
    public class KnockbackEffect : ServerAbilityEffect, IInject<AbilityApplyData>
    {
        [SerializeField] float m_KnockbackDuration;
        [SerializeField] float m_KnockbackSpeed;

        ServerCharacter m_Caster;
        public void Inject(AbilityApplyData data)
        {
            m_Caster = data.Caster;
        }

        public override void OnStart(ServerCharacter serverCharacter, Ability ability)
        {
            if (m_Caster == null)
            {
                IsActive = false;
                return;
            }

            if (serverCharacter.CharacterMovement is IKnockbackable knockbackable)
            {
                knockbackable.ApplyKnockback(m_KnockbackSpeed, m_Caster.transform.forward);
            }
        }

        public override void OnUpdate(ServerCharacter serverCharacter, Ability ability)
        {
            if (ability.TimeRunning >= m_KnockbackDuration)
            {
                if (serverCharacter.CharacterMovement is IKnockbackable knockbackable)
                {
                    knockbackable.CancelKnockback();
                }
                IsActive = false;
            }
        }
    }

}