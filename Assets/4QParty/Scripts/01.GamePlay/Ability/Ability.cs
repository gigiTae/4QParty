using FQParty.GamePlay.Abilities.Effects;
using FQParty.GamePlay.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace FQParty.GamePlay.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Ability")]
    public class Ability : ScriptableObject, IResolver
    {
        [Header("Settings")]
        [SerializeField] public AbilityConfig Config;
        [SerializeReference] List<ServerAbilityEffect> m_ServerEffects = new();
        [SerializeReference] List<ClientAbilityEffect> m_ClientEffects = new();

        [NonSerialized] public AbilityID AbilityID;
        protected AbilityRequestData m_Data;
        public float TimeStarted { get; set; }
        public float TimeRunning
        {
            get
            {
                return NetworkManager.Singleton.ServerTime.TimeAsFloat - TimeStarted;
            }
        }

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

        public virtual Ability Clone()
        {
            Ability ability = Instantiate(this);
            ability.m_ServerEffects = m_ServerEffects.Select(e => e.Clone()).ToList();
            ability.m_ClientEffects = m_ClientEffects.Select(e => e.Clone()).ToList();
            return ability;
        }

        // ========================= Server =================================

        public virtual void OnStartServer(ServerCharacter serverCharacter)
        {
            m_ServerEffects.ForEach(e =>
            {
                e.IsActive = true;
                e.OnStart(serverCharacter, this);
            });
        }

        public virtual void OnUpdateServer(ServerCharacter serverCharacter)
        {
            m_ServerEffects.ForEach(e =>
            {
                if (e.IsActive) e.OnUpdate(serverCharacter, this);
            });
        }

        public virtual AbilityConclusion IsEndServer()
        {
            if (m_ServerEffects.Count == 0) return AbilityConclusion.Stop;

            bool shouldStop = Config.ServerEndPolicy switch
            {
                AbilityEndPolicy.AnyEffectCompleted => m_ServerEffects.Any(e => !e.IsActive),
                AbilityEndPolicy.AllEffectCompleted => m_ServerEffects.All(e => !e.IsActive),
                _ => true
            };

            return shouldStop ? AbilityConclusion.Stop : AbilityConclusion.Continue;
        }

        /// <summary> 어빌리티가 정상 종료될 때 호출 </summary>
        public virtual void OnEndServer(ServerCharacter serverCharacter)
        {
            OnCanceledServer(serverCharacter);
            m_ServerEffects.ForEach(e => e.End(serverCharacter, this));
        }

        /// <summary> 어빌리티가 강제 취소될 때 호출 </summary>
        public virtual void OnCanceledServer(ServerCharacter serverCharacter)
        {
            m_ServerEffects.ForEach(e => e.Cancel(serverCharacter, this));
        }

        // ========================= Client =================================

        public virtual void OnStartClient(ClientCharacter clientCharacter)
        {
            m_ClientEffects.ForEach(e =>
            {
                e.IsActive = true;
                e.OnStart(clientCharacter, this);
            });
        }

        public virtual void OnUpdateClient(ClientCharacter clientCharacter)
        {
            m_ClientEffects.ForEach(e =>
            {
                if (e.IsActive) e.OnUpdate(clientCharacter, this);
            });
        }

        public virtual void OnEndClient(ClientCharacter clientCharacter)
        {
            OnCanceledClient(clientCharacter);
            m_ClientEffects.ForEach(e => e.End(clientCharacter, this));
        }

        public virtual void OnCanceledClient(ClientCharacter clientCharacter)
        {
            m_ClientEffects.ForEach(e => e.Cancel(clientCharacter, this));
        }
        public virtual AbilityConclusion IsEndClient()
        {
            if (m_ClientEffects.Count == 0) return AbilityConclusion.Stop;

            bool shouldStop = Config.ClientEndPolicy switch
            {
                AbilityEndPolicy.AnyEffectCompleted => m_ClientEffects.Any(e => !e.IsActive),
                AbilityEndPolicy.AllEffectCompleted => m_ClientEffects.All(e => !e.IsActive),
                _ => true
            };

            return shouldStop ? AbilityConclusion.Stop : AbilityConclusion.Continue;
        }

        public void OnAnimationStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_ServerEffects.ForEach(e => e.OnAnimationStateExit(animator, stateInfo, layerIndex));
        }


        public void Resolve<TData>(TData data)
        {
            m_ServerEffects.ForEach(effect =>
            {
                if(effect is IInject<TData> injectEffect)
                {
                    injectEffect.Inject(data);
                }
            });
        }

    }
}