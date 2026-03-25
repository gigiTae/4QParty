using UnityEngine;


namespace FQParty.GamePlay.Cam
{
    [CreateAssetMenu(menuName = "Settings/PlayerCameraSetting")]
    public class PlayerCameraSetting : ScriptableObject
    {
        [Header("Position Offset")]
        public Vector3 OffsetPosition;
        public Vector3 Rotation = new Vector3(45, 0, 0);

    }

}