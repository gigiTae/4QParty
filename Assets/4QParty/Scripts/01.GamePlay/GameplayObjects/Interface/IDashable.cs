using UnityEngine;

namespace FQParty.GamePlay.Character.Movement
{
    public interface IDashable
    {
        /// <summary>
        // 대쉬 시작
        /// </summary>
        void StartDash(float speed, float duration);

        /// <summary>
        /// 대쉬 취소
        /// </summary>
        void CancelDash();
    }
}