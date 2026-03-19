using FQParty.GamePlay.Abilities;
using FQParty.GamePlay.Abilities.Effects;
using Unity.Netcode;
using UnityEngine;


namespace FQParty.GamePlay.Character
{
    public class ServerCharacter : NetworkBehaviour
    {
        [SerializeField] ClientCharacter m_ClientCharacter;
        public ClientCharacter ClientCharacter => m_ClientCharacter;

        [SerializeField] AbilityDatabase m_Database;

        [Rpc(SendTo.Server)]
        public void ServerStartAbilityRpc(AbilityPacket packet)
        {
            AbilityData datas = m_Database.Find(packet.AbilityID);
           

        }


    }

}