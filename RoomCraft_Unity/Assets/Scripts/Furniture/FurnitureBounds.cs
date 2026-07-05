using System.Collections.Generic;
using RoomCraft.Room;
using UnityEngine;

namespace RoomCraft.Furniture
{
    /// <summary>
    /// 가구 배치 시 경계검사를 담당하는 유틸리티 클래스.
    /// 방 범위를 벗어났는지, 다른 가구와 겹치는지 체크한다.
    /// 다각형 추가 로직 추가로 내부 로직 수정 진행
    /// FurnitureInteraction에서 드래그 중에 호출한다.
    /// </summary>
    public class FurnitureBounds : MonoBehaviour
    {
        [Header("Room Bounds")] 
        [SerializeField] private float wallTickness = 0.01f;

        [Header("Visual Feedback")]
        [SerializeField] private Color warningColor = new Color(1f, 0.3f, 0.3f, 0.8f);

        private List<Vector2> roomOutLine;
        
        
        /// <summary>
        /// 방의 실제 꼭짓점(다각형)을 설정, RoomBuilder에서 방 생성 후 호출
        /// </summary>
        public void SetRoomShape(List<Vector2> vertices)
        {
            roomOutLine = GetInsetPolygon(vertices, wallTickness);
        }

        
        /// <summary>
        /// 가구가 방 경계(다각형) 밖에 있는지 검사
        /// 가구 바운딩 박스의 네 모서리가 전부 다각형 안에 있어야 유효 
        /// </summary>
        public bool IsInsideRoom(FurnitureObject furniture)
        {
            if (roomOutLine == null) return true;
            
            Bounds bounds = furniture.GetComponent<Collider>().bounds;

            Vector2[] corners =
            {
                new Vector2(bounds.min.x, bounds.min.z),
                new Vector2(bounds.max.x, bounds.min.z),
                new Vector2(bounds.max.x, bounds.max.z),
                new Vector2(bounds.min.x, bounds.max.z),
            };

            foreach (Vector2 corner in corners)
            {
                if (!PolygonUtils.PointInPolygon(corner, roomOutLine))
                    return false;
            }
            
            return true;
        }
        
        
        /// <summary>
        /// 다른 가구와 겹치는지 검사.
        /// 현재 가구의 Collider bounds가 씬에 있는 다른 가구의 bounds와 교하면 true
        /// </summary>
        public bool IsOverlapping(FurnitureObject furniture)
        {
            Bounds mybounds = furniture.GetComponent<Collider>().bounds;
            
            // 씬 안에 있는 모든 가구들을 찾아서 비교 
            FurnitureObject[] allFurniture = FindObjectsByType<FurnitureObject>(FindObjectsSortMode.None);

            foreach (FurnitureObject other in allFurniture)
            {
                if (other ==  furniture) continue;  // 자기 자신은 스킵
                
                Bounds otherBounds = other.GetComponent<Collider>().bounds;
                
                if (mybounds.Intersects(otherBounds))
                    return true;
            }
            
            return false;
        }

        
        /// <summary>
        /// 종합 검사: 방 밖이거나 겹치면 경고 색상 적용, 정상이면 원래 색으로 복귀
        /// 드래그 중 매 프레임 호출
        /// 반환값: true면 유효한 위치, false면 유효하지 않은 위치
        /// </summary>
        public bool ValidatePosition(FurnitureObject furniture)
        {
            bool insideRoom = IsInsideRoom(furniture);
            bool overlapping = IsOverlapping(furniture);
            
            Renderer renderer = furniture.GetComponentInChildren<Renderer>();
            if (renderer == null) return true;

            if (!insideRoom || overlapping)
            {
                // 경고 표시: 빨간색 (자식 전체에 적용)
                Renderer[] renderers = furniture.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                    r.material.color = warningColor;
                return false;
            }
            else
            {
                // 정상: 선택 상태 색상으로 복귀
                furniture.Select();         // Select()가 밝은 색으로 만들어줌
                return true;
            }
        }
        
        
        /// <summary>
        /// 다각형을 중심점 기준으로 살짝 안쪽으로 축소 (벽 두께 보정용)
        /// </summary>
        private List<Vector2> GetInsetPolygon(List<Vector2> polygon, float inset)
        {
            Vector2 centroid = Vector2.zero;
            foreach (Vector2 v in polygon)
                centroid += v;
            centroid /= polygon.Count;

            List<Vector2> result = new List<Vector2>();
            foreach (Vector2 v in polygon)
            {
                Vector2 dir = (v - centroid).normalized;
                result.Add(v - dir * inset);
            }
            
            return result;
        }
    }
}