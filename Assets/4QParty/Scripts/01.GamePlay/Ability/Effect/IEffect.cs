using System;
using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine;
using UnityEngine.UI;


namespace FQParty.GamePlay.Abilities.Effects
{
    public enum EffectNetType
    {
        OnlyOwner,
        Server,
        Client 
    }

    public interface IEffect 
    {
        public EffectNetType NetType { get; }
    }

    public abstract class IEffect<TTarget> : IEffect
    {
        public virtual EffectNetType NetType { get; }
        public abstract bool IsExpired { get; }

        public abstract void Start(TTarget target);

        public virtual void Update(TTarget target) { }

        public virtual void Cancel(TTarget target) { }
    }

    public interface IApplyEffect<T>
    {
        public void ApplyEffect(IEffect<T> effect);
    }

}