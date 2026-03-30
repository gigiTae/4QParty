using FQParty.GamePlay.Abilities;
using FQParty.GamePlay.Input;
using UnityEngine;

namespace FQParty.GamePlay.Settings
{
    [CreateAssetMenu(menuName = "Settings/PlayerCharacterSettings")]
    public class PlayerCharacterSettings : ScriptableObject,
        ICharacterStatusSettings,
        IPlayerCharacterMovementSettings
    {
        [Header("Movement")]
        [SerializeField] float m_MoveSpeed = 5f;
        public float MoveSpeed => m_MoveSpeed;

        [SerializeField] GamePlayInputReader m_GameInputReader;
        public GamePlayInputReader GamePlayInputReader => m_GameInputReader;

        [SerializeField] bool m_UseInputMove = true;
        public bool UseInputMove => m_UseInputMove;

        [Header("Status")]
        [SerializeField] float m_MaxHp = 100f;
        public float MaxHp => m_MaxHp;

        [SerializeField]
        float m_AttackPower = 10f;
        public float AttackPower => m_AttackPower;

        [SerializeField] Ability m_DeadAbility;
        public Ability DeadAbility => m_DeadAbility;

        [Header("Ability Settings")]
        [SerializeField, Tooltip("공격 입력 시 실행될 어빌리티입니다.")]
        private Ability m_AttackPerformedAbility;
        public Ability AttackPerformedAbility => m_AttackPerformedAbility;

        [SerializeField, Tooltip("공격 입력 취소 시 실행될 어빌리티입니다.")]
        private Ability m_AttackCanceledAbility;
        public Ability AttackCanceledAbility => m_AttackCanceledAbility;

        [SerializeField, Tooltip("상호작용 입력 시 실행될 어빌리티입니다.")]
        private Ability m_InteractPerformedAbility;
        public Ability InteractPerformedAbility => m_InteractPerformedAbility;

        [SerializeField, Tooltip("상호작용 입력 취소 시 실행될 어빌리티입니다.")]
        private Ability m_InteractCanceledAbility;
        public Ability InteractCanceledAbility => m_InteractCanceledAbility;

        [SerializeField, Tooltip("대시 입력 시 실행될 어빌리티입니다.")]
        private Ability m_DashPerformedAbility;
        public Ability DashPerformedAbility => m_DashPerformedAbility;

        [SerializeField, Tooltip("대시 입력 취소 시 실행될 어빌리티입니다.")]
        private Ability m_DashCanceledAbility;
        public Ability DashCanceledAbility => m_DashCanceledAbility;
    }
}