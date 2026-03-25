using FQParty.GamePlay.Character;
using UnityEngine;


namespace FQParty.GamePlay.GameplayObjects
{
    public interface IInteractable
    {
        bool IsInteractable { get; }

        void Interact(ServerCharacter serverCharacter);
    }

}