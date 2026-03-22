using FQParty.GamePlay.Character;
using UnityEngine;


namespace FQParty.GamePlay.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/DashAbility")]
    public class DashAbility : Ability
    {
        public override bool OnStart(ServerCharacter serverCharacter)
        {
            serverCharacter.NetworkAnimator.SetTrigger(Config.Anim);

            return AbilityConclusion.Continue;
        }

        public override bool OnUpdate(ServerCharacter serverCharacter)
        {
            return AbilityConclusion.Continue;    
        }


    }

}