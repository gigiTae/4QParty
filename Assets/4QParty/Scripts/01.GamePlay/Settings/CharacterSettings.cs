using FQParty.GamePlay.Abilities;
using UnityEngine;

namespace FQParty.GamePlay.Settings
{
    [CreateAssetMenu(menuName = "Settings/CharacterSettings")]
    public class CharacterSettings : ScriptableObject
    {
        [Header("Movement")]
        [Tooltip("이동속도")]
        public float MoveSpeed = 5.0f;

        [Header("Status")]
        [Tooltip("캐릭터 체력")]
        public float MaxHp = 100f;

        [Tooltip("공격력 ex) AttackPower * 공격력 계산방식")]
        public float AttackPower = 1f;
    }
}