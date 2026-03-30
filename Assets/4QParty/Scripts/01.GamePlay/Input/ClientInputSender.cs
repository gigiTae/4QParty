using FQParty.GamePlay.Abilities;
using FQParty.GamePlay.GameplayObjects;
using FQParty.GamePlay.Settings;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

namespace FQParty.GamePlay.Input
{
    /// <summary>
    /// 입력을 받아서 어빌리티를 실행하는 객체
    /// </summary>
    [RequireComponent(typeof(ServerAbilityPlayer), typeof(ClientAbilityPlayer))]
    public class ClientInputSender : NetworkBehaviour
    {
        PlayerCharacterSettings m_Settings;
        public void BindSettings(PlayerCharacterSettings settings)
        {
            m_Settings = settings;
        }

        ServerAbilityPlayer m_ServerAbilityPlayer;
        ClientAbilityPlayer m_ClientAbilityPlayer;
        InputControllerType m_InputControllerType = InputControllerType.KeyboradAndMouse;

        readonly AbilityRequestData[] m_AbilityRequests = new AbilityRequestData[5];
        int m_AbilityRequestCount;

        private void Awake()
        {
            m_ServerAbilityPlayer = GetComponent<ServerAbilityPlayer>();
            m_ClientAbilityPlayer = GetComponent<ClientAbilityPlayer>();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsClient || !IsOwner)
            {
                enabled = false;
                return;
            }

            if (m_Settings != null && m_Settings.GamePlayInputReader != null)
            {
                // Performed Events
                m_Settings.GamePlayInputReader.AttackPerformedEvent += OnAttackPerformed;
                m_Settings.GamePlayInputReader.DashPerformedEvent += OnDashPerformed;
                m_Settings.GamePlayInputReader.InteractPerformedEvent += OnInteractPerformed;

                // Canceled Events 
                m_Settings.GamePlayInputReader.AttackCanceledEvent += OnAttackCanceled;
                m_Settings.GamePlayInputReader.DashCanceledEvent += OnDashCanceled;
                m_Settings.GamePlayInputReader.InteractCanceledEvent += OnInteractCanceled;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (m_Settings.GamePlayInputReader != null)
            {
                m_Settings.GamePlayInputReader.AttackPerformedEvent -= OnAttackPerformed;
                m_Settings.GamePlayInputReader.DashPerformedEvent -= OnDashPerformed;
                m_Settings.GamePlayInputReader.InteractPerformedEvent -= OnInteractPerformed;

                m_Settings.GamePlayInputReader.AttackCanceledEvent -= OnAttackCanceled;
                m_Settings.GamePlayInputReader.DashCanceledEvent -= OnDashCanceled;
                m_Settings.GamePlayInputReader.InteractCanceledEvent -= OnInteractCanceled;
            }
        }

        #region Input Handlers

        private void OnAttackPerformed() => RequestAbilityFromSettings(m_Settings.AttackPerformedAbility);
        private void OnAttackCanceled() => RequestAbilityFromSettings(m_Settings.AttackCanceledAbility);

        private void OnDashPerformed() => RequestAbilityFromSettings(m_Settings.DashPerformedAbility);
        private void OnDashCanceled() => RequestAbilityFromSettings(m_Settings.DashCanceledAbility);

        private void OnInteractPerformed() => RequestAbilityFromSettings(m_Settings.InteractPerformedAbility);
        private void OnInteractCanceled() => RequestAbilityFromSettings(m_Settings.InteractCanceledAbility);

        /// <summary>
        /// 셋팅 데이터에 할당된 어빌리티가 있는지 확인 후 요청합니다.
        /// </summary>
        private void RequestAbilityFromSettings(Ability ability)
        {
            if (ability != null)
            {
                RequestAbility(ability);
            }
        }
        #endregion

        private void FixedUpdate()
        {
            // 한 프레임에 쌓인 요청들을 처리
            for (int i = 0; i < m_AbilityRequestCount; ++i)
            {
                PerformAbility(m_AbilityRequests[i]);
            }

            m_AbilityRequestCount = 0;
        }

        private void PerformAbility(AbilityRequestData data)
        {
            // 클라이언트 실행
            m_ClientAbilityPlayer.RequestAbility(data);

            // 서버로 실행 요청 전달
            m_ServerAbilityPlayer.RequestAbilityServerRpc(data);
        }

        public void RequestAbility(Ability ability)
        {
            AbilityID abilityID = ability.AbilityID;

            Assert.IsNotNull(GameDataManager.Instance.GetAbilityByID(abilityID),
                $"Ability ID {abilityID}가 GameDataManager 프로토타입에 존재하지 않습니다!");

            if (m_AbilityRequestCount >= m_AbilityRequests.Length)
            {
                Debug.LogWarning("Ability request queue is full!");
                return;
            }

            // 쿨타임 확인
            bool canRequset = m_ServerAbilityPlayer.CanRequsetAbility(ability);
            if (!canRequset) return;


            // 입력 방향 정보
            if (ability.Config.InputOptions.HasFlag(AbilityInputOptions.Direction))
            {
                m_AbilityRequests[m_AbilityRequestCount].Direction = GetDirectionInput();
            }

            // 입력 시간 정보
            if (ability.Config.InputOptions.HasFlag(AbilityInputOptions.Duration))
            {
                m_AbilityRequests[m_AbilityRequestCount].Duration = 0f;
            }


            m_AbilityRequests[m_AbilityRequestCount].AbilityID = ability.AbilityID;

            m_AbilityRequestCount++;
        }

        public Vector2 GetDirectionInput()
        {
            Vector2 direction = Vector2.zero;
            if (m_InputControllerType == InputControllerType.Gamepad)
            {
                direction = m_Settings.GamePlayInputReader.PlayerMoveInput;
            }
            else if (m_InputControllerType == InputControllerType.KeyboradAndMouse)
            {
                direction = m_Settings.GamePlayInputReader.GetMouseDirection(transform);
            }

            return direction;
        }
    }
}