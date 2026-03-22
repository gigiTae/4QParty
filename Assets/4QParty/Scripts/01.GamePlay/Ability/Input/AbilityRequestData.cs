using Codice.CM.Common.Merge;
using System;
using Unity.Netcode;
using UnityEngine;

namespace FQParty.GamePlay.Abilities
{
    public struct AbilityRequestData : INetworkSerializable
    {
        public AbilityID AbilityID;        // 게임 내 전체 액션 리스트에서의 인덱스 - 런타임에 인스턴스 참조를 복구하기 위한 용도
        public bool ShouldQueue;         // true인 경우 어빌리티를 대기열에 추가, false인 경우 현재 어빌리티를 취소하고 즉시 실행
        public bool CancelMovement;      // true인 경우 이 액션을 실행하기 전에 현재 이동 상태를 취소함

        [Flags]
        private enum PackFlags
        {
            None = 0,
        }

        public static AbilityRequestData Create(Ability ability) =>
            new()
            {
                AbilityID = ability.AbilityID
            };

        private PackFlags GetPackFlags()
        {
            PackFlags flags = PackFlags.None;


            return flags;
        }


        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            PackFlags flags = PackFlags.None;
            if(!serializer.IsReader)
            {
                flags = GetPackFlags(); 
            }

            serializer.SerializeValue(ref AbilityID);
        }
    }

}