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
        [Tooltip("스킬 재사용 가능 시간(초)입니다. 서버에서 이 시간 동안은 동일한 스킬 요청을 받아들이지 않습니다.")]
        public float ReuseTimeSeconds = 0f;

        [Tooltip("어빌리티 처리 규칙 'Queue'는 대기열에 추가하고, 'Cancel'은 현재 스킬을 캔슬하며 즉시 발동합니다.")]
        public AbilityPlayType PlayType;

        [Tooltip("서버 로직(데미지 계산, 상태 이상 등)이 언제 종료된 것으로 간주할지 결정합니다.\n- Any: 효과 중 하나만 끝나도 종료\n- All: 모든 효과가 다 끝나야 종료")]
        public AbilityEndPolicy ServerEndPolicy = AbilityEndPolicy.AnyEffectCompleted;

        [Tooltip("클라이언트 연출(이펙트, 사운드, 애니메이션 등)의 종료 시점을 결정합니다.\n- Any: 연출 하나만 끝나도 다음 동작 가능\n- All: 모든 연출이 끝나야 다음 동작 가능")]
        public AbilityEndPolicy ClientEndPolicy = AbilityEndPolicy.AnyEffectCompleted;

        [Tooltip("어빌리터 입력조건")]
        public AbilityInputOptions InputOptions;
    }
}