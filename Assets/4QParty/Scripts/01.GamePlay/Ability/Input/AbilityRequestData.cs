using Codice.CM.Common.Merge;
using System;
using Unity.Netcode;
using UnityEngine;

namespace FQParty.GamePlay.Abilities
{
    public struct AbilityRequestData : INetworkSerializable
    {
        public AbilityID AbilityID;      

        [Flags]
        private enum PackFlags
        {
            None = 0,
        }

        public static AbilityRequestData Create(Ability ability) =>
            new()
            {
                AbilityID = ability.AbilityID
            };

        private PackFlags GetPackFlags()
        {
            PackFlags flags = PackFlags.None;


            return flags;
        }


        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            PackFlags flags = PackFlags.None;
            if(!serializer.IsReader)
            {
                flags = GetPackFlags(); 
            }

            serializer.SerializeValue(ref AbilityID);
        }
    }

}