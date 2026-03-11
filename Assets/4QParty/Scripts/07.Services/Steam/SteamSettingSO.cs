using UnityEngine;


namespace FQParty.Services.Steam
{
    [CreateAssetMenu(fileName = "SteamSettings", menuName = "Settings/SteamSettings")]
    public class SteamSettingsSO : ScriptableObject
    {
        [Header("Lobby options")]
        public int MaxPlayer = 4;



    }

}