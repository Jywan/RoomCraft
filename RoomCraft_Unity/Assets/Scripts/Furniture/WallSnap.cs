using System;
using System.Collections.Generic;
using UnityEngine;
using RoomCraft.Room;

namespace RoomCraft.Furniture
{
    /// <summary>
    /// 가구를 방 외곽선(벽)에 붙여서 정렬해주는 유틸리티
    /// 방 외곽선의 각 변과 가구 위치 사이 거리를 검사해서 일정거리 이내면 그 벽에 딷붙는 위치/각도로 스냅
    /// FurnitureInteraction.HandleDrag()에서 그리드 스냅 이후에 호출한다.
    /// </summary>
    public class WallSnap : MonoBehaviour
    {
        [Header("Snap Settings")]
        [SerializeField] private float snapDistance = 0.3f;     // 벽에서 이 거리(m) 이내면 스냅
        [SerializeField] private bool snapEnabled = true;

        private List<Vector2> roomOutline;

        
        /// <summary>
        /// 방의 꼭짓점을 설정, RoomBuilder에서 방 생성후에 호출
        /// </summary>
        public void SetRoomShape(List<Vector2> vertices)
        {
            roomOutline = vertices;
        }

        public void ToggleSnap()
        {
            snapEnabled = !snapEnabled;
            Debug.Log($"벽 스냅: {(snapEnabled ? "ON" : "OFF")}");
        }
        
        public bool IsEnabled() => snapEnabled;
        
        
        /// <summary>
        /// 가구를 가장 가까운 뱍에 스냅 시도.
        /// 스냅 조건(거리 이내)을 만족하면 위치/회전을 계산해서 동려주고 true 반환
        /// 아니면 false 반환 (호출부에서 원래 위치를 그대로 쓰면 됨)
        /// </summary>
        public bool TrySnap(FurnitureObject furniture, Vector3 currentPos, out Vector3 snappedPos,
            out float snappedRotY)
        {
            snappedPos = currentPos;
            snappedRotY = furniture.GetRotationY();

            if (!snapEnabled || roomOutline == null || roomOutline.Count < 3)
                return false;

            Vector3 size = furniture.GetData().GetSizeInMeters();
            float halfDepth = size.z / 2f;
            float halfWidth = size.x / 2f;

            Vector2 point = new Vector2(currentPos.x, currentPos.z);
            Vector2 centroid = GetCentroid();
            
            float bestDist = float.MaxValue;
            Vector2 bestA = Vector2.zero, bestB = Vector2.zero;
            Vector2 bestNormal = Vector2.zero;
            bool found = false;
            
            int n = roomOutline.Count;
            for (int i = 0; i < n; i++)
            {
                Vector2 a = roomOutline[i];
                Vector2 b = roomOutline[(i + 1) % n];
                
                Vector2 closest = ClosestPointOnSegment(point, a, b);
                float dist = Vector2.Distance(point, closest);

                if (dist < snapDistance && dist < bestDist)
                {
                    Vector2 edge = (b - a).normalized;
                    Vector2 normal = new Vector2(-edge.y, edge.x);
                    if (Vector2.Dot(normal, centroid - a) < 0f)
                        normal = -normal;
                    
                    bestDist = dist;
                    bestA = a;
                    bestB = b;
                    bestNormal = normal;
                    found = true;
                }
            }

            if (!found) return false;
            
            // 벽을 따라(가구 폭 방향) 코너를 넘어가지 않도록 위치를 세그먼트 안쪽으로 고정
            Vector2 edgeDir = (bestB - bestA).normalized;
            float segmentLength = Vector2.Distance(bestA, bestB);
            float t = Vector2.Dot(point - bestA, edgeDir);
            float margin = Mathf.Min(halfWidth, segmentLength / 2f);        // 벽이 가구보다 짧으면 가운데로
            t = Mathf.Clamp(t, margin, segmentLength - margin);
            Vector2 alongWall = bestA + edgeDir * t;
            
            // 벽에서 가구 반 깊이 + 벽 두께 절반만큼 안쪽으로 띄운 위치
            Vector2 snapped2D = alongWall + bestNormal * (halfDepth + RoomBuilder.WallThickness / 2f);
            snappedPos = new Vector3(snapped2D.x, currentPos.y, snapped2D.y);
            
            // 가구 정면(로컬 +Z)이 방 안쪽을 향하도록 회전
            snappedRotY = Mathf.Atan2(bestNormal.x, bestNormal.y) * Mathf.Rad2Deg;
            return true;
        }

        private Vector2 GetCentroid()
        {
            Vector2 c = Vector2.zero;
            foreach (Vector2 v in roomOutline)
                c += v;
            return c / roomOutline.Count;
        }

        private Vector2 ClosestPointOnSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            Vector2 ab = b - a;
            float t = Vector2.Dot(p - a, ab) /  ab.sqrMagnitude;
            t = Mathf.Clamp01(t);
            return a + ab * t;
        }
    }
}