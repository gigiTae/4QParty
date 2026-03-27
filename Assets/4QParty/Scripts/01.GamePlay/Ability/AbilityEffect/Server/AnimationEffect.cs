using FQParty.GamePlay.Character;
using System;
using UnityEditor.Animations;
using UnityEngine;


namespace FQParty.GamePlay.Abilities.Effects
{
    [Serializable]
    public class AnimationEffect : ServerAbilityEffect
    {
        [SerializeField] public string Animation;
        [SerializeField] public string StateName;

        public override void OnStart(ServerCharacter serverCharacter, Ability ability)
        {
            serverCharacter.NetworkAnimator.SetTrigger(Animation);
        }

        public override void OnUpdate(ServerCharacter serverCharacter, Ability ability)
        {   
        }
        public override void OnAnimationStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsName(StateName))
            {
                IsActive = false;
            }
        }
    }
}