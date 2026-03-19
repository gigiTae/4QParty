using FQParty.GamePlay.Abilities.Effects;
using UnityEngine;
using System.Collections.Generic;
using FQParty.GamePlay.Character;

namespace FQParty.GamePlay.Abilities
{
    public class AbilityEffectHandler : MonoBehaviour, IApplyEffect<IDashable>
    {
        private List<IEffect<IDashable>> m_ActiveEffects = new();

        [SerializeField]
        private ClientCharacterMovement m_Movement;

        public void ApplyEffect(IEffect<IDashable> effect)
        {
            m_ActiveEffects.Add(effect);    
            effect.Start(m_Movement);
        }

        void Update()
        {
            for (int i = m_ActiveEffects.Count - 1; i >= 0; i--)
            {
                var effect = m_ActiveEffects[i];
                effect.Update(m_Movement);

                if(effect.IsExpired)
                {
                    effect.Cancel(m_Movement);
                    m_ActiveEffects.RemoveAt(i);
                }
            }
        }
    }

}