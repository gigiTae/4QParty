using FQParty.GamePlay.Cam;
using UnityEngine;



namespace FQParty.GamePlay.Character
{
    public class PlayerServerCharacter : ServerCharacter
    {
        PlayerCamera m_PlayerCamera;

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