using FQParty.GamePlay.GameplayObjects;
using FQParty.GamePlay.Settings;
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
    public class CharacterStatus : NetworkBehaviour, IDamageable
    {
        [SerializeField] CharacterSettings m_Settings;
        [SerializeField] ServerCharacter m_ServerCharacter;

        NetworkVariable<CharacterStatement> m_Statement = new(CharacterStatement.Alive);
        NetworkVariable<float> m_CurrentHp = new();
        NetworkVariable<float> m_AttackPower = new();
        public float MaxHp => m_Settings.MaxHp;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
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

        public void ReceiveDamage(ServerCharacter serverCharacter, float value)
        {
            if (!IsServer) return;

            m_CurrentHp.Value = Mathf.Max(0, m_CurrentHp.Value - value);
        }

        private void OnGUI()
        {
            string abilityName = ""; 
            if (m_ServerCharacter.AbilityPlayer.PlayingAbility != null)
            {
                abilityName = m_ServerCharacter.AbilityPlayer.PlayingAbility.name;
            }

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
            string hpText = $"Hp: {m_CurrentHp.Value} / {MaxHp} \n {abilityName}";
            GUI.Label(new Rect(screenPos.x - 50, Screen.height - screenPos.y - 20, 100, 40), hpText, style);
        }

    }
}