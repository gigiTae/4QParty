using FQParty.Common.Persistance;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace FQParty.Common.DebugHelper
{
    public class DebugGizumo : PersistanceSingleton<DebugGizumo>
    {
        // 박스 정보를 담을 내부 구조체
        private struct BoxDebugData
        {
            public Vector3 Center;
            public Vector3 Size;
            public Quaternion Rotation;
            public Color Color;
            public float ExpireTime;
        }

        private List<BoxDebugData> m_Boxes = new List<BoxDebugData>();

        /// <summary>
        /// 월드 공간에 디버그 박스를 생성합니다.
        /// </summary>
        /// <param name="center">중심점</param>
        /// <param name="size">전체 크기 (HalfExtents 아님)</param>
        /// <param name="rotation">회전값</param>
        /// <param name="color">표시 색상</param>
        /// <param name="duration">유지 시간 (초)</param>
        public void AddBox(Vector3 center, Vector3 size, Quaternion rotation, Color color, float duration = 1.0f)
        {
            m_Boxes.Add(new BoxDebugData
            {
                Center = center,
                Size = size,
                Rotation = rotation,
                Color = color,
                ExpireTime = Time.time + duration
            });
        }

        private void Update()
        {
            // 시간이 다 된 데이터 제거 (역순 제거가 안전함)
            for (int i = m_Boxes.Count - 1; i >= 0; i--)
            {
                if (Time.time > m_Boxes[i].ExpireTime)
                {
                    m_Boxes.RemoveAt(i);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (m_Boxes == null || m_Boxes.Count == 0) return;

            Matrix4x4 oldMatrix = Gizmos.matrix;

            // 만료된 박스를 OnDrawGizmos에서도 체크하고 싶다면 여기서 필터링 가능
            // (Update가 작동하지 않는 에디터 모드 대비)

            for (int i = 0; i < m_Boxes.Count; i++)
            {
                var box = m_Boxes[i];

                // 위치, 회전, 스케일을 모두 포함한 행렬 생성
                // 스케일은 Vector3.one을 쓰거나 box.Size를 직접 제어할 수 있습니다.
                Gizmos.matrix = Matrix4x4.TRS(box.Center, box.Rotation, Vector3.one);

                // 선 그리기
                Gizmos.color = box.Color;
                // 행렬에 위치(Center)가 포함되었으므로 여기서는 Vector3.zero를 사용
                Gizmos.DrawWireCube(Vector3.zero, box.Size);
            }

            // 모든 작업이 끝난 후 행렬 복구
            Gizmos.matrix = oldMatrix;
        }
    }

}