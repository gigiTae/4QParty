using FQParty.GamePlay.Character;
using System;
using UnityEngine;


namespace FQParty.GamePlay.Abilities.Effects
{
    [Serializable]
    public class AnimationEffect : AbilityEffect
    {
        [SerializeField] public string Animation;

        public override void OnStart(ServerCharacter serverCharacter, Ability ability)
        {
            serverCharacter.NetworkAnimator.SetTrigger(Animation);
        }

        public override void OnUpdate(ServerCharacter serverCharacter, Ability ability)
        {
        }
    }

}