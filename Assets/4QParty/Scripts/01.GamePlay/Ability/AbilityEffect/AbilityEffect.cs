using FQParty.GamePlay.Character;
using System;
using UnityEngine;


namespace FQParty.GamePlay.Abilities.Effects
{
    [Serializable]
    public abstract class AbilityEffect
    {
        public virtual void OnStart(ServerCharacter serverCharacter, Ability ability) {}
        public virtual void OnUpdate(ServerCharacter serverCharacter, Ability ability) {}

        public virtual void Cancel(ServerCharacter serverCharacter, Ability ability) {}
        public virtual void End(ServerCharacter serverCharacter, Ability ability) {}
    }

}