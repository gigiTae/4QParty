using Unity.Netcode;
using UnityEngine;


namespace FQParty.GamePlay.Character.Movement
{
    /// <summary>
    /// 캐릭터 이동 추상 클래스
    /// </summary>
   public abstract class CharacterMovement : NetworkBehaviour
    {
        public enum MovementState
        {
            Moveable, // 이동 가능 상태 
            Stop, // 정지
        }
        protected NetworkVariable<MovementState> m_State = new(MovementState.Moveable);

        [Rpc(SendTo.Server)]
        public virtual void SetMovementStateServerRpc(MovementState state)
        {
            m_State.Value = state;
        }
    }

}