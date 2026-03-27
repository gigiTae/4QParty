using FQParty.GamePlay.Input;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FQParty.GamePlay.Abilities
{
    [Serializable]
    public class AbilityConfig
    {
        [Tooltip("재사용 대기 시간 (쿨타임). 서버에서 이 시간 동안 재사용 요청을 무시합니다.")]
        public float ReuseTimeSeconds = 0f;

        [Tooltip("어빌리티 처리 규칙 'Queue'는 대기열에 추가하고, 'Cancel'은 현재 스킬을 캔슬하며 즉시 발동합니다.")]
        public AbilityPlayType PlayType;

        [Tooltip("다른 능력에의해 중단 가능한지")]
        public bool IsInterruptible;

        [Tooltip("어빌리터 종료조건")]
        public AbilityEndPolicy EndPolicy = AbilityEndPolicy.AnyEffectCompleted;

        [Tooltip("어빌리터 입력조건")]
        public AbilityInputOptions InputOptions;
    }
}