using UnityEngine;
using UnityEngine.UIElements;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] UIDocument m_UIDocument;

    private void Awake()
    {
        if (m_UIDocument == null)
        {
            m_UIDocument = GetComponent<UIDocument>();  
        }
    }

    public void EnableLoadingScreen(bool enable = true)
    {
        m_UIDocument.rootVisualElement.style.display = enable ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
