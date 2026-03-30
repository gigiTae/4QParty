using FQParty.GamePlay.GameplayObjects;
using Unity.Netcode;
using UnityEngine;


namespace FQParty.GamePlay.Character.Movement
{
    /// <summary>
    /// 캐릭터 이동 추상 클래스
    /// </summary>
    public abstract class CharacterMovement : NetworkBehaviour
    {
        /// <summary>
        /// 서버 권한으로 무브먼트를 설정합니다
        /// </summary>
        public enum ServerMovementState
        {
            Moveable,  // 이동
            Stop,      // 정지
            Knockback, // 넉백
        }

        public ServerMovementState MovementState
        {
            get => m_MovementState.Value;
            set
            {
                if (!IsServer) return;
                m_MovementState.Value = value;  
            }
        }
        private NetworkVariable<ServerMovementState> m_MovementState = new(ServerMovementState.Moveable);
    }

}