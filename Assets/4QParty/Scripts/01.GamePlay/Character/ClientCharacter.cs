using FQParty.GamePlay.Abilities;
using FQParty.GamePlay.Abilities.AbilityCaster;
using FQParty.GamePlay.Input;
using Unity.Netcode;
using UnityEngine;


namespace FQParty.GamePlay.Character
{
    public class ClientCharacter : NetworkBehaviour
    {
        [SerializeField]
        ServerCharacter m_ServerCharacter;

        [SerializeField]
        ClientCharacterMovement m_ClientCharacterMovement;

        [SerializeField]
        ClientAbilityCaster m_ClientAbilityCaster;

        [SerializeField]
        GamePlayInputReader m_GamePlayInputReader;

        AbilityData m_DashAbility;

        public void Awake()
        {
            if (m_GamePlayInputReader)
            {
                m_GamePlayInputReader.OnPlayerDashPerfomed += CastAbility;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            m_GamePlayInputReader.OnPlayerDashPerfomed -= CastAbility;
        }

        void CastAbility()
        {
            if (IsOwner)
            {
                m_ClientAbilityCaster.CastAbility(m_DashAbility);
                m_ServerCharacter.ServerCastAbilityRpc();
            }
        }


    }

}