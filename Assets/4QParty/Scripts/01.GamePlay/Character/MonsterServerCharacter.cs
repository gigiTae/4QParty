using FQParty.GamePlay.Abilities;
using FQParty.GamePlay.Character.Movement;
using FQParty.GamePlay.Settings;
using NUnit.Framework;
using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;


namespace FQParty.GamePlay.Character
{
    public class MonsterServerCharacter : ServerCharacter
    {
        [SerializeField] MonsterCharacterSettings m_Settings;

        private void Awake()
        {
            BindSettings();
        }

        void BindSettings()
        {
            if (CharacterStatus != null)
            {
                CharacterStatus.BindSettings(m_Settings);
            }
        }

    }


}