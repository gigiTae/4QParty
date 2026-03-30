using FQParty.GamePlay.Abilities;
using FQParty.GamePlay.Input;
using UnityEngine;


namespace FQParty.GamePlay.Settings
{
    public interface ICharacterStatusSettings
    {
        float MaxHp { get; }
        float AttackPower { get; }

        Ability DeadAbility {  get; }
    }

    public interface IPlayerCharacterMovementSettings
    {
        GamePlayInputReader GamePlayInputReader { get; }
        float MoveSpeed { get; }
    }
}