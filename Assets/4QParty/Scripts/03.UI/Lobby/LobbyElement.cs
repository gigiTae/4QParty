using Codice.Client.Common.WebApi.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;



namespace FQParty.UI.Lobby
{
    [UxmlElement]
    public partial class LobbyElement : VisualElement
    {
        Button m_LeaveButton;
        Button m_StartGameButton;
        List<Label> m_PlayerNameLabels = new();

        public event Action OnLeaveClicked;
        public event Action OnStartGameClicked;

        public LobbyElement()
        {
            AddPlayerPartElement();
            AddButton();
        }

        public void AddPlayerPartElement()
        {
            var playerContianer = new VisualElement();

            for (int i = 0; i < 4; ++i)
            {
                var playerNameLabel = new Label { text = "Null" };
                playerContianer.Add(playerNameLabel);
                m_PlayerNameLabels.Add(playerNameLabel);
            }
            Add(playerContianer);
        }

        public void AddButton()
        {
            var leaveButton = new Button
            {
                text = "LEAVE"
            };

            m_LeaveButton = leaveButton;
            m_LeaveButton.clicked += () => OnLeaveClicked?.Invoke();
            Add(leaveButton);

            var startGameButton = new Button
            {
                text = "START GAME"
            };
            startGameButton.enabledSelf = false;
            m_StartGameButton = startGameButton;
            m_StartGameButton.clicked += () => OnStartGameClicked?.Invoke();
            Add(startGameButton);
        }

        public void UpdatePlayerLabel(int index, string name)
        {
            if (m_PlayerNameLabels == null) return;

            if (index < 0 || index >= m_PlayerNameLabels.Count)
            {
                Debug.LogWarning($"[LobbyElement] Index {index} is out of range.");
                return;
            }

            if (m_PlayerNameLabels[index] != null)
            {
                m_PlayerNameLabels[index].text = name;
            }
        }

        public void SetStartButtonActive(bool isActive)
        {
            m_StartGameButton?.SetEnabled(isActive);
        }
    }

}