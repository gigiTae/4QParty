using FQParty.Common.Event;
using FQParty.GamePlay.Character;
using FQParty.GamePlay.GameplayObjects;
using UnityEngine;

namespace FQParty.GamePlay.Events
{
    /// <summary>
    /// 플레이어 빙의 요청 이벤트 
    /// </summary>
    [CreateAssetMenu(menuName = "GameEvent/RequestPossessEvent")]
    public class RequestPossessEvent : EventSO<RequestPossessContext> {}

    public struct RequestPossessContext
    {
        // 빙의 오브젝트 프리팹
        public PossessableObject PossessableObject;
  
        // 빙의 요청한 캐릭터
        public ServerCharacter RequsetObject;
    }
}