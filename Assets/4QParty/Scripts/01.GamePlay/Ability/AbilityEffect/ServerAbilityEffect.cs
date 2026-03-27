using FQParty.GamePlay.Character;
using System;
using UnityEngine;


namespace FQParty.GamePlay.Abilities.Effects
{
    /// <summary>
    /// 서버 사이드 어빌리티 이펙트 최상위 추상 클래스
    /// ■ 주의: 시작 시 IsActive는 true이며, 종료 시 반드시 직접 false로 설정해야 합니다.</para>
    /// ■ 참고: 타 이펙트 상태에 따라 Cancel 호출이 지연될 수 있습니다.</para>
    /// </summary>
    [Serializable]
    public abstract class ServerAbilityEffect
    {
        public virtual void OnStart(ServerCharacter serverCharacter, Ability ability) { }
        public virtual void OnUpdate(ServerCharacter serverCharacter, Ability ability) { }

        public virtual void Cancel(ServerCharacter serverCharacter, Ability ability) { }
        public virtual void End(ServerCharacter serverCharacter, Ability ability) { }

        public virtual void OnAnimationStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        public bool IsActive { get; set; }

        public virtual ServerAbilityEffect Clone()
        {
            return (ServerAbilityEffect)MemberwiseClone();
        }
    }

}