using UnityEngine;
using UnityEngine.PlayerLoop;


namespace FQParty.GamePlay.Abilities.Targeting
{

    public abstract class TargetingStrategy
    {
        protected AbilityData m_AbilityData;
        protected AbilityCaster m_Caster;
        protected bool m_IsTargeting;
        public abstract void Start(AbilityData abilityData, AbilityCaster abilityCaster);
        public virtual void Update() { }
        public virtual void Cancel() { }
    }

}