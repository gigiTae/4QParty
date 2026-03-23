using FQParty.GamePlay.Abilities.Effects;
using FQParty.GamePlay.Character;
using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;


namespace FQParty.GamePlay.Abilities
{
    public abstract class Ability : ScriptableObject
    {
        [NonSerialized]
        public AbilityID AbilityID;

        protected AbilityRequestData m_Data;

        [SerializeField] public AbilityConfig Config;
        [SerializeReference] private List<AbilityEffect> m_Effects;

        /// <summary>
        /// 어빌리티 실행 시작 시간 
        /// </summary>
        public float TimeStarted { get; set; }

        public float TimeRunning { get { return (Time.time - TimeStarted); } }

        public void Initialize(ref AbilityRequestData data)
        {
            m_Data = data;
            AbilityID = m_Data.AbilityID;
        }

        public virtual void Reset()
        {
            m_Data = default;
            AbilityID = default;
            TimeStarted = 0;
        }

        public virtual AbilityConclusion OnStart(ServerCharacter serverCharacter)
        {
            foreach (var effect in m_Effects) 
            {
                effect.OnStart(serverCharacter, this);
            }

            return AbilityConclusion.Continue;
        }

        public virtual AbilityConclusion OnUpdate(ServerCharacter serverCharacter)
        {
            foreach (var effect in m_Effects)
            {
                effect.OnUpdate(serverCharacter, this);
            }

            return AbilityConclusion.Continue;
        }

        /// <summary>
        /// 어빌리티가 조건에 의해서 정상적으로 종료된 경우 호출합니다
        /// </summary>
        public virtual void End(ServerCharacter serverCharacter)
        {
            Cancel(serverCharacter);

            foreach(var effect in m_Effects)
            {
                effect.End(serverCharacter, this);
            }
        }

        /// <summary>
        /// 어빌리티가 취소되면 호출합니다
        /// </summary>
        public virtual void Cancel(ServerCharacter serverCharacter)
        {
            foreach (var effect in m_Effects)
            {
                effect.Cancel(serverCharacter, this);
            }
        }

        public bool AnticipatedClient { get; protected set; }
  
        public virtual AbilityConclusion OnStartClient(ClientCharacter clientCharacter)
        {
            return AbilityConclusion.Continue;
        }

        public virtual AbilityConclusion OnUpdateClient(ClientCharacter clientCharacter)
        {
            return AbilityConclusion.Continue;
        }

        public virtual void EndClient(ClientCharacter clientCharacter)
        {
            CancelClient(clientCharacter);
        }

        public virtual void CancelClient(ClientCharacter clientCharacter) { }

        public virtual void AnticipateAbilityClient(ClientCharacter clientCharacter)
        {
            AnticipatedClient = true;
            TimeStarted = UnityEngine.Time.time;
        }

        public virtual void StartClientMove(ClientCharacter clientCharacter)
        {}

    }

}