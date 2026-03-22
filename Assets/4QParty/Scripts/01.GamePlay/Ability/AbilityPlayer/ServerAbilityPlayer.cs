using Codice.Client.Common.Connection;
using Codice.Client.Common.WebApi.Responses;
using FQParty.GamePlay.Character;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static PlasticGui.WorkspaceWindow.Merge.MergeInProgress;

namespace FQParty.GamePlay.Abilities
{
    public class ServerAbilityPlayer
    {
        ServerCharacter m_ServerCharacter;

        List<Ability> m_Queue;

        List<Ability> m_NonBlockingAbilities;

        Dictionary<AbilityID, float> m_LastUsedTimestamps;

        /// <summary>
        /// 어빌티 대기열(Abiltiy Queue)이 무한정 늘어나는 것을 방지하기 위해, 대기열의 총 재생 시간을 이 초(seconds) 단위 수치로 제한합니다.
        /// 어빌리티는 무기한으로 중단(block)될 가능성이 있으므로 대기열의 정확한 시간 길이는 추정만 가능할 뿐입니다. 
        /// 하지만 이 수치는 수많은 작은 어빌리티들이 과도하게 쌓이는 것을 방지하는 유용한 예측 지표가 됩니다.
        /// </summary>
        const float k_MaxQueueTimeDepth = 1.6f;

        AbilityRequestData m_PendingSynthesizedAblity = new AbilityRequestData();
        bool m_HasPendingSynthsizedAblity;

        public ServerAbilityPlayer(ServerCharacter serverCharacter)
        {
            m_ServerCharacter = serverCharacter;
            m_Queue = new();
            m_NonBlockingAbilities = new();
            m_LastUsedTimestamps = new();
        }

        public void PlayAbility(ref AbilityRequestData ability)
        {
            if (!ability.ShouldQueue && m_Queue.Count > 0 &&
                (m_Queue[0].Config.IsInterruptible ||
                m_Queue[0].Config.CanBeInterruptedBy(ability.AbilityID)))
            {
                ClearAbility(false);
            }

            if (GetQueueTimeDepth() >= k_MaxQueueTimeDepth)
            {
                return;
            }

            var newAbility = AbilityFactory.CreateAbilityFromData(ref ability);

            m_Queue.Add(newAbility);
            if (m_Queue.Count == 1)
            {
                StartAbility();
            }
        }

        void StartAbility()
        {
            if (m_Queue.Count == 0) return;

            float reuseTime = m_Queue[0].Config.ReuseTimeSeconds;
            if (reuseTime > 0 &&
                m_LastUsedTimestamps.TryGetValue(m_Queue[0].AbilityID, out float lastTimeUsed) &&
                Time.time - lastTimeUsed > reuseTime)
            {
                AdvanceQueue(false);
                return;
            }

            m_Queue[0].TimeStarted = Time.time;
            bool play = m_Queue[0].OnStart(m_ServerCharacter);

            if (play == AbilityConclusion.Stop)
            {
                AdvanceQueue(false);
                return;
            }


            m_LastUsedTimestamps[m_Queue[0].AbilityID] = Time.time;

            if (m_Queue[0].Config.ExecTimeSeconds == 0 && m_Queue[0].Config.BlockingMode == BlockingModeType.OnlyDuringExecTime)
            {
                m_NonBlockingAbilities.Add(m_Queue[0]);
                AdvanceQueue(false);
                return;
            }
        }

        void AdvanceQueue(bool endRemoved)
        {
            if (m_Queue.Count > 0)
            {
                if (endRemoved)
                {
                    m_Queue[0].End(m_ServerCharacter);
                    if (m_Queue[0].ChainIntoNewAction(ref m_PendingSynthesizedAblity))
                    {
                        m_HasPendingSynthsizedAblity = true;
                    }
                }
                var ability = m_Queue[0];
                m_Queue.RemoveAt(0);
                TryReturnAbility(ability);
            }

            if (!m_HasPendingSynthsizedAblity || m_PendingSynthesizedAblity.ShouldQueue)
            {
                StartAbility();
            }
        }

        public void OnUpdate()
        {
            if (m_HasPendingSynthsizedAblity)
            {
                m_HasPendingSynthsizedAblity = false;
                PlayAbility(ref m_PendingSynthesizedAblity);
            }

            if (m_Queue.Count > 0 && m_Queue[0].ShouldBecomeNonBlocking())
            {
                m_NonBlockingAbilities.Add(m_Queue[0]);
                AdvanceQueue(false);
            }

            if (m_Queue.Count > 0)
            {
            }

        }


        bool UpdateAbility(Ability ability)
        {
            bool keepGoing = ability.OnUpdate(m_ServerCharacter);
            bool exiprable = ability.Config.DurationSeconds > 0f;
            float timeElased = Time.time - ability.TimeStarted;
            bool timeExipred = exiprable && timeElased >= ability.Config.DurationSeconds;
            return keepGoing && !timeExipred;
        }

        float GetQueueTimeDepth()
        {
            if (m_Queue.Count == 0)
            {
                return 0f;
            }

            float totalTime = 0;
            foreach (var ability in m_Queue)
            {
                var config = ability.Config;
                float actionTime = config.BlockingMode == BlockingModeType.OnlyDuringExecTime ? config.ExecTimeSeconds :
                             config.BlockingMode == BlockingModeType.EntireDuration ? config.DurationSeconds :
                             throw new System.Exception($"Unrecognized blocking mode: {config.BlockingMode}");

                totalTime += actionTime;
            }


            return totalTime - m_Queue[0].TimeRunning;
        }

        public void ClearAbility(bool cancelNonBlocking)
        {
            if (m_Queue.Count > 0)
            {
                m_LastUsedTimestamps.Remove(m_Queue[0].AbilityID);
                m_Queue[0].Cancel(m_ServerCharacter);
            }

            //clear the ability queue
            {
                var removedAbilities = ListPool<Ability>.Get();

                foreach (var ability in m_Queue)
                {
                    removedAbilities.Add(ability);
                }

                m_Queue.Clear();

                foreach (var ability in removedAbilities)
                {
                    TryReturnAbility(ability);
                }

                ListPool<Ability>.Release(removedAbilities);
            }
        }

        void TryReturnAbility(Ability ability)
        {
            if (m_Queue.Contains(ability))
            {
                return;
            }

            if (m_NonBlockingAbilities.Contains(ability))
            {
                return;
            }

            AbilityFactory.ReturnAbility(ability);
        }


    }
}
