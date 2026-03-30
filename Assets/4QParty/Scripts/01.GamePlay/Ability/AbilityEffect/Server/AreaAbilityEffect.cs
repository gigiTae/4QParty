using FQParty.GamePlay.Abilities.Effects;
using FQParty.GamePlay.Character;
using FQParty.GamePlay.GameplayObjects;
using System;
using UnityEngine;

namespace FQParty.GamePlay.Abilities
{
    /// <summary>
    /// 콜라이더를 생성해서 닿는 대상에서 어빌리티를 부여합니다
    /// </summary>
    [Serializable]
    public class AreaAbilityEffect : ServerAbilityEffect
    {
        [SerializeField] Ability m_ApplyAbility;

        [SerializeField] float m_Duration = 1f;

        ContactTrigger m_ContactTrigger;
        ServerCharacter m_ServerCharacter;

        public override void OnStart(ServerCharacter serverCharacter, Ability ability)
        {
            m_ServerCharacter = serverCharacter;
            m_ContactTrigger = serverCharacter.GetComponentInChildren<ContactTrigger>();

            if (m_ContactTrigger != null)
            {
                m_ContactTrigger.OnTriggerEneterEvent += OnTriggerEneter;
            }
            else
            {
                Debug.Log("AreaAbilityEffect는 자식오브젝트에 ContactTrigger가 반드시 필요합니다");
                IsActive = false;
            }
        }
        public override void OnUpdate(ServerCharacter serverCharacter, Ability ability)
        {
            if (ability.TimeRunning >= m_Duration)
            {
                m_ContactTrigger.OnTriggerEneterEvent -= OnTriggerEneter;
                m_ServerCharacter = null;
                m_ContactTrigger = null;
                IsActive = false;
            }
        }

        public override void Cancel(ServerCharacter serverCharacter, Ability ability)
        {
            if (m_ContactTrigger != null)
            {
                m_ContactTrigger.OnTriggerEneterEvent -= OnTriggerEneter;
                m_ServerCharacter = null;
                m_ContactTrigger = null;
            }
        }

        void OnTriggerEneter(Collider collider)
        {
            var targert = collider.GetComponent<IApplyAbility>();

            if (targert != null)
            {
                var ability = AbilityFactory.CreateAbilityFromID(m_ApplyAbility.AbilityID);

                AbilityApplyData data = new AbilityApplyData()
                {
                    Caster = m_ServerCharacter
                };
                ability.Resolve(data);
                targert.ApplyAbility(ability);
            }
        }
    }

}