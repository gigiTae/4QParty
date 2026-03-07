using Unity.Properties;
using System;
using UnityEngine.UIElements;
using FQParty.Common.Constant;

namespace FQParty.Session.CommonSession
{
    [UxmlElement]
    public partial class SessionNameLabel : Label
    {
        [UxmlAttribute, CreateProperty]
        public string SessionType
        {
            get => m_SessionType;
            set
            {
                if (m_SessionType == value)
                {
                    return;
                }

                m_SessionType = value;
                if (panel != null)
                {
                    UpdateBindings();
                }
            }
        }
        string m_SessionType;

        DataBinding m_DataBinding;

        public SessionNameLabel()
        {
            AddToClassList(UITheme.Header);
            m_DataBinding = new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(SessionNameViewModel.SessionName)),
                bindingMode = BindingMode.ToTarget
            };
            SetBinding(new BindingId(nameof(text)), m_DataBinding);

            RegisterCallback<AttachToPanelEvent>(_ => UpdateBindings());
            RegisterCallback<DetachFromPanelEvent>(_ => CleanupBindings());
        }

        void UpdateBindings()
        {
            CleanupBindings();
            m_DataBinding.dataSource = new SessionNameViewModel(m_SessionType);
        }

        void CleanupBindings()
        {
            if (m_DataBinding.dataSource is IDisposable disposable)
            {
                disposable.Dispose();
            }
            m_DataBinding.dataSource = null;
        }
    }
}
