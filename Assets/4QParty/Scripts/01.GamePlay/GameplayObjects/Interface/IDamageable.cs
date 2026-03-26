using FQParty.GamePlay.Character;
using UnityEngine;


namespace FQParty.GamePlay.GameplayObjects
{
    public interface IDamageable
    {
        bool IsDamageable();

        void ReceiveDamage(ServerCharacter serverCharacter, float value);
    }

}