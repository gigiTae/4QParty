using FQParty.GamePlay.Character;
using UnityEngine;


namespace FQParty.GamePlay.Abilities.Effects
{
    /// <summary>
    /// 투사체 이펙트
    /// </summary>
    public class ProjectileEffect : ServerAbilityEffect
    {
        [SerializeField] float m_FireTime = 0f;


        public override void OnStart(ServerCharacter serverCharacter, Ability ability)
        {
            if (m_FireTime <= 0f)
            {
                FireProjectile();
                IsActive = false;
            }
        }

        public override void OnUpdate(ServerCharacter serverCharacter, Ability ability)
        {
            if (ability.TimeRunning >= m_FireTime)
            {
                FireProjectile();
                IsActive = false;
            }
        }

        void FireProjectile()
        {

        }

    }

}