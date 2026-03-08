using FQParty.Common.Constant;
using System;
using UnityEngine;
using UnityEngine.UIElements;


namespace FQParty.Session.CommonSession
{
    [UxmlElement]
    public partial class StartSessionButton : Button
    {
        StartSessionViewModel m_ViewModel;

        public StartSessionButton()
        {
            AddToClassList(UITheme.Button);

            clicked += StartSession;
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
        }
    }

}