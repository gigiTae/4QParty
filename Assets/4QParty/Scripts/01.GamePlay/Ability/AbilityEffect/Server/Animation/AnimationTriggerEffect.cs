using FQParty.GamePlay.Character;
using System;
using UnityEditor.Animations;
using UnityEngine;


namespace FQParty.GamePlay.Abilities.Effects
{
    [Serializable]
    public class AnimationTriggerEffect : ServerAbilityEffect
    {
        [SerializeField] string m_TriggerName;
        [SerializeField] string m_StateName;
        public override void OnStart(ServerCharacter serverCharacter, Ability ability)
        {
            serverCharacter.NetworkAnimator.SetTrigger(m_TriggerName);
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