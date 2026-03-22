using FQParty.GamePlay.Character;
using System;
using UnityEngine;

namespace FQParty.GamePlay.Abilities
{
    public abstract class BaseAbiltiyInput : MonoBehaviour
    {
        protected ServerCharacter m_PlayerOwner;
        protected Vector3 m_Origin;
        protected AbilityID m_AbilityPrototypeID;
        protected Action<AbilityRequestData> m_SendInput;
        Action m_OnFinished;

       public void Initiate(ServerCharacter playerOwner, Vector3 orgin, AbilityID abilityPrototypeID, Action<AbilityRequestData> onSendInput, Action onFinished)
        {
            m_PlayerOwner = playerOwner;
            m_Origin = orgin;
            m_AbilityPrototypeID = abilityPrototypeID;  
            m_SendInput = onSendInput;
            m_OnFinished = onFinished;
        }

        public void OnDestroy()
        {
            m_OnFinished();
        }

        public virtual void OnReleaseKey() { }
    }
}