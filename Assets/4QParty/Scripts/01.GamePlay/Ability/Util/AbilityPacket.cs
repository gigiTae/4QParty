using JetBrains.Annotations;
using System;
using Unity.Netcode;
using UnityEngine;


namespace FQParty.GamePlay.Abilities
{
    public struct AbilityPacket : INetworkSerializable
    {
        public ulong AbilityID;

        [Flags]
        private enum PackFlags
        {
            None = 0,
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref AbilityID);

        }
    }

}