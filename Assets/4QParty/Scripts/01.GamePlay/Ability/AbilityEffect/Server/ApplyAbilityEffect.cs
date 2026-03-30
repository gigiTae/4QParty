using FQParty.Common.DebugHelper;
using FQParty.GamePlay.Character;
using FQParty.GamePlay.GameplayObjects;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace FQParty.GamePlay.Abilities.Effects
{
    [Serializable]
    public class ApplyAbilityEffect : ServerAbilityEffect
    {
        [Header("공격 콜라이더 설정")]
        [SerializeField] LayerMask m_LayerMask;
        [SerializeField] Vector3 m_Offset;
        [SerializeField] Vector3 m_HalfExtents;

        [Header("어빌리티 시전 설정")]
        [SerializeField] float m_FireTime;

        [Header("어빌리티 설정")]
        [Tooltip("타겟에게 부여하는 어빌리티(상대에게 대미지, 스턴, 넉백등)")]
        [SerializeField] Ability m_ApplyAbility;

        public override void OnStart(ServerCharacter serverCharacter, Ability ability)
        {
            if (m_FireTime <= 0f)
            {
                ApplyAbilityToTarget(serverCharacter);
                IsActive = false;
            }
        }

        public override void OnUpdate(ServerCharacter serverCharacter, Ability ability)
        {
            if (ability.TimeRunning >= m_FireTime)
            {
                ApplyAbilityToTarget(serverCharacter);
                IsActive = false;
            }
        }

        void ApplyAbilityToTarget(ServerCharacter serverCharacter)
        {
            var targets = DetectTarget(serverCharacter);

            if (targets == null) return;

            foreach (var target in targets)
            {
                var ability = AbilityFactory.CreateAbilityFromID(m_ApplyAbility.AbilityID);

                AbilityApplyData data = new AbilityApplyData()
                {
                   Caster = serverCharacter
                };
                ability.Resolve(data);
                target.ApplyAbility(ability);
            }
        }

        IApplyAbility[] DetectTarget(ServerCharacter serverCharacter)
        {
            Quaternion rotation = serverCharacter.transform.rotation;
            Vector3 rotatedOffset = rotation * m_Offset;
            Vector3 center = serverCharacter.transform.position + rotatedOffset;

            var coliders = Physics.OverlapBox(center, m_HalfExtents, rotation, m_LayerMask);

            if (coliders.Length == 0)
            {
                DebugGizumo.Instance.AddBox(center, m_HalfExtents * 2f, rotation, Color.green, 0.1f);
                return null;
            }

            var targetList = new List<IApplyAbility>();

            for (int i = 0; i < coliders.Length; i++)
            {
                IApplyAbility target = coliders[i].GetComponent<IApplyAbility>();

                if (target != null)
                {
                    targetList.Add(target);
                }
            }

            DebugGizumo.Instance.AddBox(center, m_HalfExtents * 2f, rotation,
                targetList.Count > 0 ? Color.red : Color.green, 0.1f);

            return targetList.Count > 0 ? targetList.ToArray() : null;
        }
    }

}