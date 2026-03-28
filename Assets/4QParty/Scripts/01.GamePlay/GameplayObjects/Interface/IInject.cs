using UnityEngine;


namespace FQParty.GamePlay
{
    public interface IInject<TData>
    {
       void Inject(TData data);
    }

    public interface IResolver
    {
        void Resolve<TData>(TData data);
    }
}