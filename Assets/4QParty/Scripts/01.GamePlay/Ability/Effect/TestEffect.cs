using System;
using UnityEngine;


namespace FQParty.GamePlay.Abilities.Effects
{
    [Serializable]
    public class TestEffect : IServerEffect<IMobile>
    {
        public string Text;

        public void Apply(IMobile _)
        {

        }
    }
}