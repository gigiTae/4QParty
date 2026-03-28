using FQParty.GamePlay.Character;
using UnityEngine;


namespace FQParty.GamePlay.Abilities
{
    /// <summary>
    /// 어빌리티를 상대에게 부여할때 필요한 데이터
    /// </summary>
    public struct AbilityApplyData
    {
        // 어빌리티를 부여한 대상
        public ServerCharacter Caster;
    }

}