using Codice.Client.BaseCommands;
using FQParty.GamePlay.Abilities.Effects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FQParty.GamePlay.Abilities
{
    [CreateAssetMenu(fileName = "AbilityData", menuName = "Abilities/AbilityData")]
    public class AbilityData : ScriptableObject
    {
        private ulong m_AbilityID;
        [Tooltip("네트워크 전송 및 데이터 식별을 위한 고유 ID입니다. 0일 경우 자동으로 생성됩니다.")]
        public ulong AbilityID => m_AbilityID;

        [Tooltip("Ability의 효과들을 설정합니다.")]
        [SerializeReference] public List<IEffect> EffectList;

        private void OnValidate()
        {
            if (AbilityID == 0)
            {
                m_AbilityID = GenerateRandomID();
            }
        }
        private static ulong GenerateRandomID()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToUInt64(buffer, 0);
        }
    }

}