using FQParty.GamePlay.Character;
using System;
using UnityEngine;


namespace FQParty.GamePlay.Abilities.Effects
{
    [Serializable]
    public class DespawnAbility : ServerAbilityEffect
    {
        [SerializeField] float m_DespawnTime = 1f;
        private Transform m_MainCameraTransform;

        public override void OnStart(ServerCharacter serverCharacter, Ability ability)
        {
        }

        public override void OnUpdate(ServerCharacter serverCharacter, Ability ability)
        {
            if (ability.TimeRunning >= m_DespawnTime)
            {
                serverCharacter.gameObject.SetActive(false);
                serverCharacter.NetworkObject.Despawn(false);
                IsActive = false;
            }
        }
    }
}