using Unity.Netcode;
using UnityEngine;

namespace FQParty.Common.Persistance
{
    // T를 NetworkBehaviour로 제한하여 타입 안정성 확보
    public class PersistanceNetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
    {
        public bool UnparentOnAwake = true;
        protected static T m_Instance;

        public static bool HasInstance => m_Instance != null;
        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = FindFirstObjectByType<T>();

                    // 네트워크 싱글톤은 동적 생성보다 씬에 미리 존재하거나 
                    // 프리팹을 통한 스폰을 권장하므로 경고를 띄워주는 것이 좋습니다.
                    if (m_Instance == null)
                    {
                        Debug.LogWarning($"[Singleton] {typeof(T).Name} instance is missing!");
                    }
                }
                return m_Instance;
            }
        }

        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying) return;

            if (m_Instance == null)
            {
                m_Instance = this as T;
                if (UnparentOnAwake) transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            else if (m_Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public override void OnDestroy()
        {
            if (m_Instance == this)
            {
                m_Instance = null;
            }
            base.OnDestroy();
        }
    }
}