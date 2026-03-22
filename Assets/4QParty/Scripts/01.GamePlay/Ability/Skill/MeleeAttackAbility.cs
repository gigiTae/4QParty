using FQParty.GamePlay.Character;
using FQParty.GamePlay.GameplayObjects;
using UnityEngine;
using System.Collections.Generic;
using Unity.Plastic.Antlr3.Runtime;

namespace FQParty.GamePlay.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/MeleeAttack")]
    public class MeleeAttackAbility : Ability
    {
        [Header("충돌체 설정")]
        [SerializeField] LayerMask m_LayerMask;
        [SerializeField] Vector3 m_Offset;
        [SerializeField] Vector3 m_HalfExtents;

        [Header("Effect")]
        [SerializeField] float m_Damage;

        public override bool OnStart(ServerCharacter serverCharacter)
        {
            serverCharacter.CharacterMovement.SetMovementStateServerRpc(CharacterMovement.MovementState.Stop);

            var targets = DetectTarget(serverCharacter);

            foreach (var target in targets)
            {
                target.ReceiveDamage(serverCharacter, m_Damage);
            }

            return AbilityConclusion.Stop;
        }

        public override bool OnUpdate(ServerCharacter serverCharacter)
        {
            return AbilityConclusion.Stop;
        }

        public override void Cancel(ServerCharacter serverCharacter)
        {
            Debug.Log("Cancel");
            serverCharacter.CharacterMovement.SetMovementStateServerRpc(CharacterMovement.MovementState.Moveable);
        }

        IDamageable[] DetectTarget(ServerCharacter serverCharacter)
        {
            Vector3 center = serverCharacter.transform.position + m_Offset;
            Vector3 foward = serverCharacter.transform.forward;
            Quaternion rotation = serverCharacter.transform.rotation;

            var coliders = Physics.OverlapBox(center, m_HalfExtents, rotation, m_LayerMask);

            if (coliders.Length == 0)
            {
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
            return damageables.Count > 0 ? damageables.ToArray() : null;
        }

    }

}