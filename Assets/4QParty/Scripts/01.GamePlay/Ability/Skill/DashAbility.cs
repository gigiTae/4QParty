using FQParty.GamePlay.Character;
using UnityEngine;


namespace FQParty.GamePlay.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/DashAbility")]
    public class DashAbility : Ability
    {
        public override AbilityConclusion OnStart(ServerCharacter serverCharacter)
        {
            base.OnStart(serverCharacter);

            serverCharacter.CharacterMovement.SetMovementStateServerRpc(CharacterMovement.MovementState.Stop);

            return AbilityConclusion.Continue;  
        }

        public override AbilityConclusion OnUpdate(ServerCharacter serverCharacter)
        {
            base.OnUpdate(serverCharacter);

            return AbilityConclusion.Stop;    
        }

        public override void Cancel(ServerCharacter serverCharacter)
        {
            base.Cancel(serverCharacter);

            serverCharacter.CharacterMovement.SetMovementStateServerRpc(CharacterMovement.MovementState.Moveable);
        }

    }

}