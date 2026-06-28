using UnityEngine;

namespace RoomCraft.Furniture
{
    /// <summary>
    /// 가구 배치 시 병계검사를 담당하는 유틸리티 클래스.
    /// 방 범위를 벗어났는지, 다른 가구와 겹치는지 체크한다.
    /// FurnitureInteraction에서 드래그 중에 호출한다.
    /// </summary>
    public class FurnitureBounds : MonoBehaviour
    {
        [Header("Room Bounds")] 
        [SerializeField] private float roomWidth = 4f;  // 방 가로 (m)
        [SerializeField] private float roomDepth = 3f;  // 방 세로 (m)\

        [Header("Visual Feedback")] [SerializeField]
        private Color warningColor = new Color(1f, 0.3f, 0.3f, 0.8f);
        
        
        /// <summary>
        /// 방 크기를 설정한다. RoomBuilder에서 방 생성 후 호출
        /// </summary>
        public void SetRoomSize(float width, float depth)
        { 
            roomWidth = width;
            roomDepth = depth;
        }

        
        /// <summary>
        /// 가구가 방 경계밖에 있는지 검사
        /// 가구의 Collider bounds를 기준으로 방의 가로/세로 범위를 벗아나면 false. 
        /// </summary>
        public bool IsInsideRoom(FurnitureObject furniture)
        {
            Bounds bounds = furniture.GetComponent<Collider>().bounds;

            float wallThickness = 0.01f;
            float halfWidth = roomWidth / 2f - wallThickness;
            float halfDepth = roomDepth / 2f - wallThickness;
            
            // 가구의 경계가 방 범를 벗어나는지 체크
            if (bounds.min.x < -halfWidth || bounds.max.x > halfWidth)
                return false;
            if (bounds.min.z < -halfDepth || bounds.max.z > halfDepth)
                return false;
            
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
    }
}