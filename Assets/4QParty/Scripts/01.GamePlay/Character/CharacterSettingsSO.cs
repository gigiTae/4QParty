using UnityEngine;

namespace FQParty.GamePlay.Character
{

    [CreateAssetMenu(fileName = "CharacterSettings", menuName = "GamePlay/CharacterSettings")]
    public class CharacterSettingsSO : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField] float m_MoveSpeed = 5.0f;

        public float MoveSpeed => m_MoveSpeed;
    }
}