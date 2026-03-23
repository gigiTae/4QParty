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

        public Ability PlayingAbility => m_PlayingAbility;
        Ability m_PlayingAbility;

        Queue<Ability> m_PendingQueue;

        List<Ability> m_NonBlockingAbilities;

        Dictionary<AbilityID, float> m_LastUsedTimestamps;

        Queue<Ability> m_RequestQueue = new();

        public void RequestAbility(AbilityRequestData data)
        {
            Ability ability = AbilityFactory.CreateAbilityFromData(ref data);

            // Check Cooltime
            float reuseTime = ability.Config.ReuseTimeSeconds;
            if (reuseTime > 0 &&
                m_LastUsedTimestamps.TryGetValue(ability.AbilityID, out float lastTimeUsed) &&
                Time.time - lastTimeUsed < reuseTime)
            {
                Debug.Log("¾îºô¸®Æ¼ ÄðÅ¸ÀÓ");
                return;
            }

            m_RequestQueue.Enqueue(ability);
        }

        public ServerAbilityPlayer(ServerCharacter serverCharacter)
        {
            m_ServerCharacter = serverCharacter;
            m_PendingQueue = new();
            m_NonBlockingAbilities = new();
            m_LastUsedTimestamps = new();
        }

        public void PlayAbility(Ability ability)
        {
            switch (ability.Config.PlayType)
            {
                case AbilityPlayType.Cancel:
                    {
                        CancelPlayingAbility();
                        m_PendingQueue.Enqueue(ability);
                        break;
                    }
                case AbilityPlayType.Queue:
                    {
                        m_PendingQueue.Enqueue(ability);
                        break;
                    }
                case AbilityPlayType.NonBlocking:
                    {
                        StartAbility(ability);
                        m_NonBlockingAbilities.Add(ability);
                        break;
                    }
            }
        }

        AbilityConclusion StartAbility(Ability ability)
        {
            ability.TimeStarted = Time.time;
            m_LastUsedTimestamps[ability.AbilityID] = Time.time;
            return ability.OnStart(m_ServerCharacter);
        }

        public void OnUpdateAbility()
        {
            ProcessRequsetAbility();

            AbilityConclusion conclusion = AbilityConclusion.Continue;

            if (m_PlayingAbility != null)
            {
                conclusion = m_PlayingAbility.OnUpdate(m_ServerCharacter);
            }
            else if (m_PendingQueue.Count > 0)
            {
                Ability ability = m_PendingQueue.Dequeue();
                conclusion = StartAbility(ability);
                m_PlayingAbility = ability;
            }

            if (conclusion == AbilityConclusion.Stop)
            {
                m_PlayingAbility.End(m_ServerCharacter);
                TryReturnAbility(m_PlayingAbility);
                m_PlayingAbility = null;
            }

            foreach (var nonBlockAbility in m_NonBlockingAbilities)
            {
                nonBlockAbility.OnUpdate(m_ServerCharacter);
            }
        }


        void ProcessRequsetAbility()
        {
            int count = m_RequestQueue.Count;

            if (count == 0) return;

            for (int i = 0; i < count; i++)
            {
                var data = m_RequestQueue.Dequeue();
                PlayAbility(data);
            }
        }

        public void CancelPlayingAbility()
        {
            var removedAbilities = ListPool<Ability>.Get();

            if (m_PlayingAbility != null)
            {
                m_PlayingAbility.Cancel(m_ServerCharacter);
                removedAbilities.Add(m_PlayingAbility);
                m_PlayingAbility = null;
            }

            foreach (var ability in m_PendingQueue)
            {
                removedAbilities.Add(ability);
            }
            m_PendingQueue.Clear();

            foreach (var ability in removedAbilities)
            {
                TryReturnAbility(ability);
            }

            ListPool<Ability>.Release(removedAbilities);
        }

        void TryReturnAbility(Ability ability)
        {
            if (m_PlayingAbility == ability ||
                m_PendingQueue.Contains(ability) ||
                m_NonBlockingAbilities.Contains(ability))
            {
                return;
            }

            AbilityFactory.ReturnAbility(ability);
        }


    }
}
