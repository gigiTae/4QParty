using FQParty.GamePlay.Abilities;
using UnityEngine;

namespace FQParty.GamePlay.Character
{

    [CreateAssetMenu(fileName = "CharacterSettings", menuName = "GamePlay/CharacterSettings")]
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

        [Header("Ability")]
        [Tooltip("대쉬 어빌리티")]
        public Ability DashAbility;

        [Tooltip("기본공격 어빌리티")]
        public Ability AttackAbility;

        [Tooltip("상호작용 어빌리티")]
        public Ability InteractAbility;
    }
}