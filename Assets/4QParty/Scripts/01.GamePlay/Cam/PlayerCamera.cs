using UnityEngine;

namespace FQParty.GamePlay.Cam
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private PlayerCameraSetting m_Setting;
        private Transform m_TargetTransform;

        void Awake()
        {
            transform.rotation = Quaternion.Euler(m_Setting.Rotation);
        }

        public void SetTarget(Transform target)
        {
            m_TargetTransform = target;

            if (m_TargetTransform != null && m_Setting != null)
            {
                transform.position = m_TargetTransform.position + m_Setting.OffsetPosition;
                transform.rotation = Quaternion.Euler(m_Setting.Rotation);
            }
        }

        void LateUpdate()
        {
            if (m_TargetTransform == null || m_Setting == null) return;

            Vector3 targetPosition = m_TargetTransform.position + m_Setting.OffsetPosition;
            transform.position = targetPosition;
        }
    }
}