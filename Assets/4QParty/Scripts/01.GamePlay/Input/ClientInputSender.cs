using FQParty.GamePlay.Abilities;
using FQParty.GamePlay.Character;
using FQParty.GamePlay.GameplayObjects;
using NUnit.Framework;
using System;
using Unity.Netcode;
using UnityEngine;


namespace FQParty.GamePlay.Input
{
    /// <summary>
    /// 입력을 받아서 어빌리티를 실행하는 객체
    /// </summary>
    [RequireComponent(typeof(ServerAbilityPlayer))]
    public class ClientInputSender : NetworkBehaviour
    {
        public event Action<AbilityRequestData> AbilityInputEvent;

        ServerAbilityPlayer m_ServerAbilityPlayer;

        [SerializeField] GamePlayInputReader m_GameInputReader;
        [SerializeField] Ability m_InteractAbility;
        [SerializeField] Ability m_DashAbility;
        [SerializeField] Ability m_AttackAbility;

        struct AbilityRequset
        {
            public AbilityID RequsetAbilityID;
        }

        readonly AbilityRequset[] m_AbilityRequsets = new AbilityRequset[5];

        int m_AbilityRequsetCount;

        public void Awake()
        {
            m_ServerAbilityPlayer = GetComponent<ServerAbilityPlayer>();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsClient || !IsOwner)
            {
                enabled = false;
                return;
            }

            m_GameInputReader.AttackPerformedEvent += OnAttackAbilityStarted;
            m_GameInputReader.DashPerformedEvent += OnDashAbilityStarted;
            m_GameInputReader.InteractPerformedEvent += OnInteractAbilityStarted;
        }

        public override void OnNetworkDespawn()
        {
            m_GameInputReader.AttackPerformedEvent -= OnAttackAbilityStarted;
            m_GameInputReader.DashPerformedEvent -= OnDashAbilityStarted;
            m_GameInputReader.InteractPerformedEvent -= OnInteractAbilityStarted;
        }

        void OnDashAbilityStarted()
        {
            RequsetAbility(m_DashAbility.AbilityID);
        }

        void OnAttackAbilityStarted()
        {
            if (m_AttackAbility != null)
                RequsetAbility(m_AttackAbility.AbilityID);
        }

        void OnInteractAbilityStarted()
        {
            RequsetAbility(m_InteractAbility.AbilityID);
        }

        void SendInput(AbilityRequestData ability)
        {
            AbilityInputEvent?.Invoke(ability);
            m_ServerAbilityPlayer.RequestAbilityServerRpc(ability);
        }

        void FixedUpdate()
        {
            for (int i = 0; i < m_AbilityRequsetCount; ++i)
            {
                var ability = GameDataManager.Instance.GetAbilityByID(m_AbilityRequsets[i].RequsetAbilityID);
                PerformAbility(ability.AbilityID);
            }

            m_AbilityRequsetCount = 0;

        }

        void PerformAbility(AbilityID abilityID)
        {
            var data = new AbilityRequestData();
            data.AbilityID = abilityID;
            SendInput(data);
        }

        public void RequsetAbility(AbilityID abilityID)
        {
            Assert.IsNotNull(GameDataManager.Instance.GetAbilityByID(abilityID),
             $"Ability with abilityID {abilityID} must be contained in the Ability prototypes of GameDataSource!");

            if (m_AbilityRequsetCount < m_AbilityRequsets.Length)
            {
                m_AbilityRequsets[m_AbilityRequsetCount].RequsetAbilityID = abilityID;
                m_AbilityRequsetCount++;
            }
        }


    }
}