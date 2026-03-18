using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


namespace FQParty.GamePlay.Abilities.Effects
{
    public interface IEffect { }

    public interface IEffect<TTarget> : IEffect
    {
        void Apply(TTarget target);
    }

    public interface IServerEffect<TTarget> : IEffect<TTarget>
    { }

    public interface IClientEffect<TTarget> : IEffect<TTarget>
    { }

    public interface IOwnerEffect<TTarget> : IEffect<TTarget>
    { }
}