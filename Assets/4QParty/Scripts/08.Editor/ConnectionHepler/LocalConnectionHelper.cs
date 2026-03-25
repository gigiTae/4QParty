using FQParty.Common.Constant;
using FQParty.ConnectionManagement;
using FQParty.SceneManagement;
using System.Numerics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


namespace FQParty.Editor.EdiotrHepler
{
    /// <summary>
    /// Editor 환경에서 테스트하기 위한 헬퍼
    /// </summary>
    public class LocalConnectionHelper : MonoBehaviour
    {
        bool IsHost = false;
        bool IsClient = false;

        private void Awake()
        {
            Debug.Log("키보드 H로 호스트 시작, C로 클라이언트 시작, S로 게임시작");
        }

        void Update()
        {
            if (IsHost && SceneManager.GetActiveScene().name == SceneTheme.k_TestLobby)
            {
                if (Keyboard.current.sKey.wasPressedThisFrame)
                {
                    Debug.Log("유니티 모드로 게임을 시작합니다");
                    SceneLoader.Instance.LoadScene(SceneTheme.k_GamePlay, true);
                }
            }

            if (!IsHost || !IsClient)
            {
                // Host or Client 로 시작 
                if (Keyboard.current.hKey.wasPressedThisFrame)
                {
                    IsHost = true;
                    Debug.Log("유니티 모드로 호스트를 시작합니다");
                    ConnectionManager.Instance.StartUnityHostSession();
                }

                if (Keyboard.current.cKey.wasPressedThisFrame)
                {
                    IsClient = true;    
                    Debug.Log("유니티 모드로 클라이언트를 시작합니다");
                    ConnectionManager.Instance.StartUnityClientSession();
                }
            }
        }
    }

}