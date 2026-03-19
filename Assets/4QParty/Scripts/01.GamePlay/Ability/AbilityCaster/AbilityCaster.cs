using FQParty.GamePlay.Abilities;
using FQParty.GamePlay.Abilities.Effects;
using Unity.Netcode;
using UnityEngine;

public class AbilityCaster : NetworkBehaviour
{
    [SerializeField] AbilityDatabase m_Database;
    [SerializeField] AbilityEffectHandler m_Handler;

    // 어빌리티 실행
    public void CastAbility(AbilityPacket packet)
    {
        AbilityData data = m_Database.Find(packet.AbilityID);

        foreach(IEffect effect in data.EffectList)
        {
            if(effect is IEffect<IDashable> dashEffect)
            {
                m_Handler.ApplyEffect(dashEffect);
            }
        }

    }
}
