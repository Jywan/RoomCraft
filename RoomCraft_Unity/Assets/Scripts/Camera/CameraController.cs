using RoomCraft.Furniture;
using RoomCraft.Room;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoomCraft.CameraSystem
{
    public enum CameraMode
    {
        TopView,
        FreeView,
        FirstPerson
    }
    
    /// <summary>
    /// 카메라 시점을 제어하는 컨트롤러.
    /// 탑뷰(위에서 내려다보기)와 FreeView(3D 자유 회전) 두 가지 모드를 지원하며,
    /// 모드 간 부드러운 전환 애니메이션을 제공한다.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Vector3 targetPoint = Vector3.zero;
        
        [Header("Top View Settings")]
        [SerializeField] private float topViewHeight = 10f;

        [Header("Free View Settings")] 
        [SerializeField] private float distance = 8f;
        [SerializeField] private float minDistance = 3;
        [SerializeField] private float maxDistance = 20f;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float zoomSpeed = 5f;
        
        [Header("Angle Limits")]
        [SerializeField] private float minVerticalAngle = -10f;
        [SerializeField] private float maxVerticalAngle = 80f;
        
        [Header("Furniture Interaction")]
        [SerializeField] private LayerMask furnitureLayer;
        
        [Header("First Person Settings")]
        [SerializeField] private RoomBuilder roomBuilder;
        [SerializeField] private FurnitureInteraction furnitureInteraction;
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float lookSpeed = 2f;
        [SerializeField] private CharacterController characterController;
        
        private CameraMode currentMode = CameraMode.TopView;
        private float horizontalAngle = 0f;
        private float verticalAngle = 45f;

        private bool isDragging = false;
        private Vector3 lastMousePosition;
        
        // 전환 애니메이션
        private bool isTransitioning = false;
        private Vector3 transitionStartPos;
        private Quaternion transitionStartRot;
        private Vector3 transitionEndPos;
        private Quaternion transitionEndRot;
        private float transitionProgress = 0f;
        private float transitionDuration = 0.5f;
        
        // 1인칭 사용
        private float fpYaw = 0f;
        private float fpPitch = 0f;
        private bool fpDragging = false;
        private Vector3 fpLastMousePosition;
        
        // Camera.main 캐싱
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
            ApplyTopView();
        }

        private void Update()
        {
            if (isTransitioning)
            {
                UpdateTransition();
                return;
            }

            if (currentMode == CameraMode.FreeView)
                HandleFreeViewInput();
            else if (currentMode == CameraMode.FirstPerson)
                HandleFirstPersonInput();
            
            HandleZoom();
        }

        /// <summary>
        /// 외부에서 호출: 시점 변환.
        /// 현재 모드와 같으면 무시, 다르면 전환 애니메이션 시작.
        /// </summary>
        public void SwitchMode(CameraMode mode)
        {
            if (currentMode == mode) return;
            
            currentMode = mode;
            StartTransition();
        }
        
        /// <summary>
        /// 탑뷰 ↔ 3D 뷰 토글.
        /// 현재 탑뷰면 FreeView로, FreeView면 탑뷰로 전환한다.
        /// </summary>
        public void ToggleView()
        {
            if (currentMode == CameraMode.TopView)
                SwitchMode(CameraMode.FreeView);
            else if (currentMode == CameraMode.FreeView)
                SwitchMode(CameraMode.FirstPerson);
            else
                SwitchMode(CameraMode.TopView);
        }

        /// <summary>
        /// 현재 카메라 모드 반환.
        /// </summary>
        public CameraMode GetCurrentMode()
        {
            return currentMode;
        }
        
        /// <summary>
        /// FreeView 상태에서 좌클릭 드래그 입력을 처리.
        /// 드래그 방향에 따라 수평/수직 각도를 변경하여 카메라를 targetPoint 기준으로 공전시킨다.
        /// </summary>
        private void HandleFreeViewInput()
        {
            // 맥북 터치패드로 하기 힘들어서 마우스 우클릭 -> 좌클릭으로 변경
            if (Input.GetMouseButtonDown(0))
            {
                // UI 위에서 클릭했으면 카메라 드래그 무시
                if (EventSystem.current.IsPointerOverGameObject())
                    return;
                
                // 가구 윈에서 클릭했으면 카메라 드래그 무시 (가구이동과 겹치지 않게)
                if (IsPointerOverFurniture())
                    return;
                
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            if (isDragging)
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                horizontalAngle += delta.x * rotationSpeed * 0.1f;
                verticalAngle -= delta.y * rotationSpeed * 0.1f;
                verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
                lastMousePosition = Input.mousePosition;

                ApplyFreeView();
            }
            
        }

        /// <summary>
        /// 마우스 스크롤(트랙패드 두 손가락 스크롤)로 줌 인/아웃 처리.
        /// 탑뷰에서는 카메라 높이를, FreeView에서는 타겟까지의 거리를 조절한다.
        /// </summary>
        private void HandleZoom()
        {
            if (currentMode == CameraMode.FirstPerson) return;
            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) > 0.01f)
            {
                if (currentMode == CameraMode.TopView)
                {
                    topViewHeight -= scroll * zoomSpeed;
                    topViewHeight = Mathf.Clamp(topViewHeight, minDistance, maxDistance);
                    ApplyTopView();
                }
                else
                {
                    distance -= scroll * zoomSpeed;
                    distance = Mathf.Clamp(distance, minDistance, maxDistance);
                    ApplyFreeView();
                }
            }
        }

        /// <summary>
        /// 탑뷰 적용: 카메라를 targetPoint 바로 위에 배치하고 아래를 내려다보도록 회전.
        /// </summary>
        private void ApplyTopView()
        {
            transform.position = targetPoint + Vector3.up * topViewHeight;
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        /// <summary>
        /// FreeView 적용: 구면 좌표계(수평각, 수직각, 거리)를 이용해
        /// targetPoint를 중심으로 공전하는 위치에 카메라를 배치하고, targetPoint를 바라본다.
        /// </summary>
        private void ApplyFreeView()
        {
            float rad_h = horizontalAngle * Mathf.Deg2Rad;
            float rad_v = verticalAngle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                distance * Mathf.Cos(rad_v) * Mathf.Sin(rad_h),
                distance * Mathf.Sin(rad_v),
                distance * Mathf.Cos(rad_v) * Mathf.Cos(rad_h)
            );

            transform.position = targetPoint + offset;
            transform.LookAt(targetPoint);
        }

        /// <summary>
        /// 모드 전환 시 현재 위치/회전에서 목표 위치/회전까지의 보간 애니메이션을 시작한다.
        /// 목표값은 전환될 모드(탑뷰 또는 FreeView)에 맞게 계산된다.
        /// </summary>
        private void StartTransition()
        {
            transitionStartPos = transform.position;
            transitionStartRot = transform.rotation;
            
            // 목표 위치 계산
            if (currentMode == CameraMode.TopView)
            {
                transitionEndPos = targetPoint + Vector3.up * topViewHeight;
                transitionEndRot = Quaternion.Euler(90f, 0f, 0f);
            }
            else if (currentMode == CameraMode.FirstPerson)
            {
                // 자주 호출되지 않기에 null 체크는 남기는 방향으로 유지함.
                if (furnitureInteraction != null)
                    furnitureInteraction.DeselectAll();

                Vector3 spawn = roomBuilder != null
                    ? roomBuilder.GetFirstPersonSpawnPoint()
                    : targetPoint + Vector3.up * 1.6f;

                transitionEndPos = spawn;
                fpYaw = transform.eulerAngles.y;
                fpPitch = 0f;
                transitionEndRot = Quaternion.Euler(0f, fpYaw, 0f);
            }
            else
            {
                float rad_h = horizontalAngle * Mathf.Deg2Rad;
                float rad_v = verticalAngle * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(
                    distance * Mathf.Cos(rad_v) * Mathf.Sin(rad_h),
                    distance * Mathf.Sin(rad_v),
                    distance * Mathf.Cos(rad_v) * Mathf.Cos(rad_h)
                );
                transitionEndPos = targetPoint + offset;
                transitionEndRot = Quaternion.LookRotation(targetPoint - transitionEndPos);
            }

            transitionProgress = 0f;
            isTransitioning = true;
        }

        /// <summary>
        /// 매 프레임 전환 애니메이션 진행.
        /// SmoothStep 보간으로 시작/끝 지점 사이를 부드럽게 이동한다.
        /// </summary>
        private void UpdateTransition()
        {
            transitionProgress += Time.deltaTime / transitionDuration;

            if (transitionProgress >= 1f)
            {
                transitionProgress = 1f;
                isTransitioning = false;
            }
            
            // SmoothStep으로 부드러운 전환
            float t = Mathf.SmoothStep(0f, 1f, transitionProgress);
            transform.position = Vector3.Lerp(transitionStartPos, transitionEndPos, t);
            transform.rotation = Quaternion.Slerp(transitionStartRot, transitionEndRot, t);
        }

        
        /// <summary>
        /// 방 중심점 설정 (방 생성 후 호출).
        /// 카메라가 바라보는 기준점을 변경하고, 현재 모드에 맞게 즉시 위치를 갱신한다.
        /// </summary>
        public void SetTargetPoint(Vector3 point)
        {
            targetPoint = point;
            
            if (currentMode == CameraMode.TopView)
                ApplyTopView();
            else 
                ApplyFreeView();
        }

        
        /// <summary>
        /// 외부에서 호출: 줌 인/아웃
        /// amount 양수면 줌 인, 음수면 줌 아웃
        /// </summary>
        public void Zoom(float amount)
        {

            if (currentMode == CameraMode.FirstPerson) return;
            if (currentMode == CameraMode.TopView)
            {
                topViewHeight -= amount * zoomSpeed;
                topViewHeight = Mathf.Clamp(topViewHeight, minDistance, maxDistance);
                ApplyTopView();
            }
            else
            {
                distance -= amount * zoomSpeed;
                distance = Mathf.Clamp(distance, minDistance, maxDistance);
                ApplyFreeView();
            }
        }
        
        
        /// <summary>
        /// 마우스 아래에 가구가 있는지 확인한다.
        /// </summary>
        private bool IsPointerOverFurniture()
        {
            // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 캐싱으로 주석
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, 100f, furnitureLayer);
        }
        
        private void HandleFirstPersonInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                fpDragging = true;
                fpLastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                fpDragging = false;
            }

            if (fpDragging)
            {
                Vector3 delta = Input.mousePosition - fpLastMousePosition;
                fpYaw += delta.x * lookSpeed * 0.1f;
                fpPitch -= delta.y * lookSpeed * 0.1f;
                fpPitch = Mathf.Clamp(fpPitch, -80f, 80f);
                fpLastMousePosition = Input.mousePosition;
                transform.rotation = Quaternion.Euler(fpPitch, fpYaw, 0f);
            }
            
            Vector3 move = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) move += transform.forward;
            if (Input.GetKey(KeyCode.S)) move -= transform.forward;
            if (Input.GetKey(KeyCode.A)) move -= transform.right;
            if (Input.GetKey(KeyCode.D)) move += transform.right;

            move.y = 0f;
            if (move.sqrMagnitude > 0.0001f)
            {
                Vector3 delta2 = move.normalized * (moveSpeed * Time.deltaTime);
                
                // CharacterController가 연결되어 있다면 충돌 처리된 이동, 없다면 기존 방식으로 적용
                if (characterController != null)
                    characterController.Move(delta2);
                else 
                    transform.position += delta2;
            }

        }
    }
}