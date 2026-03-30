using FQParty.GamePlay.Character;
using UnityEngine;
using UnityEngine.UIElements;


namespace FQParty.UI.Play
{
    public class HpUI : MonoBehaviour
    {
        UIDocument m_UIDocument;
        ProgressBar m_ProgressBar;

        Transform m_MainCameraTransform;

        CharacterStatus m_CharacterStatus;

        private void Awake()
        {
            m_CharacterStatus = GetComponentInParent<CharacterStatus>();

            if (m_CharacterStatus != null)
            {
                m_CharacterStatus.OnHpChangedEvent += OnHpChanged;
            }

            m_UIDocument = GetComponent<UIDocument>();

            if (m_UIDocument != null)
            {
                m_ProgressBar = m_UIDocument.rootVisualElement.Q<ProgressBar>();
            }

            m_MainCameraTransform = Camera.main.transform;

        }

        void OnHpChanged(float previousValue, float newValue)
        {
            m_ProgressBar.value = newValue;
        }

        void LateUpdate()
        {
            //transform.LookAt(m_MainCameraTransform.position);
              transform.LookAt(transform.position + m_MainCameraTransform.forward);    
        }

    }

}