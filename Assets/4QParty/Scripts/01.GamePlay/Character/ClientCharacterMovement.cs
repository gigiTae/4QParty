using FQParty.GamePlay.Abilities.Effects;
using FQParty.GamePlay.Input;
using Unity.Netcode;
using UnityEngine;


namespace FQParty.GamePlay.Character
{
    enum MoveState
    {
        Default,
        Dash,
    }

    [RequireComponent(typeof(CharacterController))]
    public class ClientCharacterMovement : NetworkBehaviour, IDashable
    {
        [SerializeField] CharacterSettingsSO m_Settings;
        [SerializeField] GamePlayInputReader m_GamePlayInputReader;
        [SerializeField] CharacterController m_CharacterController;
        [SerializeField] Camera m_PlayerCameara;

        MoveState m_MoveState = MoveState.Default;
        Vector3 m_DashDirection;
        float m_CurrentDashSpeed = 0f;


        void Awake()
        {
            if (m_CharacterController == null)
            {
                m_CharacterController = GetComponent<CharacterController>();
            }

            if (m_PlayerCameara == null)
            {
                m_PlayerCameara = Camera.main;
            }
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        void Update()
        {
            if (!IsOwner) return;

            if (m_MoveState == MoveState.Default)
            {
                ApplyInput();
            }
        }

        void ApplyInput()
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
            m_MoveState = MoveState.Dash;
            m_CurrentDashSpeed = speed;
            m_DashDirection = transform.forward;
        }

        public void CancelDash()
        {
            Debug.Log("CancelDash");
            m_MoveState = MoveState.Default;
            m_CurrentDashSpeed = 0;
        }

        public void UpdateDash()
        {
            Vector3 motion = m_DashDirection * m_CurrentDashSpeed * Time.deltaTime;
            m_CharacterController.Move(motion);
        }
    }
}
