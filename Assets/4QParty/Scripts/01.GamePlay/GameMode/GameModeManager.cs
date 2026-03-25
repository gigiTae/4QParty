using FQParty.Common.Persistance;
using FQParty.GamePlay.GameMode;
using PlasticGui.WorkspaceWindow.QueryViews;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// GameMode를 관리하는 클래스
/// </summary>
public class GameModeManager : PersistanceNetworkSingleton<GameModeManager>
{
    GameModeBase m_CurrentGameMode;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Register(GameModeBase gameMode)
    {
        m_CurrentGameMode = gameMode;
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
        NetworkManager.SceneManager.OnUnload += OnUnload;
    }

    void OnUnload(ulong clientId, string sceneName, AsyncOperation asyncOperation)
    {
        m_CurrentGameMode = null;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
        NetworkManager.SceneManager.OnUnload -= OnUnload;
    }

    void OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        m_CurrentGameMode?.OnLoadEventCompleted();
    }
}
