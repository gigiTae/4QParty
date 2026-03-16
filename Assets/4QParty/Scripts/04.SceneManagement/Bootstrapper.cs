using FQParty.Common.Constant;
using FQParty.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임 시작을 세팅 클래스
/// </summary>
public class Bootstrapper : MonoBehaviour
{
    private void Start()
    {
        SceneLoader.Instance.LoadScene(SceneTheme.k_Main, false);
    }
}
