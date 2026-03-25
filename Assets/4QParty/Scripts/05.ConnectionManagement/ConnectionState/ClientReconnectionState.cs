using UnityEngine;



namespace FQParty.ConnectionManagement
{
    /// <summary>
    /// TODO: 재접속 기능을 넣어야할까? 이건 상의가 필요해보인단
    /// </summary>
    class ClientReconnectingState : ClientConnectedState
    {
        public override void Enter()
        {
            //    StartHost();
        }

        public override void Exit() { }
    }

}