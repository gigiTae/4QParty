using FQParty.Common.Event;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace FQParty.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] UIDocument m_Document;
        [SerializeField] float m_FillSpeed = 0.5f;
        [SerializeField] LoadSceneGroupEvent m_LoadSceneGroupEvent;
        [SerializeField] SceneGroupListSO m_SceneGroupList;

        ProgressBar m_ProgressBar;

        float m_TargetProgress;
        bool m_IsLoading;

        public readonly SceneGroupManager m_Manager = new SceneGroupManager();

        void Awake()
        {
            m_ProgressBar = m_Document.rootVisualElement.Q<ProgressBar>();

            m_LoadSceneGroupEvent.Subscribe(OnLoadSceneGroup);

            m_Manager.OnSceneLoaded += sceneName => Debug.Log("Loaded: " + sceneName);
            m_Manager.OnSceneUnloaded += sceneName => Debug.Log("Unloaded: " + sceneName);
            m_Manager.OnSceneGroupLoaded += () => Debug.Log("Scene group loaded");
        }

        void OnLoadSceneGroup(LoadSceneGroupContext context)
        {
            if (m_IsLoading)
            {
                Debug.LogWarning("현재 다른 씬그룹을 로딩하고 있습니다");
                return;
            }

            var index = m_SceneGroupList.SceneGroups.FindIndex(g => g.GroupName == context.GroupName);

            if (index != -1)
            {
                _ = LoadSceneGroup(index);
            }
            else
            {
                Debug.LogWarning($"{context.GroupName}의 씬 그룹을 찾을 수 없습니다");
            }
        }

        async void Start()
        {
            await LoadSceneGroup(0);
        }

        void Update()
        {
            if (!m_IsLoading) return;

            float currentFillAmount = m_ProgressBar.value;
            float progressDifference = Mathf.Abs(currentFillAmount - m_TargetProgress);

            float dynamicFillSpeed = progressDifference * m_FillSpeed;

            m_ProgressBar.value = Mathf.Lerp(currentFillAmount, m_TargetProgress, Time.deltaTime * dynamicFillSpeed);
        }

        public async Task LoadSceneGroup(int index)
        {
            m_ProgressBar.value = 0f;
            m_TargetProgress = 0f;

            if (index < 0 || index >= m_SceneGroupList.SceneGroups.Count)
            {
                Debug.LogError("Invalid scene group index: " + index);
                return;
            }

            LoadingProgress progress = new LoadingProgress();
            progress.Progressed += target => m_TargetProgress = Mathf.Max(target, m_TargetProgress);

            EnableLoadingScreen(true);
            await m_Manager.LoadScenes(m_SceneGroupList.SceneGroups[index], progress);
            EnableLoadingScreen(false);
        }

        void EnableLoadingScreen(bool enable = true)
        {
            m_IsLoading = enable;
            m_Document.rootVisualElement.style.display = enable ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> Progressed;

        const float ratio = 1f;

        public void Report(float value)
        {
            Progressed?.Invoke(value / ratio);
        }
    }
}