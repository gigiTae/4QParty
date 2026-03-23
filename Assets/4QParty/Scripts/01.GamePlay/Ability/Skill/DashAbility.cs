using FQParty.GamePlay.Character.Movement;
using FQParty.GamePlay.Character;
using UnityEngine;


namespace FQParty.GamePlay.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/DashAbility")]
    public class DashAbility : Ability
    {
        [SerializeField] float DashSpeed = 20f;
        [SerializeField] float DashDuration = 1f;

        public override AbilityConclusion OnStart(ServerCharacter serverCharacter)
        {
            base.OnStart(serverCharacter);
            return AbilityConclusion.Continue;  
        }

        public override AbilityConclusion OnUpdate(ServerCharacter serverCharacter)
        {
            base.OnUpdate(serverCharacter);

            return AbilityConclusion.Continue;    
        }

        public override void Cancel(ServerCharacter serverCharacter)
        {
            base.Cancel(serverCharacter);
        }

        public override void StartClientMove(ClientCharacter clientCharacter)
        {
            if (clientCharacter.CharacterMovement is IDashable dash)
            {
                dash.StartDash(DashSpeed, DashDuration);
            }
        }
    }

}