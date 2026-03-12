using UnityEngine;


namespace FQParty.SteamService
{
    [CreateAssetMenu(fileName = "SteamSettings", menuName = "Settings/SteamSettings")]
    public class SteamSettingsSO : ScriptableObject
    {
        [Header("Lobby options")]
        public int MaxPlayer = 4;
    }

}