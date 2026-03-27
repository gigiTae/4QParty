using System;
using UnityEngine;

namespace FQParty.GamePlay.Input
{
    [Serializable]
    public enum AbilityInputType
    {
        Trigger, // 버튼 누름  
        Release, // 버튼 땜
        TriggerAndDirection, // 버튼 누름과 방향
    }
}