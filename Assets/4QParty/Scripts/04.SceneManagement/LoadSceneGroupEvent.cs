 using System;
using UnityEngine;
using FQParty.Common.Event;

namespace FQParty.SceneManagement
{
    [CreateAssetMenu(fileName = "LoadSceneGroupEvent", menuName = "Event/LoadSceneGroupEvent")]
    public class LoadSceneGroupEvent : EventSO<LoadSceneGroupContext> { }
    public struct LoadSceneGroupContext
    {
        public string GroupName;
        public bool UseNetworkSceneManager;
    }
}