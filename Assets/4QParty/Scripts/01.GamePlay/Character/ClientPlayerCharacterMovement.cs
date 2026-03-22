using FQParty.GamePlay.Input;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


namespace FQParty.GamePlay.Character
{
    /// <summary>
    /// 클라이언트 권한 Movement
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class ClientPlayerCharacterMovement : CharacterMovement
    {
        [SerializeField] CharacterSettings m_Settings;
        [SerializeField] GamePlayInputReader m_GamePlayInputReader;
        [SerializeField] CharacterController m_CharacterController;
        [SerializeField] Camera m_PlayerCameara;
        [SerializeField] InputActionReference m_MoveInputActionReference;

        public override void OnNetworkSpawn()
        {
        }
        public override void OnNetworkDespawn()
        {
        }


        Vector3 m_DashDirection;
        float m_CurrentDashSpeed = 0f;
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
                    UpdateInputMove();
                    break;
                case MovementState.Stop:
                    break;
            }
        }

        void UpdateDash()
        {
            Vector3 motion = m_DashDirection * m_CurrentDashSpeed * Time.deltaTime;
            m_CharacterController.Move(motion);
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
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1.0f);
            }

            Vector3 motion = moveDirection * m_Settings.MoveSpeed * Time.deltaTime;
            m_CharacterController.Move(motion);
        }

        public void StartDash(float speed, float duration)
        {
            m_CurrentDashSpeed = speed;
            m_DashDirection = transform.forward;
        }

    }
}
