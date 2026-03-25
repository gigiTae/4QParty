using UnityEngine;


namespace FQParty.GamePlay.GameplayObjects
{
    /// <summary>
    /// 플레이어 스폰장소 지정
    /// </summary>
    public class PlayerSpawnPoint : MonoBehaviour
    {
        [Tooltip("스폰하는 플레이어의 인덱스 0~3")]
        [Range(0, 3)]
        [SerializeField] private int m_PlayerIndex = 1;
        public int PlayerIndex => m_PlayerIndex;
    }
}