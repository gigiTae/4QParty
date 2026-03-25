using FQParty.GamePlay.Character;
using System;
using UnityEditor.Animations;
using UnityEngine;


namespace FQParty.GamePlay.Abilities.Effects
{
    [Serializable]
    public class AnimationEffect : AbilityEffect
    {
        [SerializeField] public string Animation;
        [SerializeField] public string StateName;
        public override bool IsActive => m_IsActive;
        private bool m_IsActive;

        public override void OnStart(ServerCharacter serverCharacter, Ability ability)
        {
            this.GetType();
            m_IsActive = true;
            serverCharacter.NetworkAnimator.SetTrigger(Animation);
        }

        public override void OnUpdate(ServerCharacter serverCharacter, Ability ability)
        {   
        }
        public override void OnAnimationStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsName(StateName))
            {
                m_IsActive = false;
            }
        }
    }
}