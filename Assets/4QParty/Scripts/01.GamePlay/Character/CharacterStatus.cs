using FQParty.GamePlay.Abilities;
using FQParty.GamePlay.GameplayObjects;
using FQParty.GamePlay.Settings;
using System;
using System.Data;
using Unity.Netcode;
using UnityEngine;


namespace FQParty.GamePlay.Character
{
    public enum CharacterStatement
    {
        Alive,
        Dead,
    }

    /// <summary>
    /// 캐릭터의 기본상태를 관리하는 클래스
    /// </summary>
    [RequireComponent(typeof(ServerAbilityPlayer))]
    public class CharacterStatus : NetworkBehaviour, IDamageable
    {
        public Action<float,float> OnHpChangedEvent;

        public void BindSettings(ICharacterStatusSettings settings)
        {
            m_Settings = settings;
        }
        ICharacterStatusSettings m_Settings;

        ServerAbilityPlayer m_ServerAbilityPlayer;

        NetworkVariable<CharacterStatement> m_Statement = new(CharacterStatement.Alive);
        NetworkVariable<float> m_CurrentHp = new();
        NetworkVariable<float> m_AttackPower = new();

        void Awake()
        {
            m_ServerAbilityPlayer = GetComponent<ServerAbilityPlayer>();
        }

        public float MaxHp
        {
            get
            {
                if (m_Settings == null)
                {
                    return 0f;
                }
                return m_Settings.MaxHp;
            }
        }

        void OnHpChanged(float previousValue, float newValue)
        {
            OnHpChangedEvent?.Invoke(previousValue, newValue);    
        }


        public float AttackPower => m_AttackPower.Value;

        public override void OnNetworkSpawn()
        {
            m_CurrentHp.OnValueChanged += OnHpChanged;
            
            if (IsServer && m_Settings != null)
            {
                m_CurrentHp.Value = m_Settings.MaxHp;
                m_AttackPower.Value = m_Settings.AttackPower;
            }
        }
        public override void OnNetworkDespawn()
        {

        }

        public bool IsDamageable()
        {
            return m_Statement.Value != CharacterStatement.Dead;
        }

        public void TakeDamage(float damage)
        {
            if (!IsServer) return;

            m_CurrentHp.Value = Mathf.Max(0f, m_CurrentHp.Value - damage);

            if (m_CurrentHp.Value <= 0f)
            {
                SetDead();
            }
        }

        void SetDead()
        {
            if (m_Statement.Value == CharacterStatement.Dead) return;

            m_Statement.Value = CharacterStatement.Dead;
            var data = AbilityRequestData.Create(m_Settings.DeadAbility);
            m_ServerAbilityPlayer.RequestAbilityServerRpc(data);
        }

        private void OnGUI()
        {
            // 1. 월드 좌표를 스크린 좌표로 변환 (캐릭터 머리 위)
            Vector3 worldPos = transform.position + Vector3.up * 2.5f;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            // 2. 카메라 뒤에 있는 경우 그리지 않음
            if (screenPos.z < 0) return;

            // 3. GUI 스타일 설정
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;

            // HP 양에 따라 색상 변경 (디버그 가독성)
            style.normal.textColor = m_CurrentHp.Value > 30 ? Color.green : Color.red;

            // 4. 화면에 HP 출력 (Y축은 GUI 좌표계상 반대이므로 변환)
            string hpText = $"Hp: {m_CurrentHp.Value} / {MaxHp}";
            GUI.Label(new Rect(screenPos.x - 50, Screen.height - screenPos.y - 20, 100, 40), hpText, style);
        }
    }
}