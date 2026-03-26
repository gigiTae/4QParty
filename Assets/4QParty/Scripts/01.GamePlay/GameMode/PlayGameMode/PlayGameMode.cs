using FQParty.GamePlay.Cam;
using FQParty.GamePlay.Events;
using FQParty.GamePlay.GameMode;
using FQParty.GamePlay.GameplayObjects;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace FQParty.GamePlay.GameMode
{
    public class PlayGameMode : GameModeBase
    {
        [SerializeField] SpawnSystem m_PlayerSpwanSystem;

        protected override void StartGameMode()
        {
            m_PlayerSpwanSystem.StartSpwan();
        }

    }

}