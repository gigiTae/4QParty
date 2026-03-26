using FQParty.GamePlay.Events;
using UnityEngine;


namespace FQParty.GamePlay.GameMode
{
    [CreateAssetMenu(menuName = "GameMode/PlayGameModeSettings")]
    public class PlayGameModeSettings : ScriptableObject
    {
        [Header("스폰 세팅")]

        [Tooltip("시작시 플레이어 캐릭터입니다")]
        public GameObject StartPlayerCharacter;
        
        public RequestPossessEvent RequestPossessEvent;
    }
}