using UnityEngine;
using Steamworks;


public class SteamManager : MonoBehaviour
{
    private static SteamManager _instance;
    public static bool Initialized { get; private set; }
    private void Awake()
    {
        // 싱글톤 설정: 하나만 존재하게 함
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        // 스팀 API 초기화
        try
        {
            if (SteamAPI.RestartAppIfNecessary((AppId_t)480)) // 480은 테스트용 AppID
            {
                Application.Quit();
                return;
            }
        }
        catch (System.DllNotFoundException e)
        {
            Debug.LogError("[Steamworks.NET] Steam dll을 찾을 수 없습니다. " + e);
            return;
        }

        Initialized = SteamAPI.Init();
        if (!Initialized)
        {
            Debug.LogError("[Steamworks.NET] 스팀 초기화 실패! (스팀이 꺼져있거나 AppID 설정 오류)");
        }
        else
        {
            Debug.Log("[Steamworks.NET] 스팀 연결 성공!");
        }

        CreateLobby();
    }

    private void Update()
    {
        if (!Initialized) return;

        // 매 프레임 스팀 콜백(이벤트)을 처리해야 매치메이킹 등이 작동함
        SteamAPI.RunCallbacks();
    }

    private void OnDestroy()
    {
        if (_instance != this) return;
        SteamAPI.Shutdown();
    }
    public void CreateLobby()
    {
        if (!SteamManager.Initialized) return;

        // 공용 로비, 최대 4명 생성 요청
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 4);
        Debug.Log("로비 생성 요청을 보냈습니다...");
    }
}