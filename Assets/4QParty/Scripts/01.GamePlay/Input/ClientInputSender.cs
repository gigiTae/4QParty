using FQParty.GamePlay.Abilities;
using FQParty.GamePlay.Abilities.Effects;
using FQParty.GamePlay.Character;
using Unity.Netcode;
using UnityEngine;


namespace FQParty.GamePlay.Input
{
    /// <summary>
    /// 입력을 받아서 어빌리티를 실행하는 객체
    /// </summary>
    [RequireComponent(typeof(ServerCharacter))]
    public class ClientInputSender : NetworkBehaviour
    {
        [SerializeField]
        GamePlayInputReader m_GameInputReader;

        [SerializeField]
        AbilityCaster m_AbilityCaster;

        [SerializeField]
        AbilityData m_DashAbility;

        [SerializeField]
        AbilityData m_AttackAbility;
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsClient || !IsOwner)
            {
                {
                    enabled = false;
                    return;
                }
            }
            m_GameInputReader.OnDashInput += OnDashInput;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        }

        public void OnDashInput()
        {
            AbilityPacket packet;
            packet.AbilityID = m_DashAbility.AbilityID;
            m_AbilityCaster.CastAbility(packet);
        }


    }
}