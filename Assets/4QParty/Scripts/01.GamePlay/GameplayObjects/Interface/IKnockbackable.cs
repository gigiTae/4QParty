using UnityEngine;


namespace FQParty.GamePlay.GameplayObjects
{
    public interface IKnockbackable
    {
        void ApplyKnockback(float speed);

        void CancelKnockback();
    }
}