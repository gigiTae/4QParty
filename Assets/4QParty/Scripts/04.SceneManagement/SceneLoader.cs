using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace FQParty.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] UIDocument m_Document;
        [SerializeField] SceneGroup[] m_SceneGroups;
        [SerializeField] float m_FillSpeed = 0.5f;

        ProgressBar m_ProgressBar;

        float m_TargetProgress;
        bool m_IsLoading;

        public readonly SceneGroupManager manager = new SceneGroupManager();

        void Awake()
        {
            m_ProgressBar = m_Document.rootVisualElement.Q<ProgressBar>();

            manager.OnSceneLoaded += sceneName => Debug.Log("Loaded: " + sceneName);
            manager.OnSceneUnloaded += sceneName => Debug.Log("Unloaded: " + sceneName);
            manager.OnSceneGroupLoaded += () => Debug.Log("Scene group loaded");
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

            if (index < 0 || index >= m_SceneGroups.Length)
            {
                Debug.LogError("Invalid scene group index: " + index);
                return;
            }

            LoadingProgress progress = new LoadingProgress();
            progress.Progressed += target => m_TargetProgress = Mathf.Max(target, m_TargetProgress);

            EnableLoadingScreen(true);
            await manager.LoadScenes(m_SceneGroups[index], progress);
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