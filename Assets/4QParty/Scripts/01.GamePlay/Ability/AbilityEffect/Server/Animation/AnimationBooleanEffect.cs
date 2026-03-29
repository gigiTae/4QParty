using FQParty.GamePlay.Character;
using System;
using UnityEngine;


namespace FQParty.GamePlay.Abilities.Effects
{
    [Serializable]
    public class AnimationBooleanEffect : ServerAbilityEffect
    {
        [SerializeField] string m_BoolName;
        [SerializeField] bool m_Value;
        [SerializeField] string m_StateName;
        public override void OnStart(ServerCharacter serverCharacter, Ability ability)
        {
            serverCharacter.NetworkAnimator.Animator.SetBool(m_BoolName, m_Value);
        }

        public override void OnAnimationStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsName(m_StateName))
            {
                IsActive = false;
            }
        }
    }
}