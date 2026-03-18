using FQParty.GamePlay.Abilities.Effects;
using UnityEngine;
using System.Collections.Generic;

namespace FQParty.GamePlay.Abilities
{
    [CreateAssetMenu(fileName = "AbilityData", menuName = "Abilities/AbilityData")]
    public class AbilityData : ScriptableObject
    {
        [SerializeReference]  public List<IEffect> EffectList;
    }

}