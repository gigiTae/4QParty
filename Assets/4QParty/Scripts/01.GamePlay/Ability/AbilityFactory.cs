using FQParty.GamePlay.GameplayObjects;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace FQParty.GamePlay.Abilities
{
    public static class AbilityFactory
    {
        private static Dictionary<AbilityID, ObjectPool<Ability>> s_AbilityPools = new();

        private static ObjectPool<Ability> GetAbilityPool(AbilityID abilityID)
        {
            if (!s_AbilityPools.TryGetValue(abilityID, out var abilityPool))
            {
                abilityPool = new ObjectPool<Ability>(
                    createFunc: () => Object.Instantiate(GameDataSource.Instance.GetAbilityPrototypeByID(abilityID)),
                    actionOnRelease: ability => ability.Reset(),
                    actionOnDestroy: Object.Destroy);
            }

            return abilityPool; 
        }

        public static Ability CreateAbilityFromData(ref AbilityRequestData data)
        {
            var ret = GetAbilityPool(data.AbilityID).Get();
            ret.Initialize(ref data);   
            return ret; 
        }

        public static void ReturnAbility(Ability ability)
        {
            var pool = GetAbilityPool(ability.AbilityID);
            pool.Release(ability);
        }

        public static void PurgePooledAbilitys()
        {
            foreach (var abilityPool in s_AbilityPools.Values)
            {
                abilityPool.Clear();
            }
        }
    }
}
