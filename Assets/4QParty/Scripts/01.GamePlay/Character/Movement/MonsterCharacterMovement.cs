using FQParty.GamePlay.GameplayObjects;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace FQParty.GamePlay.Character.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class MonsterCharacterMovement : CharacterMovement, IKnockbackable
    {
        CharacterController m_CharacterController;

        bool m_OnKnockback = false;
        Vector3 m_KnockbackDirection;
        float m_KnockbackSpeed = 0;

        void Awake()
        {
            m_CharacterController = GetComponent<CharacterController>();
        }

        void Update()
        {
            if(m_OnKnockback)
            {
                Vector3 motion = m_KnockbackDirection * Time.deltaTime * m_KnockbackSpeed;
                m_CharacterController.Move(motion);
            }
        }

        public void ApplyKnockback(float speed, Vector3 direction)
        {
            m_OnKnockback = true;
            m_KnockbackSpeed = speed;
            m_KnockbackDirection = direction;
        }

        public void CancelKnockback()
        {
            m_KnockbackDirection = Vector3.zero;
            m_KnockbackSpeed = 0f;
            m_OnKnockback = false;
        }


    }

}