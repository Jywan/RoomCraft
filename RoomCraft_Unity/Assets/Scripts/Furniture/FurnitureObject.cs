using RoomCraft.Data;
using UnityEngine;

namespace RoomCraft.Furniture
{
    /// <summary>
    /// 씬에 배치된 개별 가구 오브젝트에 붙는 컴포넌트
    /// 가구의 데이터(치수, 이름 등)를 들고 있고,
    /// 선택/드래그/회전 시 이 컴포넌트를 통해 접근한다.
    /// </summary>
    public class FurnitureObject : MonoBehaviour
    {
        private FurnitureData data;
        private Renderer furnitureRenderer;
        private Color originalColor;
        private bool isSelected = false;

        /// <summary>
        /// 가구 생성 시 호출, 데이터를 받아서 오브젝트에 적용한다.
        /// scale를 치수에 맞게 설정하고, 색상을 입힌다.
        /// </summary>
        public void Initialize(FurnitureData furnitureData)
        {
            data = furnitureData;
            furnitureRenderer = GetComponent<Renderer>();
            
            // 팩토리가 이미 크기/색상을 설정했으므로 여기는 이름/태그만
            // Renderer가 없을 수 있음 - root에는 Renderer가 없고 자식에만 있음
            if (furnitureRenderer == null)
                furnitureRenderer = GetComponentInChildren<Renderer>();
            if (furnitureRenderer != null)
                originalColor = furnitureRenderer.material.color;

            gameObject.name = $"Furniture_{data.furnitureName}";
            gameObject.tag = "Furniture";
        }

        /// <summary>
        /// 가구 데이터 변환. UI에서 치수/이름 표시할 때 사용
        /// </summary>
        public FurnitureData GetData()
        {
            return data;
        }


        /// <summary>
        /// 선택 상태 표시. 색상을 밝게 해서 선택됐음을 시각적으로 알린다.
        /// </summary>
        public void Select()
        {
            isSelected = true;
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
                r.material.color = originalColor * 1.3f;
        }

        /// <summary>
        /// 선택 해제. 원래 색상으로 되돌린다.
        /// </summary>
        public void Deselect()
        {
            isSelected = false;
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
                r.material.color = originalColor;
        }

        public bool IsSelected()
        {
            return isSelected;
        }

        /// <summary>
        /// 가구 위치를 지정한 좌표로 이동.
        /// </summary>
        public void MoveTo(Vector3 position)
        {
            transform.position = new Vector3(position.x, 0f, position.z);
        }


        /// <summary>
        /// 지정한 각도만큼 Y축 회전
        /// </summary>
        public void RotateBy(float angle)
        {
            transform.Rotate(Vector3.up, angle);
        }
    }
}