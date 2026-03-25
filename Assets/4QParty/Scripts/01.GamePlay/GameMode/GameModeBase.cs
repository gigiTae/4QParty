using FQParty.GamePlay.GameplayObjects;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace FQParty.GamePlay.GameMode
{
    public abstract class GameModeBase : NetworkBehaviour
    {
        void Awake()
        {
            GameModeManager.Instance.Register(this);
        }



        /// <summary>
        /// 클라이언트와 호스트가 모두(동기화, 씬로드)등이 완료되면 호출합니다
        /// </summary>
        public void OnLoadEventCompleted()
        {
            if (IsServer)
            {
                StartGameMode();
            }
        }

        protected abstract void StartGameMode();
    }
}