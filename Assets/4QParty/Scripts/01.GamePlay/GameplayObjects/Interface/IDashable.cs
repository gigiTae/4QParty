using UnityEngine;

namespace FQParty.GamePlay.Character.Movement
{
    public interface IDashable
    {
        void StartDash(float speed);
        void CancelDash();
    }
}