using FQParty.GamePlay.Character;
using System;
using UnityEngine;


namespace FQParty.GamePlay.Abilities.Effects
{
    [Serializable]
    public abstract class ClientAbilityEffect
    {
        public virtual void OnStart(ClientCharacter clientCharacter, Ability ability) { }
        public virtual void OnUpdate(ClientCharacter clientCharacter, Ability ability) { }

        public virtual void Cancel(ClientCharacter clientCharacter, Ability ability) { }
        public virtual void End(ClientCharacter clientCharacter, Ability ability) { }

        public bool IsActive { get; set; }

        public virtual ClientAbilityEffect Clone()
        {
            return (ClientAbilityEffect)MemberwiseClone();
        }
    }
}