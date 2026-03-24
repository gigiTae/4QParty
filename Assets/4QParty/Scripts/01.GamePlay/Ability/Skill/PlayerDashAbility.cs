using FQParty.GamePlay.Character.Movement;
using FQParty.GamePlay.Character;
using UnityEngine;


namespace FQParty.GamePlay.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/PlayerDash")]
    public class PlayerDashAbility : Ability
    {
        [SerializeField] float DashSpeed = 20f;
        [SerializeField] float DashDuration = 1f;

        public override void OnStart(ServerCharacter serverCharacter)
        {
            base.OnStart(serverCharacter);
        }

        public override void OnUpdate(ServerCharacter serverCharacter)
        {
            base.OnUpdate(serverCharacter);
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