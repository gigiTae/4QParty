using FQParty.Common.DebugHelper;
using FQParty.GamePlay.Character;
using FQParty.GamePlay.GameplayObjects;
using System;
using System.Collections.Generic;
using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine;


namespace FQParty.GamePlay.Abilities.Effects
{
    [Serializable]
    public class AttackEffect : AbilityEffect
    {
        bool m_IsActive = false;
        public override bool IsActive => m_IsActive;

        [SerializeField] LayerMask m_LayerMask;
        [SerializeField] Vector3 m_Offset;
        [SerializeField] Vector3 m_HalfExtents;
        [SerializeField] float m_Damage;

        public override void OnStart(ServerCharacter serverCharacter, Ability ability)
        {
            var targets = DetectTarget(serverCharacter);

            if (targets == null) return;

            foreach (var target in targets)
            {
                target.ReceiveDamage(serverCharacter, m_Damage);
            }
        }

        IDamageable[] DetectTarget(ServerCharacter serverCharacter)
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

            var damageables = new List<IDamageable>();

            for (int i = 0; i < coliders.Length; i++)
            {
                IDamageable target = coliders[i].GetComponent<IDamageable>();

                if (target != null)
                {
                    damageables.Add(target);
                }
            }

            DebugGizumo.Instance.AddBox(center, m_HalfExtents * 2f, rotation,
                damageables.Count > 0 ? Color.red : Color.green, 0.1f);

            return damageables.Count > 0 ? damageables.ToArray() : null;
        }
    }

}