using FQParty.GamePlay.Character;
using FQParty.GamePlay.GameplayObjects;
using System;
using UnityEngine;

namespace FQParty.GamePlay.Abilities.Effects
{
    [Serializable]
    public class TakeDamageEffect : ServerAbilityEffect , IInject<AbilityApplyData>
    {   
        [SerializeField] float m_DamageMultiplier;
        ServerCharacter m_Caster;

        public void Inject(AbilityApplyData data)
        {
            m_Caster = data.Caster;
        }

        public override void OnStart(ServerCharacter serverCharacter, Ability ability)
        {
            if(m_Caster ==null)
            {
                Debug.LogWarning("TakeDameEffect: 공격을 한 대상을 찾을 수 없습니다");
                IsActive = false;
                return;
            }

            float damage = m_DamageMultiplier * m_Caster.CharacterStatus.AttackPower;
            serverCharacter.CharacterStatus.TakeDamage(damage);
            IsActive = false;
        }
    }

}