using FQParty.GamePlay.Character;
using Unity.Netcode;
using UnityEngine;


namespace FQParty.GamePlay.Input
{
    [RequireComponent(typeof(ServerCharacter))]
    public class ClientInputSender : NetworkBehaviour
    {
        GamePlayInputReader m_GameInputReader;

        [SerializeField]
        ServerCharacter m_ServerCharacter;

        [SerializeField]
        ClientCharacter m_ClientCharacter;

    }
}