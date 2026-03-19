using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace FQParty.GamePlay.Input
{
    [CreateAssetMenu(fileName = "GamePlayInputReader", menuName = "Input/GamePlayInputReader")]
    public class GamePlayInputReader : ScriptableObject
    {
        public event Action OnDashInput;

        GamePlayInputAction m_GamePlayInputAction;
        void OnEnable()
        {
            m_GamePlayInputAction = new GamePlayInputAction();
            m_GamePlayInputAction.Enable();

            m_GamePlayInputAction.Player.Dash.performed += HandleDash;
        }

        void OnDisable()
        {
            m_GamePlayInputAction.Player.Dash.performed -= HandleDash;

            m_GamePlayInputAction.Disable();
            m_GamePlayInputAction.Dispose();
            m_GamePlayInputAction = null;
        }

        public Vector2 PlayerMoveInput
        {
            get
            {
                if(!m_GamePlayInputAction.Player.enabled)
                {
                    return Vector2.zero;    
                }
                return m_GamePlayInputAction.Player.Move.ReadValue<Vector2>();
            }
        }

        void HandleDash(InputAction.CallbackContext context)
        {
            OnDashInput.Invoke();
        }
    }
}