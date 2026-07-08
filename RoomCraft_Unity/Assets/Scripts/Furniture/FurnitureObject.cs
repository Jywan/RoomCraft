using System;
using System.Collections.Generic;
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
        // 추가 수정(최적화): 씬에 활성화되어 있는 모든 FurnitureObject를 자동으로 추적하는 정적 리스트
        // FurnitureBounds.IsOverlapping()이 매 프레임 FindObjectsByType로 씬을 스캔하던 걸
        // 여기서 미리 관리햐둔 리스트를 읽는것으로 대체하기 위함
        public static readonly List<FurnitureObject> All = new List<FurnitureObject>();
        
        private FurnitureData data;
        private Renderer furnitureRenderer;
        private Collider furnitureCollider;         // Collider 캐싱용 필드 추가
        private Renderer[] allRenderers;            // 전체 자식 Renderer 캐싱용
        private Color originalColor;
        private bool isSelected = false;
        
        /// <summary>
        /// 이 컴포넌트가 활성화 될 때 (예: 가구 생성시) 리스트에 자기 자신을 등록
        /// </summary>
        private void OnEnable()
        {
            All.Add(this);
        }
        
        /// <summary>
        /// 이 컴포넌트가 비활성화/파괴될 때(가구 삭제 시) 리스트에서 제거
        /// </summary>
        private void OnDisable()
        {
            All.Remove(this);
        }
        
        /// <summary>
        /// 가구 생성 시 호출, 데이터를 받아서 오브젝트에 적용한다.
        /// scale를 치수에 맞게 설정하고, 색상을 입힌다.
        /// </summary>
        public void Initialize(FurnitureData furnitureData)
        {
            data = furnitureData;
            furnitureRenderer = GetComponent<Renderer>();
            furnitureCollider = GetComponent<Collider>();           // 여기서 한번만 GetComponent 호출
            allRenderers = GetComponentsInChildren<Renderer>();
            
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
            // Renderer[] renderers = GetComponentsInChildren<Renderer>(); 캐싱 최적화로 인해 주석 처리
            foreach (Renderer r in allRenderers)
                r.material.color = originalColor * 1.3f;
        }

        /// <summary>
        /// 선택 해제. 원래 색상으로 되돌린다.
        /// </summary>
        public void Deselect()
        {
            isSelected = false;
            // Renderer[] renderers = GetComponentsInChildren<Renderer>(); 캐싱 최적화로 인해 주석 처리
            foreach (Renderer r in allRenderers)
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
        
        
        /// <summary>
        ///  Y축 회전을 절대각으로 지정 (0 ~ 360)
        /// </summary>
        public void SetRotationY(float angleY)
        {
            transform.rotation = Quaternion.Euler(0f, angleY, 0f);
        }
        
        
        /// <summary>
        /// 현재 Y축 회전각 반환 (0 ~ 360)
        /// </summary>
        public float GetRotationY()
        {
            return transform.eulerAngles.y;
        }

        
        /// <summary>
        /// FurnitureBounds 등 외부에서 매번 GetComponent<Collider>() 호출하는 대신 이걸 쓰도록 전환
        /// </summary>
        public Bounds GetBounds()
        {
            return furnitureCollider.bounds;
        }

        /// <summary>
        /// FurnitureBounds.ValidatePosition()에서 쓰도록 노출
        /// </summary>
        public Renderer[] GetRenderers()
        {
            return allRenderers;
        }
    }
}