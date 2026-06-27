using RoomCraft.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoomCraft.Furniture
{
    /// <summary>
    /// 가구의 인터렉션을 총괄하는 매니저
    /// 가구 생성, 선택, 드래그 이동, 회전, 삭제를 처리한다.
    /// 씬에 하나만 존재하며 빈 GameObject에 붙여서 사용.
    /// </summary>
    public class FurnitureInteraction : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayerMask floorLayer;          // 바닥 레이어 (드래그 시 Raycast 대상)
        [SerializeField] private LayerMask furnitureLayer;      // 가구 레이어 (클릭 감지용)
        
        private FurnitureObject selectedFurniture = null;
        private bool isDragging = false;
        private Camera mainCamera;
        private FurnitureBounds bounds;
        private Vector3 lastValidPosition;                      // 마지막 유효 위치 (무효 시 되돌리기 용도!)

        private void Start()
        {
            mainCamera = Camera.main;
            bounds = FindAnyObjectByType<FurnitureBounds>();
        }
        
        private void Update()
        {
            HandleSelection();
            HandleDrag();
            HandleRotation();
            HandleDelete();
        }
        
        // ===== 가구 생성 =====
        
        /// <summary>
        /// 새 가구를 생성해서 씬에 배치
        /// 기본 Cube로 만들고, FurnitureObject 컴포넌트를 붙여서 초기화
        /// </summary>
        public FurnitureObject CreateFurniture(FurnitureData data)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.layer = LayerMask.NameToLayer("Furniture");
            
            FurnitureObject furniture = obj.AddComponent<FurnitureObject>();
            furniture.Initialize(data);
            
            // 생성 시 랜덤한 빈곳으로 이동
            furniture.Initialize(data);
            furniture.MoveTo(new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-0.5f, 0.5f)));
            
            return furniture;
        }
        
        
        // ===== 선택 처리 =====
        
        /// <summary>
        /// 좌클릭 시 Ray를 쏴서 가구를 선택하거나 선택 해제한다.
        /// 가구를 클릭하면 선택, 빈 곳을 클릭하면 선택 해제
        /// </summary>
        private void HandleSelection()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            
            // UI 위에서 클릭했으면 무시
            if (EventSystem.current.IsPointerOverGameObject()) return;
            
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                FurnitureObject clicked = hit.collider.GetComponent<FurnitureObject>();

                if (clicked != null)
                {
                    // 기존 선택 해제
                    if (selectedFurniture != null && selectedFurniture != clicked)
                        selectedFurniture.Deselect();
                    
                    selectedFurniture = clicked;
                    selectedFurniture.Select();
                    isDragging = true;
                    lastValidPosition = selectedFurniture.transform.position;
                }
                else
                {
                    // 가구가 아닌곳 클릭 -> 선택 해제
                    DeselectCurrent();
                }
            }
            else
            {
                DeselectCurrent();
            }
        }
        
        
        // ===== 드래그 이동 =====
        
        /// <summary>
        /// 선택된 가구가 있고 드래그 중일 때, 마우스 위치를 바닥에 Ray로 쏴서
        /// 가구를 해당 위치로 이동시킨다.
        /// (추가) 이동 후 경계/겹침 검사를 수행하고, 유효하지 않은 위치면 경고 표시
        /// (추가) 마우스를 뗐을 때 유요하지 않은 위치면 마지막 유효 위치로 되돌린다.
        /// </summary>
        private void HandleDrag()
        {
            if (selectedFurniture == null || !isDragging) return;
            
            // 마우스 뗐으면 드래그 종료
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                
                // 최종 위치가 유효하지 않으면 마지막 유효 위치로 이동
                if (bounds != null && !bounds.ValidatePosition(selectedFurniture))
                {
                    selectedFurniture.MoveTo(lastValidPosition);
                    selectedFurniture.Select();         // 색상 복귀
                }
                return;
            }
            
            // 바닥에 Raycast해서 위치 가져오기
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, floorLayer))
            {
                selectedFurniture.MoveTo(hit.point);
                
                // 드래그 실시간 검증
                if (bounds != null)
                {
                    bool valid = bounds.ValidatePosition(selectedFurniture);
                    if (valid)
                    {
                        lastValidPosition = hit.point;
                    }
                }
            }
        }
        
        
        // ===== 회전 =====

        /// <summary>
        /// R키를 누르면 선택된 가구를 90도 회전시킨다
        /// </summary>
        private void HandleRotation()
        {
            if (selectedFurniture == null) return;

            if (Input.GetKeyDown(KeyCode.R))
            {
                selectedFurniture.Rotate90();
            }
        }
        
        
        // ===== 삭제 =====

        /// <summary>
        /// Delete 또는 Backspace 키를 누르면 선택된 가구를 삭제한다.
        /// </summary>
        private void HandleDelete()
        {
            if (selectedFurniture == null) return;

            if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
            {
                Destroy(selectedFurniture.gameObject);
                selectedFurniture = null;
            }
        }
        
        
        // ===== 유틸 =====
        
        /// <summary>
        /// 현재 선택 해제.
        /// </summary>
        private void DeselectCurrent()
        {
            if (selectedFurniture != null)
            {
                selectedFurniture.Deselect();
                selectedFurniture = null;
            }
            isDragging = false;
        }
        
        
        /// <summary>
        /// 외부에서 현재 선택된 가구 정보 가져오기. UI 표시용.
        /// </summary>
        public FurnitureObject GetSelectedFurniture()
        {
            return selectedFurniture;
        }
    }
}