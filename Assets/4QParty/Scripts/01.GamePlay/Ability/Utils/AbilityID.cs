using System;
using Unity.Netcode;
using UnityEngine;


namespace FQParty.GamePlay.Abilities
{
    public struct AbilityID : INetworkSerializeByMemcpy, IEquatable<AbilityID>
    {
        public int ID;

        public bool Equals(AbilityID other)
        {
            return ID == other.ID;  
        }

        public override bool Equals(object obj)
        {
            return obj is AbilityID other && Equals(other);
        }

        public override int GetHashCode()
        {
            return ID;
        }

        public static bool operator ==(AbilityID x, AbilityID y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(AbilityID x, AbilityID y)
        {
            return !(x == y);
        }
        public override string ToString()
        {
            return $"AbilityID({ID})";
        }
    }

}