using FQParty.GamePlay.Character;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;

namespace FQParty.GamePlay.Abilities
{
    public sealed class ClientAbilityPlayer : NetworkBehaviour
    {
        [SerializeField] ClientCharacter m_ClientCharacter;

        public Ability PlayingAbility => m_PlayingAbility;
        Ability m_PlayingAbility;

        Queue<Ability> m_PendingQueue = new();
        List<Ability> m_NonBlockingAbilities = new();
        Queue<Ability> m_RequestQueue = new();

        public void RequestAbility(AbilityRequestData data)
        {
            Ability ability = AbilityFactory.CreateAbilityFromData(ref data);

            m_RequestQueue.Enqueue(ability);
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
            ability.OnStartClient(m_ClientCharacter);
            return ability.IsEndClient();
        }

        public void Update()
        {
            ProcessRequsetAbility();

            AbilityConclusion conclusion = AbilityConclusion.Continue;

            if (m_PlayingAbility != null)
            {
                m_PlayingAbility.OnUpdateClient(m_ClientCharacter);
                conclusion = m_PlayingAbility.IsEndClient();
            }
            else if (m_PendingQueue.Count > 0)
            {
                Ability ability = m_PendingQueue.Dequeue();
                conclusion = StartAbility(ability);     
                m_PlayingAbility = ability;
            }

            if (conclusion == AbilityConclusion.Stop)
            {
                m_PlayingAbility.OnEndClient(m_ClientCharacter);
                TryReturnAbility(m_PlayingAbility);
                m_PlayingAbility = null;
            }

            foreach (var nonBlockAbility in m_NonBlockingAbilities)
            {
                nonBlockAbility.OnUpdateClient(m_ClientCharacter);
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
                m_PlayingAbility.OnCanceledClient(m_ClientCharacter);
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