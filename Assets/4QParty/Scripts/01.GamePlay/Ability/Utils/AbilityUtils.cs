using System;
using Unity.Netcode;
using UnityEngine;


namespace FQParty.GamePlay.Abilities
{
    public struct AbilityTimeStamp : INetworkSerializeByMemcpy ,IEquatable<AbilityTimeStamp>
    {
        public AbilityID ID;
        public float LastUsedTime;

        public bool Equals(AbilityTimeStamp other)
        {
            return ID == other.ID && LastUsedTime == other.LastUsedTime;    
        }
    }


    public enum AbilityConclusion
    {
        Continue,
        Stop,
    }

    public enum AbilityEndPolicy
    {
        /// <summary>
        /// 하나라도 완료되면 어빌리티 종료 (OR 조건)
        /// </summary>
        AnyEffectCompleted,

        /// <summary>
        /// 모든 이펙트가 완료되어야 어빌리티 종료 (AND 조건)
        /// </summary>
        AllEffectCompleted
    }

    [Serializable]
    public enum BlockingModeType
    {
        /// <summary>
        /// 액션이 시작된 시점부터 완전히 종료(End)될 때까지 전체 지속 시간 동안 다른 동작을 차단합니다.
        /// </summary>
        EntireDuration,

        /// <remarks>
        /// 실행 시간이 지나면 액션 객체가 백그라운드에서 계속 동작(예: 투사체 추적)하더라도, 
        /// 캐릭터는 즉시 이동하거나 다른 액션을 수행할 수 있는 "비차단(Non-blocking)" 상태로 전환됩니다. [1], [2]
        /// </remarks>
        OnlyDuringExecTime,
    }

    [Serializable]
    public enum AbilityPlayType
    {
        Queue,   // 대기열 추가 
        Cancel,  // 기존 중단 후 실행
        NonBlocking, // 병렬 실행
    }



}