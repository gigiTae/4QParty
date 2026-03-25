using FQParty.Common.Persistance;
using FQParty.GamePlay.Abilities;
using UnityEngine;

namespace FQParty.GamePlay.GameplayObjects
{
    public class GameDataManager : PersistanceSingleton<GameDataManager>
    {
        [SerializeField] AbilityList m_AbilityList;

        public Ability GetAbilityByID(AbilityID index)
        {
            if (index.ID < 0 || index.ID >= m_AbilityList.Abilities.Count)
            {
                Debug.LogError($"[GameDataSource] 유효하지 않은 AbilityID: {index.ID}");
                return null;
            }

            return m_AbilityList.Abilities[index.ID];
        }

        protected override void Awake()
        {
            base.Awake();

            BuildAbilityIDs();
        }

        void BuildAbilityIDs()
        {
            int id = 0;
            foreach (var ability in m_AbilityList.Abilities)
            {
                ability.AbilityID = new AbilityID { ID = id };
                id++;
            }
        }
    }
}