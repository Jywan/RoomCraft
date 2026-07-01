using RoomCraft.Data;
using RoomCraft.Furniture;
using RoomCraft.Room;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoomCraft.UI
{
    /// <summary>
    /// 저장/불러오기 UI 컨트롤러.
    /// 저장: 현재 배치를 프로젝트 이름으로 JSON 저장
    /// 불러오기: 저장된 파일 목록을 표시하고 선택하면 복원
    /// </summary>
    public class SaveLoadUI : MonoBehaviour
    {
        [Header("Main Buttons")]
        [SerializeField] private Button saveButton;                 // 저장 버튼 (상단에 둘 예정)
        [SerializeField] private Button loadButton;                 // 불러오기 버튼 (이것도 상단에 둘 예정)
        
        [Header("Save Panel")]
        [SerializeField] private GameObject savePanel;              // 저장 팝업
        [SerializeField] private TMP_InputField saveNameInput;      // 프로젝트 이름 입력
        [SerializeField] private Button confirmSaveButton;          // 저장하기 버튼
        [SerializeField] private Button cancelSaveButton;           // 저장 팝업 취소버튼
        
        [Header("Load Panel")]
        [SerializeField] private GameObject loadPanel;              // 불러오기 팝업
        [SerializeField] private Transform fileListContent;         // 파일 목록 스크롤 영역 (Content)
        [SerializeField] private GameObject fileItemPrefab;         // 파일 항목 프리랩 (버튼)
        [SerializeField] private Button cancelLoadButton;           // 불러오기 팝업 취소버튼

        [Header("References")]
        [SerializeField] private ProjectManager projectManager;
        [SerializeField] private FurnitureInteraction furnitureInteraction;
        [SerializeField] private RoomSetupUI roomSetupUI;

        private void Start()
        {
            saveButton.onClick.AddListener(OpenSavePanel);
            loadButton.onClick.AddListener(OpenLoadPanel);
            confirmSaveButton.onClick.AddListener(OnConfirmSave);
            cancelSaveButton.onClick.AddListener(CloseSavePanel);
            cancelLoadButton.onClick.AddListener(CloseLoadPanel);
            
            savePanel.SetActive(false);
            loadPanel.SetActive(false);
        }
        
        // ===== 저장 스크립트 부분 =====

        
        /// <summary>
        /// 저장 팝업을 열기
        /// </summary>
        private void OpenSavePanel()
        {
            savePanel.SetActive(true);
            saveNameInput.text = "";
        }

        
        /// <summary>
        /// 저장 팝업을 닫기
        /// </summary>
        private void CloseSavePanel()
        {
            savePanel.SetActive(false);
        }

        
        /// <summary>
        /// 저장하기 확인 버튼 클릭 로직
        /// 프로젝트 이름을 받고 현재 배치를 저장!
        /// </summary>
        private void OnConfirmSave()
        {
            string projectName = saveNameInput.text;
            if (string.IsNullOrEmpty(projectName))
                projectName = "내 프로젝트";

            RoomData roomData = roomSetupUI.GetCurrentRoomData();
            if (roomData == null)
            {
                Debug.LogWarning("방이 생성되지 않았습니다.");
                return;
            }
            
            projectManager.SaveProject(projectName, roomData);
            savePanel.SetActive(false);
        }
        
        
        // ===== 불러오기 스크립트 부분 =====

        /// <summary>
        /// 불러오기 팝업을 열고, 저장된 파일 목록을 표시!
        /// </summary>
        private void OpenLoadPanel()
        {
            loadPanel.SetActive(true);
            RefreshFileList();
        }

        
        /// <summary>
        /// 불러오기 팝업 닫기
        /// </summary>
        private void CloseLoadPanel()
        {
            loadPanel.SetActive(false);
        }

        
        /// <summary>
        /// 파일 목록을 갱신
        /// 기존 목록을 지우고, 저장된 파일마다 버튼을 생성
        /// </summary>
        private void RefreshFileList()
        {
            // 기존 목록 제거
            foreach (Transform child in fileListContent)
            {
                Destroy(child.gameObject);
            }
            
            // 저장된 파일 목록 가져오기
            string[] files = ProjectManager.GetSavedProjectFiles();

            foreach (string fileName in files)
            {
                // 파일 항목 버튼 생성
                GameObject item = Instantiate(fileItemPrefab, fileListContent);
                TextMeshProUGUI text = item.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                    text.text = fileName.Replace(".json", "");
                
                // 클릭 시 해당 파일 불러오기
                string file = fileName;     // 클로저용 복사
                Button btn = item.GetComponent<Button>();
                btn.onClick.AddListener(() => OnFileSelected(file));
            }
        }

        
        /// <summary>
        /// 파일 목록에서 항목을 클릭 했을 때 호출!
        /// 해당 프로젝트를 불러와서 씬에 복원하는 로직!
        /// </summary>
        private void OnFileSelected(string fileName)
        {
            ProjectData project = projectManager.LoadProject(fileName);
            if (project == null) return;
            
            // 기존 가구 전부 삭제
            FurnitureObject[] existing = FindObjectsByType<FurnitureObject>(FindObjectsSortMode.None);
            foreach (FurnitureObject obj in existing)
                Destroy(obj.gameObject);
            
            // 방 재생성 (저장된 방 크기로)
            if (project.roomData != null)
            {
                RoomBuilder builder = FindAnyObjectByType<Room.RoomBuilder>();
                if (builder != null)
                    builder.BuildRoom(project.roomData);
            }
            
            // 저장된 가구 복원
            foreach (FurnitureSaveData save in project.furnitureList)
            {
                FurnitureData data = save.ToFurnitureData();
                FurnitureObject furniture = furnitureInteraction.CreateFurniture(data);
                furniture.MoveTo(new Vector3(save.posX, 0f, save.posZ));
                furniture.transform.rotation = Quaternion.Euler(0f, save.rotationY, 0f);
            }
            
            loadPanel.SetActive(false);
        }
    }
}