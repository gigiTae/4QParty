using FQParty.GamePlay.Input;
using Unity.Netcode;
using UnityEngine;


namespace FQParty.GamePlay.Character
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMovement : NetworkBehaviour
    {
        [SerializeField] CharacterSettingsSO m_Settings;
        [SerializeField] GamePlayInputReader m_GamePlayInputReader;
        [SerializeField] CharacterController m_CharacterController;
        [SerializeField] Camera m_PlayerCameara;

        void Awake()
        {
            if (m_CharacterController == null)
            {
                m_CharacterController = GetComponent<CharacterController>();
            }

            if(m_PlayerCameara == null)
            {
                m_PlayerCameara = Camera.main;
            }
        }
        public override void OnDestroy()
        {
            base.OnDestroy();

            if (m_GamePlayInputReader != null)
            {
                m_GamePlayInputReader.OnPlayerDashPerfomed -= OnDashInput;
            }
        }

        void OnDashInput()
        {
            Debug.Log("Dash!");
        }

        void Update()
        {
            if (!IsOwner) return;

            ApplyInput();
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
    }
}
