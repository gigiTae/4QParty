using Blocks.Sessions.Common;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;


namespace FQParty.Network
{
    [UxmlElement]
    public partial class CreateSessionElement : VisualElement
    {
        private SessionSettings m_SessionSettings;
        private CreateSessionViewModel m_ViewModel;
        readonly ListPropertyBag<DataBinding> m_Bindings = new();

        [CreateProperty, UxmlAttribute]
        public SessionSettings SessionSettings
        {
            get => m_SessionSettings;
            set
            {
                if (m_SessionSettings == value) return;

                m_SessionSettings = value;
                if (panel != null)
                {
                    UpdateBindings();
                }
            }
        }

        public CreateSessionElement()
        {

        }

      


        void UpdateBindings()
        {

        }

        void CleanupBindings()
        {

        }
    }
}