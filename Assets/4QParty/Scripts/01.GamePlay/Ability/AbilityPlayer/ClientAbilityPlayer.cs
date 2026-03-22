using UnityEngine;
using System.Collections.Generic;
using FQParty.GamePlay.Character;

namespace FQParty.GamePlay.Abilities
{
    public sealed class ClientAbilityPlayer
    {
        private List<Ability> m_PlayingAbilities = new();

        private const float k_AnticipationTimeoutSeconds = 1;

        public ClientCharacter ClientCharacter { get; private set; }

        public ClientAbilityPlayer(ClientCharacter clientCharacter)
        {
            ClientCharacter = clientCharacter;
        }

        public void OnUpdate()
        {
            for (int i = m_PlayingAbilities.Count - 1; i >= 0; --i)
            {
                var ability = m_PlayingAbilities[i];
                bool keepGoing = ability.AnticipatedClient || ability.OnUpdateClient(ClientCharacter);
                bool expirable = ability.Config.DurationSeconds > 0f;
                bool timeExpired = expirable && ability.TimeRunning >= ability.Config.DurationSeconds;
                bool timeOut = ability.AnticipatedClient && ability.TimeRunning >= k_AnticipationTimeoutSeconds;

                if (!keepGoing || timeExpired || timeOut)
                {
                    if (timeOut)
                    {
                        ability.CancelClient(ClientCharacter);
                    }
                    else
                    {
                        ability.EndClient(ClientCharacter);
                    }

                    m_PlayingAbilities.RemoveAt(i);
                    AbilityFactory.ReturnAbility(ability);
                }
            }
        }

        private int FindAbility(AbilityID abilityID, bool anticipatedOnly)
        {
            return m_PlayingAbilities.FindIndex(a => a.AbilityID == abilityID && (!anticipatedOnly || a.AnticipatedClient));
        }

        public void OnAnimEvent(string id)
        {
            foreach (var abilityFx in m_PlayingAbilities)
            {
            }
        }

