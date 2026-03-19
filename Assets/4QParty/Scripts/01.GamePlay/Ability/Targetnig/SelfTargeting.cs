using Codice.Client.BaseCommands;
using FQParty.GamePlay.Abilities.Effects;
using UnityEngine;

namespace FQParty.GamePlay.Abilities.Targeting
{
    public class SelfTargeting : TargetingStrategy
    {
        public override void Start(AbilityData abilityData, AbilityCaster caster)
        {
            if(caster.TryGetComponent<IApplyEffect<IDashable>>(out var target))
            {
            }
        }
    }

}