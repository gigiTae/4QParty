using UnityEngine;


namespace FQParty.Common.Setting
{
    [CreateAssetMenu(fileName = "BuildSceneList", menuName = "Setting/BuildSceneList")]
    public class BuildSceneList : ScriptableObject
    {
        public string MainScene;

        public string SettingScene;

        public string LobbyScene;

        public string GameScene;
    }
}