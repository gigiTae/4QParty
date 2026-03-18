using Codice.Client.BaseCommands;
using System;
using UnityEngine;


namespace FQParty.GamePlay.Abilities.Effects
{

    public interface IMobile
    {
        void Dash();
    }

    public interface IApplyEffect<T> 
    {
        public void ApplyEffect(IEffect<T> effect);
    }

    [Serializable]
    public class DashEffect : IOwnerEffect<IMobile>
    {
        public float DashSpeed;
        public float DashDuration;  

        public void Apply(IMobile target)
        {
            target.Dash();
        }
    }
}