using FQParty.GamePlay.Abilities.Effects;
using FQParty.GamePlay.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FQParty.GamePlay.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Ability")]
    public class Ability : ScriptableObject
    {
        #region Variables & Properties

        [Header("Settings")]
        [SerializeField] public AbilityConfig Config;
        [SerializeReference] private List<AbilityEffect> m_Effects = new();

        [NonSerialized] public AbilityID AbilityID;
        protected AbilityRequestData m_Data;

        public float TimeStarted { get; set; }
        public float TimeRunning => Time.time - TimeStarted;
        public bool AnticipatedClient { get; protected set; }

        #endregion

        #region Initialization & Lifecycle

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
            AnticipatedClient = false;
        }

        public virtual Ability Clone()
        {
            Ability ability = Instantiate(this);
            ability.m_Effects = m_Effects.Select(e => e.Clone()).ToList();
            return ability;
        }

        #endregion

        #region Server Logic (Authority)

        public virtual void OnStart(ServerCharacter serverCharacter)
        {
            m_Effects.ForEach(e => e.OnStart(serverCharacter, this));
         }

        public virtual void OnUpdate(ServerCharacter serverCharacter)
        {
            m_Effects.ForEach(e => e.OnUpdate(serverCharacter, this));
        }

        public virtual AbilityConclusion IsEnd()
        {
            if (m_Effects.Count == 0) return AbilityConclusion.Stop;

            bool shouldStop = Config.EndPolicy switch
            {
                AbilityEndPolicy.AnyEffectCompleted => m_Effects.Any(e => !e.IsActive),
                AbilityEndPolicy.AllEffectCompleted => m_Effects.All(e => !e.IsActive),
                _ => true
            };

            return shouldStop ? AbilityConclusion.Stop : AbilityConclusion.Continue;
        }

        /// <summary> 어빌리티가 정상 종료될 때 호출 </summary>
        public virtual void End(ServerCharacter serverCharacter)
        {
            Cancel(serverCharacter);
            m_Effects.ForEach(e => e.End(serverCharacter, this));
        }

        /// <summary> 어빌리티가 강제 취소될 때 호출 </summary>
        public virtual void Cancel(ServerCharacter serverCharacter)
        {
            m_Effects.ForEach(e => e.Cancel(serverCharacter, this));
        }

        #endregion

        #region Client Logic (Prediction/Visual)

        public virtual void AnticipateAbilityClient(ClientCharacter clientCharacter)
        {
            AnticipatedClient = true;
            TimeStarted = Time.time;
        }

        public virtual AbilityConclusion OnStartClient(ClientCharacter clientCharacter) => AbilityConclusion.Continue;

        public virtual AbilityConclusion OnUpdateClient(ClientCharacter clientCharacter) => AbilityConclusion.Continue;

        public virtual void EndClient(ClientCharacter clientCharacter) => CancelClient(clientCharacter);

        public virtual void CancelClient(ClientCharacter clientCharacter) { }

        public virtual void StartClientMove(ClientCharacter clientCharacter) { }

        #endregion

        #region Animation Callbacks

        public void OnAnimationStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_Effects.ForEach(e => e.OnAnimationStateExit(animator, stateInfo, layerIndex));
        }

        #endregion
    }
}