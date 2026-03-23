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
                bool keepGoing = ability.AnticipatedClient || ability.OnUpdateClient(ClientCharacter) == AbilityConclusion.Continue;
                //          bool expirable = ability.Config.DurationSeconds > 0f;
                //        bool timeExpired = expirable && ability.TimeRunning >= ability.Config.DurationSeconds;
                //      bool timeOut = ability.AnticipatedClient && ability.TimeRunning >= k_AnticipationTimeoutSeconds;

                //     if (!keepGoing || timeExpired || timeOut)
                {
                    //       if (timeOut)
                    {
                        ability.CancelClient(ClientCharacter);
                    }
                    //     else
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

        public void AnticipateAbility(ref AbilityRequestData data)
        {
            Ability ability = AbilityFactory.CreateAbilityFromData(ref data);
            ability.AnticipateAbilityClient(ClientCharacter);
        }

        public void StartClientMoveAbility(ref AbilityRequestData data)
        {
            Ability ability = AbilityFactory.CreateAbilityFromData(ref data);
            ability.StartClientMove(ClientCharacter);
        }

        public void PlayAbility(ref AbilityRequestData data)
        {
            var anticipatedAbilityIndex = FindAbility(data.AbilityID, true);

            var abilityFX = anticipatedAbilityIndex >= 0 ? m_PlayingAbilities[anticipatedAbilityIndex] : AbilityFactory.CreateAbilityFromData(ref data);
            if (abilityFX.OnStartClient(ClientCharacter) == AbilityConclusion.Continue)
            {
                if (anticipatedAbilityIndex < 0)
                {
                    m_PlayingAbilities.Add(abilityFX);
                }
            }
            else if (anticipatedAbilityIndex >= 0)
            {
                var removedAbility = m_PlayingAbilities[anticipatedAbilityIndex];
                m_PlayingAbilities.RemoveAt(anticipatedAbilityIndex);
                AbilityFactory.ReturnAbility(removedAbility);
            }

        }

        public void CancelAllAbilities()
        {
            foreach (var ability in m_PlayingAbilities)
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