using FQParty.GamePlay.Abilities;
using UnityEngine;

public class Notify : StateMachineBehaviour
{
    int m_CallCount = 0; 

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_CallCount++;


        Debug.Log($"Enter {m_CallCount}");
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_CallCount--;

        Debug.Log($"Exit {m_CallCount}");

        // A->B 트랜지션 중에 다시 A로 가는 경우 예외처리를 하기 위해서
        if (m_CallCount != 0) return;
        
        var serverAbilityPlayr = animator.GetComponentInParent<ServerAbilityPlayer>();
        if (serverAbilityPlayr != null)
        {
            serverAbilityPlayr.OnAnimationStateExit(animator, stateInfo, layerIndex);
        }
    }

}
