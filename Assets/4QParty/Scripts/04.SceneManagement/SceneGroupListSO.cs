using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

namespace FQParty.SceneManagement
{
    [CreateAssetMenu(fileName = "SceneGroupList", menuName = "Settings/SceneGroupList")]
    public class SceneGroupListSO : ScriptableObject
    {
        public List<SceneGroup> SceneGroups;
    }
}