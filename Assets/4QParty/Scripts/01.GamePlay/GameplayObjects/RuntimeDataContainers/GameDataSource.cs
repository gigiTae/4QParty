using FQParty.Common.Persistance;
using FQParty.GamePlay.Abilities;
using UnityEngine;
using System.Collections.Generic;

namespace FQParty.GamePlay.GameplayObjects
{
    public class GameDataSource : PersistanceSingleton<GameDataSource>
    {
        [SerializeField] Ability[] m_AbilityPrototypes;

        List<Ability> m_AllAbilities;

        public Ability GetAbilityPrototypeByID(AbilityID index)
        {
            return m_AllAbilities[index.ID];
        }

        protected override void Awake()
        {
            base.Awake();

            BuildAbilityIDs();
        }

        void BuildAbilityIDs()
        {
            var uniqueAbilities = new HashSet<Ability>(m_AbilityPrototypes);

            m_AllAbilities = new List<Ability>(uniqueAbilities.Count);

            int id = 0;

            foreach (var ability in uniqueAbilities)
            {
                ability.AbilityID = new AbilityID { ID = id };
                m_AllAbilities.Add(ability);
                id++;
            }

        }
    }

}