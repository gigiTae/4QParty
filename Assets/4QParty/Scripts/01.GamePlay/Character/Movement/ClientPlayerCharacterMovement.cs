using FQParty.GamePlay.Input;
using FQParty.GamePlay.Settings;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


namespace FQParty.GamePlay.Character.Movement
{
    /// <summary>
    /// 클라이언트 권한 Movement
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class ClientPlayerCharacterMovement : CharacterMovement, IDashable
    {
        [SerializeField] CharacterSettings m_Settings;
        [SerializeField] GamePlayInputReader m_GamePlayInputReader;
        [SerializeField] CharacterController m_CharacterController;
        [SerializeField] Camera m_PlayerCameara;

        public override void OnNetworkSpawn()
        {
        }
        public override void OnNetworkDespawn()
        {
        }

  
        void Awake()
        {
            if (m_PlayerCameara == null)
            {
                m_PlayerCameara = Camera.main;
            }
        }

        void Update()
        {
            if (!IsOwner) return;

            switch (m_State.Value)
            {
                case MovementState.Moveable:
                    {
                        if(m_OnDash)
                        {
                            UpdateDash();
                        }
                        else
                        {
                            UpdateInputMove();
                        }
                        break;
                    }
                case MovementState.Stop:
                    {
                        break;
                    }
            }
        }
        
        void UpdateInputMove()
        {
            Vector2 moveInput = m_GamePlayInputReader.PlayerMoveInput;
            Vector3 moveDirection = Vector3.zero;

            if (moveInput.sqrMagnitude > 0.01f)
            {
                Vector3 right = m_PlayerCameara.transform.right;
                Vector3 foward = m_PlayerCameara.transform.forward;

                foward.y = 0;
                foward.Normalize();
                right.y = 0;
                right.Normalize();
                moveDirection = (right * moveInput.x + foward * moveInput.y).normalized;

                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = targetRotation;
            }

            Vector3 motion = moveDirection * m_Settings.MoveSpeed * Time.deltaTime;
            m_CharacterController.Move(motion);
        }

        Vector3 m_DashDirection;
        float m_CurrentDashSpeed = 0f;
        float m_DashDuration;
        float m_StartDashTime;
        bool m_OnDash = false;

        public void StartDash(float speed, float duration)
        {
            m_DashDuration = duration;  
            m_OnDash = true;
            m_CurrentDashSpeed = speed;
            m_DashDirection = transform.forward;
            m_StartDashTime = Time.time;
        }
        
        void UpdateDash()
        {
            Vector3 motion = m_DashDirection * m_CurrentDashSpeed * Time.deltaTime;
            m_CharacterController.Move(motion);

            if (Time.time - m_StartDashTime > m_DashDuration)
            {
                m_OnDash = false;
            }   
        }

        public void CancelDash()
        {
            m_OnDash = false;
        }
    }
}
