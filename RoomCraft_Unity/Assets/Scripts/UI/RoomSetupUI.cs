using RoomCraft.CameraSystem;
using RoomCraft.Data;
using RoomCraft.Furniture;
using RoomCraft.Room;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoomCraft.UI
{
    
    /// <summary>
    /// 방 설정 UI 컨트롤러
    /// 사용자가 방 크기 (m)를 입력하고 "방만들기"를 누르면
    /// RoomBuilder로 방을 생성하고, 관련 시스템(카메라, 충돌 감지)에 크기를 전달한다.
    /// 앱 시작 시 표시되게하고, 방 생성 후 숨기는 방향으로 진행 
    /// </summary>
    public class RoomSetupUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject panel;              // 방 설정 패널
        [SerializeField] private TMP_InputField widthInput;     // 가로 (m)
        [SerializeField] private TMP_InputField depthInput;     // 세로 (m)
        [SerializeField] private TMP_InputField heightInput;    // 높이 (m) 
        [SerializeField] private TMP_InputField nameInput;      // 방 이름
        [SerializeField] private Button createButton;           // 방 만들기 버튼
        [SerializeField] private Button addFurnitureButton;     // 가구 추가 버튼 (방 생성전까지는 비활성화)
        
        [Header("References")]
        [SerializeField] private RoomBuilder roomBuilder;
        [SerializeField] private FurnitureBounds furnitureBounds;
        [SerializeField] private CameraController cameraController;
        
        private RoomData currentRoomData;

        private void Start()
        {
            createButton.onClick.AddListener(OnCreateRoomClicked);
            panel.SetActive(true);
            
            // 방 생성 전에는 가구 추가 버튼 비활성화
            if (addFurnitureButton != null)
                addFurnitureButton.interactable = false;
        }
        
        
        /// <summary>
        /// "방 만들기" 버튼 클릭 시 호출
        /// 입력값을 파싱하고, 방을 생성하고, 관련 시스템에 크기를 전달한다.
        /// </summary>
        private void OnCreateRoomClicked()
        {
            // 입력값 파싱 (기본값: 4x3x2.5m)
            float width, depth, height;
            if (!float.TryParse(widthInput.text, out width)) width = 4f;
            if (!float.TryParse(depthInput.text, out depth))depth = 3f;
            if (!float.TryParse(heightInput.text, out height))height = 2.5f;
            
            // 범위 제한 (1 ~ 20m)
            width = Mathf.Clamp(width, 1f, 20f);
            depth = Mathf.Clamp(depth, 1f, 20f);
            height = Mathf.Clamp(height, 1.5f, 5f);
            
            // 방 이름
            string roomName = nameInput.text;
            if (string.IsNullOrEmpty(roomName))
                roomName = "방";
            
            // 방 생성
            currentRoomData = new RoomData(width, depth, height, roomName);
            roomBuilder.BuildRoom(currentRoomData);
            
            // 카메라 설정
            if (cameraController != null)
                cameraController.SetTargetPoint(Vector3.zero);
            
            // 충돌 감지에 방 크기 전달
            if (furnitureBounds != null)
                furnitureBounds.SetRoomSize(width, depth);
            
            // 가구 추가 버튼 활성화
            if (addFurnitureButton != null)
                addFurnitureButton.interactable = true;
            
            // 패널 숨기기
            panel.SetActive(false);
        }

        
        /// <summary>
        /// 현재 방 데이터 반환, 저장할 때 사용
        /// </summary>
        public RoomData GetCurrentRoomData()
        {
            return currentRoomData;
        }


        /// <summary>
        /// 방 설정 화면 다시 열기 (메뉴에서 "새 방 만들기" 등)
        /// </summary>
        public void OpenPanel()
        {
            panel.SetActive(true);
        }
    }
}