using System.Collections.Generic;
using UnityEngine;

namespace FQParty.GamePlay.Abilities
{
    [CreateAssetMenu(fileName = "AbilityDatabase", menuName = "Abilities/AbilityDatabase")]
    public class AbilityDatabase : ScriptableObject
    {
        public List<AbilityData> AbilityDataList;
        public AbilityData Find(ulong id)
        {
            foreach (AbilityData ability in AbilityDataList)
            {
                if (ability.AbilityID == id)
                    return ability;
            }
            return null;
        }
    }

}