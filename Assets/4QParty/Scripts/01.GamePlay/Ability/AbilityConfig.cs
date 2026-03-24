using System;
using System.Collections.Generic;
using UnityEngine;

namespace FQParty.GamePlay.Abilities
{
    [Serializable]
    public class AbilityConfig
    {
        [Tooltip("재사용 대기 시간 (쿨타임). 서버에서 이 시간 동안 재사용 요청을 무시합니다.")]
        public float ReuseTimeSeconds = 0f;

        [Header("시스템 및 로직")]
        [Tooltip("능력 입력을 관리하는 프리팹")]
        public BaseAbiltiyInput AbilityInput;

        [Tooltip("어빌리티 처리 규칙 'Queue'는 대기열에 추가하고, 'Cancel'은 현재 스킬을 캔슬하며 즉시 발동합니다.")]
        public AbilityPlayType PlayType;

        [Tooltip("다른 능력에의해 중단 가능한지")]
        public bool IsInterruptible;

        [Tooltip("차단 모드 (실행 단계 동안만 다른 액션을 막을지, 전체 지속 시간 동안 막을지 결정)")]
        public BlockingModeType BlockingMode;

        [Tooltip("어빌리터 종료조건")]
        public AbilityEndPolicy EndPolicy = AbilityEndPolicy.AnyEffectCompleted;
    }
}