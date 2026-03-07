using FQParty.Common.Constant;
using System;
using UnityEngine;
using UnityEngine.UIElements;


namespace FQParty.Session.CommonSession
{
    [UxmlElement]
    public class StartSessionButton : Button
    {
        StartSessionViewModel m_ViewModel;

        public StartSessionButton()
        {
            AddToClassList(UITheme.Button);
            
        }

        void StartSession()
        {
           
        }

        void UpdateBindings()
        {
            CleanupBindings();
            m_ViewModel = new StartSessionViewModel();
        }

        void CleanupBindings()
        {
            if (m_DataBinding.dataSource is IDisposable disposable)
            {
                disposable.Dispose();
            }

            m_ViewModel = null;
            m_DataBinding.dataSource = null;
        }
    }

}