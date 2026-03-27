using UnityEngine;
using System.Collections.Generic;
using FQParty.GamePlay.Character;
using Unity.Netcode;

namespace FQParty.GamePlay.Abilities
{
    public sealed class ClientAbilityPlayer : NetworkBehaviour
    {
        List<Ability> m_PlayingAbilities = new();


        [SerializeField]
        ClientCharacter m_ClientCharacter;

        public void Update()
        {
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
            ability.AnticipateAbilityClient(m_ClientCharacter);
        }

        public void StartClientMoveAbility(ref AbilityRequestData data)
        {
            Ability ability = AbilityFactory.CreateAbilityFromData(ref data);
            ability.StartClientMove(m_ClientCharacter);
        }

        public void PlayAbility(ref AbilityRequestData data)
        {
            var anticipatedAbilityIndex = FindAbility(data.AbilityID, true);

            var abilityFX = anticipatedAbilityIndex >= 0 ? m_PlayingAbilities[anticipatedAbilityIndex] : AbilityFactory.CreateAbilityFromData(ref data);
            if (abilityFX.OnStartClient(m_ClientCharacter) == AbilityConclusion.Continue)
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
                ability.CancelClient(m_ClientCharacter);
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
                    ability.CancelClient(m_ClientCharacter);
                    m_PlayingAbilities.RemoveAt(i);
                    AbilityFactory.ReturnAbility(ability);
                }
            }
        }


    }

}