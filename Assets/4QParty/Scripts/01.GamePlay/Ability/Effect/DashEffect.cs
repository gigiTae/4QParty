using System;
using UnityEngine;

namespace FQParty.GamePlay.Abilities.Effects
{
    public interface IDashable
    {
        void StartDash(float speed, float duration);
        void CancelDash();
        void UpdateDash();
    }


    [Serializable]
    public class DashEffect : IEffect<IDashable>
    {
        public override EffectNetType NetType => EffectNetType.OnlyOwner;
        public override bool IsExpired => m_IsExpired;

        bool m_IsExpired = false;
        public float DashSpeed = 10f;
        public float DashDuration = 0.5f;
        private float m_Timer = 0f;

        public override void Start(IDashable target)
        {
            m_IsExpired = false;
            m_Timer = 0f;
            target.StartDash(DashSpeed, DashDuration);
        }

        public override void Update(IDashable target)
        {
            m_Timer += Time.deltaTime;
            target.UpdateDash();

            if(m_Timer >= DashDuration)
            {
                m_IsExpired = true;
            }
        }

        public override void Cancel(IDashable target)
        {
            target.CancelDash();
        }
    }
}