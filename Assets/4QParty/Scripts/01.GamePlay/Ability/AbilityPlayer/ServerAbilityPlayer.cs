using FQParty.GamePlay.Character;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;


namespace FQParty.GamePlay.Abilities
{
    public class ServerAbilityPlayer : NetworkBehaviour
    {
        [SerializeField] ServerCharacter m_ServerCharacter;
        public Ability PlayingAbility => m_PlayingAbility;
        Ability m_PlayingAbility;

        public NetworkList<AbilityTimeStamp> LastUsedTimestamps => m_LastUsedTimestamps;
        NetworkList<AbilityTimeStamp> m_LastUsedTimestamps = new();

        Queue<Ability> m_PendingQueue = new();
        List<Ability> m_NonBlockingAbilities = new();
        Queue<Ability> m_RequestQueue = new();

        [Rpc(SendTo.Server)]
        public void RequestAbilityServerRpc(AbilityRequestData data)
        {
            Ability ability = AbilityFactory.CreateAbilityFromData(ref data);

            if (!CanRequsetAbility(ability)) return;

            m_RequestQueue.Enqueue(ability);
        }

        public bool CanRequsetAbility(Ability ability)
        {
            float reuseTime = ability.Config.ReuseTimeSeconds;

            if (reuseTime <= 0f) return true;

            foreach (AbilityTimeStamp timeStamp in m_LastUsedTimestamps)
            {
                if (timeStamp.ID == ability.AbilityID)
                {
                    float serverTime = NetworkManager.ServerTime.TimeAsFloat;
                    return serverTime - timeStamp.LastUsedTime < reuseTime;
                }
            }
            return true;
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
            ability.TimeStarted = NetworkManager.ServerTime.TimeAsFloat;
            AddTimeStamp(ability.AbilityID);
            ability.OnStartServer(m_ServerCharacter);
            return ability.IsEndServer();
        }

        void AddTimeStamp(AbilityID abilityID)
        {
            for (int i = 0; i < m_LastUsedTimestamps.Count; ++i)
            {
                if (m_LastUsedTimestamps[i].ID == abilityID)
                {
                    m_LastUsedTimestamps.Set(i, new AbilityTimeStamp()
                    {
                        ID = abilityID,
                        LastUsedTime = NetworkManager.ServerTime.TimeAsFloat
                    });
                    return;
                }
            }

            m_LastUsedTimestamps.Add(new AbilityTimeStamp
            {
                ID = abilityID,
                LastUsedTime = NetworkManager.ServerTime.TimeAsFloat
            });
        }

        public void Update()
        {
            ProcessRequsetAbility();

            AbilityConclusion conclusion = AbilityConclusion.Continue;

            if (m_PlayingAbility != null)
            {
                m_PlayingAbility.OnUpdateServer(m_ServerCharacter);
                conclusion = m_PlayingAbility.IsEndServer();
            }
            else if (m_PendingQueue.Count > 0)
            {
                Ability ability = m_PendingQueue.Dequeue();
                conclusion = StartAbility(ability);
                m_PlayingAbility = ability;
            }

            if (conclusion == AbilityConclusion.Stop)
            {
                m_PlayingAbility.OnEndServer(m_ServerCharacter);
                TryReturnAbility(m_PlayingAbility);
                m_PlayingAbility = null;
            }

            foreach (var nonBlockAbility in m_NonBlockingAbilities)
            {
                nonBlockAbility.OnUpdateServer(m_ServerCharacter);
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
                m_PlayingAbility.OnCanceledServer(m_ServerCharacter);
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

        public void OnAnimationStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_PlayingAbility?.OnAnimationStateExit(animator, stateInfo, layerIndex);    

            foreach(var nonBlockAbility in m_NonBlockingAbilities)
            {
                nonBlockAbility.OnAnimationStateExit(animator, stateInfo, layerIndex);
            }

        }

    }
}