        /// <summary>
        /// 플레이어가 액션을 실행할 때, 해당 캐릭터를 소유한 클라이언트에서 호출됩니다. 
        /// 이를 통해 액션이 피드백(시각 효과 등)을 즉시 재생할 수 있습니다.
        /// </summary>
        /// <remarks>
        ///
        /// '액션 예측(Action Anticipation)'이란 무엇이며 어떤 문제를 해결할까요? 짧게 말해, 로컬 클라이언트에서 
        /// 액션을 트리거하는 입력 이벤트가 감지되는 즉시 해당 액션의 로직을 실행할 수 있게 해줍니다. 
        /// 이 기능의 목적은 레이턴시(지연 시간)를 감추는 것입니다. 본 데모는 서버 권한 방식(Server Authoritative)이므로, 
        /// 기본 설정으로는 서버-클라이언트를 한 번 왕복(Round-trip)한 후에야 입력에 대한 피드백을 볼 수 있습니다. 
        /// 왕복 지연 시간이 200ms를 넘어가면 플레이어는 게임이 매우 답답하고 느리다고 느끼기 시작합니다. 
        ///
        /// 이를 방지하기 위해 시각 효과를 즉시 재생할 수 있습니다. 예를 들어, 'MeleeActionFX'는 서버의 응답을 
        /// 기다리지 않고 즉시 무기를 휘두르고 대상에 적중 반응(Hit react)을 적용합니다. 이는 서버가 실제로 
        /// 대상이 맞지 않았다고 판단할 경우 불일치를 발생시킬 수 있지만, 네트워크 환경 전체적으로는 훨씬 더 반응성이 좋게 느껴집니다.
        ///
        /// 액션 예측의 중요한 개념 중 하나는 이것이 '기회주의적'이라는 것입니다. 즉, 어떠한 강력한 보장도 하지 않습니다. 
        /// 예를 들어, 캐릭터가 이미 다른 애니메이션을 재생 중이라면 예측 애니메이션은 재생되지 않습니다. 
        /// 또 다른 복잡한 점은 서버가 요청된 모든 액션을 실제로 허용할지 알 수 없다는 것입니다. 
        /// (예: 대기열에 액션이 너무 많아 일부가 버려지는 경우) 
        /// 결과적으로 '예측된 액션(생성되었으나 아직 시작되지 않은 액션)'은 서버로부터 승인되어 전달된 '실제 액션'과 
        /// 완벽하게 일치하지 않을 수 있습니다. 
        ///
        /// 따라서 (서버 소유 캐릭터나 다른 플레이어 캐릭터의 경우처럼) '예측된 액션'이 없는 상태에서 'PlayAction'을 
        /// 수신하더라도 항상 정상적으로 처리되어야 합니다. 또한 예측된 액션을 생성했지만 서버의 확인을 
        /// 받지 못한 경우에 대비하여, 해당 액션을 결국 폐기(discard)하는 처리도 필요합니다.
        ///
        /// 액션 예측의 또 다른 특징은 '선택적(Opt-in)' 시스템이라는 점입니다. Start 구현부에서 'base.Start'를 
        /// 호출하기만 하면 되며, 만약 특정 액션에 대해 예측을 구현할 적절한 방법이 없다면 아무것도 하지 않아도 됩니다. 
        /// 이 경우 해당 액션은 '정상적으로'(서버의 액션 브로드캐스트가 클라이언트에 도착할 때 시각 피드백이 시작됨) 재생됩니다. 
        /// 각 액션 타입은 예측 효과를 자연스럽게 보여주기 위해 각기 다른 문제들을 해결해야 합니다. 
        /// 예를 들어, 본 데모의 마법사 기본 공격(FXProjectileTargetedActionFX)은 공격 애니메이션만 예측하여 재생하지만, 
        /// 이를 수정하여 마법 화살(Mage bolt) 효과까지 즉시 생성하고 구동하게 만들 수도 있습니다. 
        /// 이럴 경우 실제 서버 응답 시점에는 '데미지' 처리만 이루어지게 됩니다.
        ///
        /// [나만의 예측 로직 구현 방법]:
        ///   1. 예측하여 재생하고 싶은 시각 피드백을 ActionFX 내의 별도 private 헬퍼 메서드(예: "PlayAttackAnim")로 분리합니다.
        ///   2. ActionFX.AnticipateAction을 오버라이드합니다. 이때 반드시 'base.AnticipateAction'을 호출하고 시각 로직(PlayAttackAnim 등)을 실행하십시오.
        ///   3. Start 메서드에서 반드시 'base.Start'를 호출하십시오. (이 호출은 "Anticipated" 필드를 false로 리셋합니다.)
        ///   4. Start 메서드 내에서 해당 액션이 '예측(Anticipated)' 되었는지 확인합니다. 만약 '아니라면(NOT)', PlayAttackAnim 메서드를 호출하여 재생합니다.
        ///
        /// </remarks>
        /// <param name="data">요청된 액션 데이터입니다.</param>
        public void AnticipateAction(ref AbilityRequestData data)
        {
            //if (!ClientCharacter.IsAnimating() && Action.ShouldClientAnticipate(ClientCharacter, ref data))
            //{
            //    var actionFX = AbilityFactory.CreateActionFromData(ref data);
            //    actionFX.AnticipateActionClient(ClientCharacter);
            //    m_PlayingAbilities.Add(actionFX);
            //}
        }

        public void PlayAbility(ref AbilityRequestData data)
        {
            var anticipatedAbilityIndex = FindAbility(data.AbilityID, true);

            var abilityFX = anticipatedAbilityIndex >= 0 ? m_PlayingAbilities[anticipatedAbilityIndex] : AbilityFactory.CreateAbilityFromData(ref data);
            if(abilityFX.OnStartClient(ClientCharacter))
            {
                if(anticipatedAbilityIndex < 0)
                {
                    m_PlayingAbilities.Add(abilityFX);
                }
            }
            else if(anticipatedAbilityIndex >=0)
            {
                var removedAbility = m_PlayingAbilities[anticipatedAbilityIndex];
                m_PlayingAbilities.RemoveAt(anticipatedAbilityIndex);
                AbilityFactory.ReturnAbility(removedAbility);
            }

        }

        public void CancelAllAbilities()
        {
            foreach(var ability in m_PlayingAbilities)
            {
                ability.CancelClient(ClientCharacter);
                AbilityFactory.ReturnAbility(ability);
            }
            m_PlayingAbilities.Clear();
        }

        public void CancelAllAbilitiesWithSameProtypeID(AbilityID abilityID)
        {
            for (int i = m_PlayingAbilities.Count - 1; i >= 0; --i)
            {
                if (m_PlayingAbilities[i].AbilityID == abilityID)
                {
                    var ability = m_PlayingAbilities[i];
                    ability.CancelClient(ClientCharacter);
                    m_PlayingAbilities.RemoveAt(i);
                    AbilityFactory.ReturnAbility(ability);
                }
            }
        }


    }

}