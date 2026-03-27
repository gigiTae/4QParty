using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FQParty.GamePlay.Input
{
    [CreateAssetMenu(fileName = "GamePlayInputReader", menuName = "Input/GamePlayInputReader")]
    public class GamePlayInputReader : ScriptableObject
    {
        public event Action InteractPerformedEvent;
        public event Action InteractCanceledEvent;
        public event Action DashPerformedEvent;
        public event Action DashCanceledEvent;
        public event Action AttackPerformedEvent;
        public event Action AttackCanceledEvent;

        private GamePlayInputAction m_GamePlayInputAction;

        void OnEnable()
        {
            if (m_GamePlayInputAction == null)
            {
                m_GamePlayInputAction = new GamePlayInputAction();
            }

            m_GamePlayInputAction.Enable();

            m_GamePlayInputAction.Player.Dash.performed += OnDashPerformed;
            m_GamePlayInputAction.Player.Dash.canceled += OnDashCanceled;

            m_GamePlayInputAction.Player.Attack.performed += OnAttackPerformed;
            m_GamePlayInputAction.Player.Attack.canceled += OnAttackCanceled;

            m_GamePlayInputAction.Player.Interact.performed += OnInteractPerformed;
            m_GamePlayInputAction.Player.Interact.canceled += OnInteractCanceled;
        }

        void OnDisable()
        {
            if (m_GamePlayInputAction != null)
            {
                // 이벤트 연결 해제 (메모리 누수 방지)
                m_GamePlayInputAction.Player.Dash.performed -= OnDashPerformed;
                m_GamePlayInputAction.Player.Dash.canceled -= OnDashCanceled;

                m_GamePlayInputAction.Player.Attack.performed -= OnAttackPerformed;
                m_GamePlayInputAction.Player.Attack.canceled -= OnAttackCanceled;

                m_GamePlayInputAction.Player.Interact.performed -= OnInteractPerformed;
                m_GamePlayInputAction.Player.Interact.canceled -= OnInteractCanceled;

                m_GamePlayInputAction.Disable();

                if (Application.isPlaying)
                {
                    m_GamePlayInputAction.Dispose();
                }

                m_GamePlayInputAction = null;
            }
        }

        public Vector2 PlayerMoveInput
        {
            get
            {
                // 안전한 접근을 위해 null 체크 추가
                if (m_GamePlayInputAction == null || !m_GamePlayInputAction.Player.enabled)
                {
                    return Vector2.zero;
                }
                return m_GamePlayInputAction.Player.Move.ReadValue<Vector2>();
            }
        }

        private void OnDashPerformed(InputAction.CallbackContext context) => DashPerformedEvent?.Invoke();
        private void OnDashCanceled(InputAction.CallbackContext context) => DashCanceledEvent?.Invoke();

        private void OnAttackPerformed(InputAction.CallbackContext context) => AttackPerformedEvent?.Invoke();
        private void OnAttackCanceled(InputAction.CallbackContext context) => AttackCanceledEvent?.Invoke();

        private void OnInteractPerformed(InputAction.CallbackContext context) => InteractPerformedEvent?.Invoke();
        private void OnInteractCanceled(InputAction.CallbackContext context) => InteractCanceledEvent?.Invoke();

        public Vector2 GetMouseDirection(Transform target)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector2 targetScreenPos = Camera.main.WorldToScreenPoint(target.position);

            Vector2 direction = (mouseScreenPos - targetScreenPos).normalized;
            return direction;
        }
    }
}