using FQParty.GamePlay.Input;
using FQParty.GamePlay.Settings;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


namespace FQParty.GamePlay.Character.Movement
{
    /// <summary>
    /// 플레이어 캐릭터의 무브먼트
    /// </summary>
    [RequireComponent(typeof(CharacterController), typeof(ClientInputSender))]
    public class PlayerCharacterMovement : CharacterMovement, IDashable
    {
        public enum PlayerMovementState
        {
            Moveable,
            RotationOnly,
            Dash,
        }

        protected PlayerMovementState m_PlayerMovementState = PlayerMovementState.Moveable;

        public void BindSettings(IPlayerCharacterMovementSettings settings)
        {
            m_Settings = settings;  
        }
        IPlayerCharacterMovementSettings m_Settings;

        CharacterController m_CharacterController;
        ClientInputSender m_ClientInputSender;
        Camera m_PlayerCameara;


        void Awake()
        {
            if (m_PlayerCameara == null)
            {
                m_PlayerCameara = Camera.main;
            }

            m_CharacterController = GetComponent<CharacterController>();
            m_ClientInputSender = GetComponent<ClientInputSender>();
        }

        void Update()
        {
            if (!IsOwner) return;

            switch (MovementState)
            {
                case ServerMovementState.Moveable:
                    {
                        switch (m_PlayerMovementState)
                        {
                            case PlayerMovementState.Moveable:
                                UpdateInputMove();
                                break;
                            case PlayerMovementState.RotationOnly:
                                UpdateRotation();
                                break;
                            case PlayerMovementState.Dash:
                                UpdateDash();
                                break;
                        }
                        break;
                    }
                case ServerMovementState.Stop:
                    {
                        break;
                    }
                case ServerMovementState.Knockback:
                    {
                        break;
                    }
            }
        }

        void UpdateInputMove()
        {
            if (!m_Settings.UseInputMove) return;   

            Vector2 moveInput = m_Settings.GamePlayInputReader.PlayerMoveInput;
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

        void UpdateRotation()
        {
            Vector2 directionXZ = m_ClientInputSender.GetDirectionInput();
            Vector3 direction = new() { x = directionXZ.x, z = directionXZ.y };

            if (directionXZ.sqrMagnitude > 0.01f)
            {
                Vector3 cameraRight = m_PlayerCameara.transform.right;
                Vector3 cameraForward = m_PlayerCameara.transform.forward;

                cameraForward.y = 0;
                cameraForward.Normalize();
                cameraRight.y = 0;
                cameraRight.Normalize();

                Vector3 targetDirection = (cameraRight * directionXZ.x + cameraForward * directionXZ.y).normalized;

                if (targetDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    transform.rotation = targetRotation;
                }
            }
        }
        
        Vector3 m_DashDirection;
        float m_CurrentDashSpeed = 0f;
        
        public void StartDash(float speed)
        {
            m_PlayerMovementState = PlayerMovementState.Dash;
            m_CurrentDashSpeed = speed;
            m_DashDirection = transform.forward;
        }

        void UpdateDash()
        {
            Vector3 motion = m_DashDirection * m_CurrentDashSpeed * Time.deltaTime;
            m_CharacterController.Move(motion);
        }

        public void CancelDash()
        {
            m_PlayerMovementState = PlayerMovementState.Moveable;
            m_CurrentDashSpeed = 0f;
        }

        public void LockMovement()
        {
            m_PlayerMovementState = PlayerMovementState.RotationOnly;
        }
        public void UnlockMovement()
        {
            m_PlayerMovementState = PlayerMovementState.Moveable;
        }

    }
}
