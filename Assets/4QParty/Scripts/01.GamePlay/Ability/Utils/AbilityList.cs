using System.Collections.Generic;
using UnityEngine;

namespace FQParty.GamePlay.Abilities
{
    /// <summary>
    /// 모든 어빌리티를 저장하는 리스트입니다 
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/AbilityList")]
    public class AbilityList : ScriptableObject
    {
        public List<Ability> Abilities;
    }
}