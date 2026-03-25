using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace FQParty.GamePlay.Input
{
    [CreateAssetMenu(fileName = "GamePlayInputReader", menuName = "Input/GamePlayInputReader")]
    public class GamePlayInputReader : ScriptableObject
    {
        public event Action OnInteractInput;
        public event Action OnDashInput;
        public event Action OnAttackInput;

        GamePlayInputAction m_GamePlayInputAction;
        void OnEnable()
        {
            m_GamePlayInputAction = new GamePlayInputAction();
            m_GamePlayInputAction.Enable();
            m_GamePlayInputAction.Player.Dash.performed += HandleDash;
            m_GamePlayInputAction.Player.Attack.performed += HandleAttack;
            m_GamePlayInputAction.Player.Interact.performed += HandleInteract;
        }

        void OnDisable()
        {
            if (m_GamePlayInputAction != null)
            {
                m_GamePlayInputAction.Player.Dash.performed -= HandleDash;
                m_GamePlayInputAction.Player.Attack.performed -= HandleAttack;
                m_GamePlayInputAction.Player.Interact.performed -= HandleInteract;
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
                if (!m_GamePlayInputAction.Player.enabled)
                {
                    return Vector2.zero;
                }
                return m_GamePlayInputAction.Player.Move.ReadValue<Vector2>();
            }
        }

        void HandleAttack(InputAction.CallbackContext context)
        {
            OnAttackInput?.Invoke();
        }
        void HandleDash(InputAction.CallbackContext context)
        {
            OnDashInput?.Invoke();
        }
        void HandleInteract(InputAction.CallbackContext context)
        {
            OnInteractInput?.Invoke();
        }
    }
}