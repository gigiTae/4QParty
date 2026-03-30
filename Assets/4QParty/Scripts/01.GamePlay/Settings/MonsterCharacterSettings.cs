using FQParty.GamePlay.Abilities;
using UnityEngine;

namespace FQParty.GamePlay.Settings
{
    [CreateAssetMenu(menuName = "Settings/MonsterCharacterSettings")]

    public class MonsterCharacterSettings : ScriptableObject,
        ICharacterStatusSettings
    {
        [Header("Status")]
        [SerializeField] float m_MaxHp = 100f;
        public float MaxHp => m_MaxHp;

        [SerializeField]
        float m_AttackPower = 5f;
        public float AttackPower => m_AttackPower;

        [SerializeField] Ability m_DeadAbility;
        public Ability DeadAbility => m_DeadAbility;
    }

}