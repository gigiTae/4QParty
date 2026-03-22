using FQParty.GamePlay.Character;
using System;
using UnityEngine;


namespace FQParty.GamePlay.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/EmoteAbility")]
    public class EmoteAbility : Ability
    {
        public override bool OnStart(ServerCharacter serverCharacter)
        {
            serverCharacter.NetworkAnimator.SetTrigger(Config.Anim);

            return AbilityConclusion.Stop;
        }

        public override bool OnUpdate(ServerCharacter serverCharacter)
        {
            throw new InvalidOperationException("No logic defined");
        }

        public override void Cancel(ServerCharacter serverCharacter)
        {
            if (!string.IsNullOrEmpty(Config.Anim2))
            {
                serverCharacter.NetworkAnimator.SetTrigger(Config.Anim2);
            }
        }

        public override bool OnUpdateClient(ClientCharacter clientCharacter)
        {
            return AbilityConclusion.Continue;
        }
    }

}