using FQParty.GamePlay.Character;
using System;
using UnityEngine;


namespace FQParty.GamePlay.Abilities
{
    public abstract class Ability : ScriptableObject
    {
        [NonSerialized]
        public AbilityID AbilityID;

        protected AbilityRequestData m_Data;

        [SerializeField] public AbilityConfig Config;

        /// <summary>
        /// 어빌리티 실행 시작 시간 
        /// </summary>
        public float TimeStarted { get; set; }

        /// <summary>
        /// 
        /// </summary>
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

        public abstract bool OnStart(ServerCharacter serverCharacter);
        public abstract bool OnUpdate(ServerCharacter serverCharacter);

        /// <summary>
        /// 어빌리티가 조건에 의해서 정상적으로 종료된 경우 호출합니다
        /// </summary>
        public virtual void End(ServerCharacter serverCharacter)
        {
            Cancel(serverCharacter);
        }

        /// <summary>
        /// 어빌리티가 취소되면 호출합니다
        /// </summary>
        public virtual void Cancel(ServerCharacter serverCharacter)
        {}

        public virtual bool ChainIntoNewAction(ref AbilityRequestData newAbility) { return false; }

        public bool AnticipatedClient { get; protected set; }

        public virtual bool OnStartClient(ClientCharacter clientCharacter)
        {
            TimeStarted = Time.time;
            return true;
        }

        public virtual bool OnUpdateClient(ClientCharacter clientCharacter)
        {
            return AbilityConclusion.Continue;
        }

        public virtual void EndClient(ClientCharacter clientCharacter)
        {
            CancelClient(clientCharacter);
        }

        public virtual void CancelClient(ClientCharacter clientCharacter) { }

        public virtual bool ShouldBecomeNonBlocking()
        {
            return Config.BlockingMode == BlockingModeType.OnlyDuringExecTime ? TimeRunning >= Config.ExecTimeSeconds : false;
        }
    }

}