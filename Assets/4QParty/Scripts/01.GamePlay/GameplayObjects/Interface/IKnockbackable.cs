using UnityEngine;


namespace FQParty.GamePlay.GameplayObjects
{
    public interface IKnockbackable
    {
        void ApplyKnockback(float speed, Vector3 direction);
        void CancelKnockback();
    }
}