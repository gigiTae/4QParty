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
    public enum AbilityPlayType
    {
        Queue,   // 대기열 추가 
        Cancel,  // 기존 중단 후 실행
        NonBlocking, // 병렬 실행
    }

    [Serializable]
    public enum AbilityInputType
    {
        Trigger, 
        TriggerAndDirection,
        Release,
        ReleaseAndDirectionAndDuration,   
    }


}