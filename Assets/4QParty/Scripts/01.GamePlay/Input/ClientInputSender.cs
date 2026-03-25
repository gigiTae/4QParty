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
    [RequireComponent(typeof(ServerCharacter))]
    public class ClientInputSender : NetworkBehaviour
    {
        [SerializeField]
        GamePlayInputReader m_GameInputReader;

        [SerializeField]
        ServerCharacter m_ServerCharacter;

        [SerializeField]
        ServerAbilityPlayer m_ServerAbilityPlayer;

        public event Action<AbilityRequestData> AbilityInputEvent;

        [SerializeField]
        Ability m_InteractAbility;

        [SerializeField]
        Ability m_DashAbility;

        [SerializeField]
        Ability m_AttackAbility;


        struct AbilityRequset
        {
            public SkillTriggerStyle TriggerStyle;
            public AbilityID RequsetAbilityID;
            public ulong TargetID;
        }

        readonly AbilityRequset[] m_AbilityRequsets = new AbilityRequset[5];

        int m_AbilityRequsetCount;

        BaseAbiltiyInput m_CurrentAbilityInput;

        public enum SkillTriggerStyle
        {
            None,
            Button,
            ButtonRelease,
        }

        bool IsReleaseStyle(SkillTriggerStyle style)
        {
            return style == SkillTriggerStyle.ButtonRelease;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsClient || !IsOwner)
            {
                enabled = false;
                return;
            }

            m_GameInputReader.OnAttackInput += OnAttackAbilityStarted;
            m_GameInputReader.OnDashInput += OnDashAbilityStarted;
            m_GameInputReader.OnInteractInput += OnInteractAbilityStarted;
        }


        public override void OnNetworkDespawn()
        {
            m_GameInputReader.OnAttackInput -= OnAttackAbilityStarted;
            m_GameInputReader.OnDashInput -= OnDashAbilityStarted;
            m_GameInputReader.OnInteractInput -= OnInteractAbilityStarted;
        }



        void OnDashAbilityStarted()
        {
            RequsetAbility(m_DashAbility.AbilityID, SkillTriggerStyle.Button);
        }

        void OnAttackAbilityStarted()
        {
            RequsetAbility(m_AttackAbility.AbilityID, SkillTriggerStyle.Button);
        }

        void OnInteractAbilityStarted()
        {
            RequsetAbility(m_InteractAbility.AbilityID, SkillTriggerStyle.Button);
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
                // 현재 활성화된 스킬입력 확인
                if (m_CurrentAbilityInput != null)
                {
                    if (IsReleaseStyle(m_AbilityRequsets[i].TriggerStyle))
                    {
                        m_CurrentAbilityInput.OnReleaseKey();
                    }
                }
                else if (!IsReleaseStyle(m_AbilityRequsets[i].TriggerStyle))
                {
                    var abilityPrototype = GameDataSource.Instance.GetAbilityPrototypeByID(m_AbilityRequsets[i].RequsetAbilityID);

                    // 어빌리티 설정에 AbilityInput이 있는 경우 
                    if (abilityPrototype.Config.AbilityInput != null)
                    {
                        var skillPlayer = Instantiate(abilityPrototype.Config.AbilityInput);
                        skillPlayer.Initiate(m_ServerCharacter, transform.position, abilityPrototype.AbilityID, SendInput, FinishAbilityInput);
                        m_CurrentAbilityInput = skillPlayer;
                    }
                    else
                    {
                        PerformAbility(abilityPrototype.AbilityID, m_AbilityRequsets[i].TriggerStyle, m_AbilityRequsets[i].TargetID);
                    }
                }
            }

            m_AbilityRequsetCount = 0;
        }

        void FinishAbilityInput()
        {
            m_CurrentAbilityInput = null;
        }

        void PerformAbility(AbilityID abilityID, SkillTriggerStyle triggerStyle, ulong targetID)
        {
            Transform hitTransform = null;

            if (targetID != 0)
            {
                NetworkObject targetNetObj;
                if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetID, out targetNetObj))
                {
                    hitTransform = targetNetObj.transform;
                }
            }

            var data = new AbilityRequestData();
            data.AbilityID = abilityID;

            SendInput(data);
        }

        public void RequsetAbility(AbilityID abilityID, SkillTriggerStyle triggerStyle, ulong targetId = 0)
        {
            Assert.IsNotNull(GameDataSource.Instance.GetAbilityPrototypeByID(abilityID),
             $"Ability with abilityID {abilityID} must be contained in the Ability prototypes of GameDataSource!");

            if (m_AbilityRequsetCount < m_AbilityRequsets.Length)
            {
                m_AbilityRequsets[m_AbilityRequsetCount].RequsetAbilityID = abilityID;
                m_AbilityRequsets[m_AbilityRequsetCount].TriggerStyle = triggerStyle;
                m_AbilityRequsets[m_AbilityRequsetCount].TargetID = targetId;
                m_AbilityRequsetCount++;
            }
        }

    }
}