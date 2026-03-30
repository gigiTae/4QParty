using FQParty.GamePlay.Cam;
using FQParty.GamePlay.Character.Movement;
using FQParty.GamePlay.Input;
using FQParty.GamePlay.Settings;
using UnityEngine;



namespace FQParty.GamePlay.Character
{
    public class PlayerServerCharacter : ServerCharacter
    {
        [SerializeField] PlayerCharacterSettings m_Settings;
        [SerializeField] ClientInputSender m_ClientInputSender;

        PlayerCamera m_PlayerCamera;

        private void Awake()
        {
            BindSettings();
        }

        void BindSettings()
        {
            if (CharacterMovement is PlayerCharacterMovement playerCharacterMovement)
            {
                playerCharacterMovement.BindSettings(m_Settings);
            }

            if (CharacterStatus != null)
            {
                CharacterStatus.BindSettings(m_Settings);
            }

            if(m_ClientInputSender != null)
            {
                m_ClientInputSender.BindSettings(m_Settings);
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                SetPlayerCamera();
            }
        }

        void SetPlayerCamera()
        {
            m_PlayerCamera = FindFirstObjectByType<PlayerCamera>();
            m_PlayerCamera.SetTarget(transform);
        }
    }
}