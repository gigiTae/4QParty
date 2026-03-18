

using FQParty.Common.Persistance;
using System.Collections.Generic;

namespace FQParty.GamePlay.Abilities
{
    public class AbilityDatabase : PersistanceSingleton<AbilityDatabase>
    {
        Dictionary<int, AbilityData> m_AbilityDatabase;

        protected override void Awake()
        {
            base.Awake();

        }




    }

}