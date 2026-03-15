using UnityEngine;

namespace FQParty.Common.Persistance
{
    public class PersistanceSingleton<T> : MonoBehaviour where T : Component
    {
        public bool UnparentOnAwake = true;
        public static bool HasInstance => m_Instance != null;
        public static T Current => m_Instance;

        protected static T m_Instance;

        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = FindFirstObjectByType<T>();
                    if (m_Instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name + "AutoCreated";
                        m_Instance = obj.AddComponent<T>();
                    }
                }
                return m_Instance;
            }
        }

        protected virtual void Awake() => InitializeSingleton();

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (UnparentOnAwake)
            {
                transform.SetParent(null);
            }

            if (m_Instance == null)
            {
                m_Instance = this as T;
                DontDestroyOnLoad(transform.gameObject);
                enabled = true;
            }
            else
            {
                if (this != m_Instance)
                {
                    Destroy(this.gameObject);
                }
            }
        }

    }

}