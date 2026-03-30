using FQParty.GamePlay.GameplayObjects;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace FQParty.GamePlay.Character.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class MonsterCharacterMovement : CharacterMovement, IKnockbackable
    {
        CharacterController m_CharacterController;

        Vector3 m_KnockbackDirection;
        float m_KnockbackSpeed = 0;

        void Awake()
        {
            m_CharacterController = GetComponent<CharacterController>();
        }

        void Update()
        {
            if (MovementState == ServerMovementState.Knockback)
            {
                Vector3 motion = m_KnockbackDirection * Time.deltaTime * m_KnockbackSpeed;
                m_CharacterController.Move(motion);
            }
        }

        public void ApplyKnockback(float speed, Vector3 direction)
        {
            if (!IsServer) return;

            MovementState = ServerMovementState.Knockback;
            m_KnockbackSpeed = speed;
            m_KnockbackDirection = direction;
        }

        public void CancelKnockback()
        {
            if (!IsServer) return;

            MovementState = ServerMovementState.Moveable;
            m_KnockbackDirection = Vector3.zero;
            m_KnockbackSpeed = 0f;
        }
    }
}