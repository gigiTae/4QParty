using FQParty.GamePlay.Character;
using FQParty.GamePlay.GameplayObjects;
using System;
using Unity.VectorGraphics.Editor;
using UnityEngine;


namespace FQParty.GamePlay.Abilities.Effects
{
    [Serializable]
    public class InteractEffect : ServerAbilityEffect
    {
        [SerializeField] float m_DetectRadius = 1f;
        [SerializeField] LayerMask m_LayerMask;

        public override void OnStart(ServerCharacter serverCharacter, Ability ability)
        {
            var target = DetectTarget(serverCharacter);

            if (target != null)
            {
                target.Interact(serverCharacter);
            }

            IsActive = false;
        }

        IInteractable DetectTarget(ServerCharacter serverCharacter)
        {
            Vector3 position = serverCharacter.transform.position;
            var coliders = Physics.OverlapSphere(position, m_DetectRadius, m_LayerMask);

            if (coliders.Length == 0) return null;

            for (int i = 0; i < coliders.Length; i++)
            {
                IInteractable target = coliders[i].GetComponent<IInteractable>();
                if (target != null)
                {
                    return target;
                }
            }

            return null;
        }

    }

}