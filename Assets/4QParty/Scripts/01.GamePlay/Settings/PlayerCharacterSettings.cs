using FQParty.GamePlay.Abilities;
using UnityEngine;

namespace FQParty.GamePlay.Settings
{
    [CreateAssetMenu(menuName = "Settings/PlayerCharacterSettings", fileName = "NewPlayerCharacterSettings")]
    public class PlayerCharacterSettings : ScriptableObject
    {
        [Header("Base Settings")]
        [SerializeField, Tooltip("캐릭터의 기초 스탯 및 핵심 설정 데이터입니다.")]
        private CharacterSettings m_CharacterSettings;
        public CharacterSettings CharacterSettings => m_CharacterSettings;

        [Header("Attack Abilities")]
        [SerializeField, Tooltip("공격 입력 시 실행될 어빌리티입니다.")]
        private Ability m_AttackPerformedAbility;
        public Ability AttackPerformedAbility => m_AttackPerformedAbility;

        [SerializeField, Tooltip("공격 입력 취소 시 실행될 어빌리티입니다.")]
        private Ability m_AttackCanceledAbility;
        public Ability AttackCanceledAbility => m_AttackCanceledAbility;

        [Header("Interaction Abilities")]
        [SerializeField, Tooltip("상호작용 입력 시 실행될 어빌리티입니다.")]
        private Ability m_InteractPerformedAbility;
        public Ability InteractPerformedAbility => m_InteractPerformedAbility;

        [SerializeField, Tooltip("상호작용 입력 취소 시 실행될 어빌리티입니다.")]
        private Ability m_InteractCanceledAbility;
        public Ability InteractCanceledAbility => m_InteractCanceledAbility;

        [Header("Dash Abilities")]
        [SerializeField, Tooltip("대시 입력 시 실행될 어빌리티입니다.")]
        private Ability m_DashPerformedAbility;
        public Ability DashPerformedAbility => m_DashPerformedAbility;

        [SerializeField, Tooltip("대시 입력 취소 시 실행될 어빌리티입니다.")]
        private Ability m_DashCanceledAbility;
        public Ability DashCanceledAbility => m_DashCanceledAbility;
    }
}