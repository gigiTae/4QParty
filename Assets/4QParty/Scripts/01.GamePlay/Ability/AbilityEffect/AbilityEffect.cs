using FQParty.GamePlay.Character;
using System;
using UnityEngine;


namespace FQParty.GamePlay.Abilities.Effects
{
    [Serializable]
    public abstract class AbilityEffect
    {
        public virtual void OnStart(ServerCharacter serverCharacter, Ability ability) { }
        public virtual void OnUpdate(ServerCharacter serverCharacter, Ability ability) { }

        public virtual void Cancel(ServerCharacter serverCharacter, Ability ability) { }
        public virtual void End(ServerCharacter serverCharacter, Ability ability) { }

        public virtual void OnAnimationStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        public abstract bool IsActive { get; }

        public virtual AbilityEffect Clone()
        {
            return (AbilityEffect)MemberwiseClone();
        }
    }
}