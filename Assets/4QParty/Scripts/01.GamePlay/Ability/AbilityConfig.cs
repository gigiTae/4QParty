using System;
using System.Collections.Generic;
using UnityEngine;

namespace FQParty.GamePlay.Abilities
{
    [Serializable]
    public class AbilityConfig
    {
        public AbilityLogic Logic;

        [Header("시간 설정 (초 단위)")]
        [Tooltip("능력 실행 총 시간 (애니메이션 재생 시간 등)")]
        public float DurationSeconds;

        [Tooltip("실제 효과 발생 시점 (예: 근접 공격 시 실제 데미지가 들어가는 타이밍)")]
        public float ExecTimeSeconds;

        [Tooltip("효과 지속 시간 (버프/디버프 등이 남는 시간)")]
        public float EffectDurationSeconds;

        [Tooltip("재사용 대기 시간 (쿨타임). 서버에서 이 시간 동안 재사용 요청을 무시합니다.")]
        public float ReuseTimeSeconds;

        [Header("애니메이션 설정")]
        [Tooltip("선입력/대기 애니메이션 트리거 (서버 확인 응답을 기다리는 동안 재생)")]
        public string AnimAnticipation;

        [Tooltip("주요 애니메이션 트리거")]
        public string Anim;

        [Tooltip("보조 애니메이션 트리거 (예: 루프 종료용)")]
        public string Anim2;

        [Tooltip("피격 애니메이션 (이 스킬에 맞은 대상이 재생할 트리거)")]
        public string ReactAnim;

        [Header("시스템 및 로직")]
        [Tooltip("능력 입력을 관리하는 프리팹")]
        public BaseAbiltiyInput AbilityInput;

        [Tooltip("다른 능력에의해 중단 가능한지")]
        public bool IsInterruptible;

        [Tooltip("이 능력을 중단시킬 수 있는 능력 리스트")]
        public List<Ability> IsInterruptableBy;

        [Tooltip("차단 모드 (실행 단계 동안만 다른 액션을 막을지, 전체 지속 시간 동안 막을지 결정)")]
        public BlockingModeType BlockingMode;

        /// <summary>
        /// 특정 능력(AbilityID)에 의해 현재 능력이 중단될 수 있는지 확인합니다.
        /// </summary>
        public bool CanBeInterruptedBy(AbilityID abilityID)
        {
            foreach (var action in IsInterruptableBy)
            {
                if (action.AbilityID == abilityID)
                {
                    return true;
                }
            }

            return false;
        }
    }
}