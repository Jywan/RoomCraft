using RoomCraft.CameraSystem;
using RoomCraft.Data;
using RoomCraft.Furniture;
using RoomCraft.UndoRedo;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RoomCraft.UI
{
    
    /// <summary>
    /// EditorScene 하단 룸 툴바
    /// 키보드 단축키와 UI 버튼 모두 같은 기능을 실행
    /// </summary>
    public class RoomToolbar : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button viewToggleButton;
        [SerializeField] private Button gridToggleButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button zoomInButton;
        [SerializeField] private Button zoomOutButton;

        [Header("References")]
        [SerializeField] private CameraController cameraController;
        [SerializeField] private GridSnap gridSnap;
        [SerializeField] private EditorSaveUI editorSaveUI;
        [SerializeField] private UndoManager undoManager;
        [SerializeField] private WallSnap wallSnap;
        
        private void Start()
        {
            mainMenuButton.onClick.AddListener(OnMainMenu);
            viewToggleButton.onClick.AddListener(cameraController.ToggleView);
            gridToggleButton.onClick.AddListener(gridSnap.ToggleSnap);
            saveButton.onClick.AddListener(editorSaveUI.OpenSavePanel);
            zoomInButton.onClick.AddListener(() => cameraController.Zoom(1f));
            zoomOutButton.onClick.AddListener(() => cameraController.Zoom(-1f));
        }

        private void Update()
        {
            // UI 입력 중이면 단축키 무시
            if (EventSystem.current.currentSelectedGameObject != null)
                return;
            
            if (Input.GetKeyDown(KeyCode.T))
                cameraController.ToggleView();
            if (Input.GetKeyDown(KeyCode.G))
                gridSnap.ToggleSnap();
            if (Input.GetKeyDown(KeyCode.B))
                wallSnap.ToggleSnap();
            
            // Undo/Redo (mac: Cmd / window: ctrl 둘다 지원)
            bool modifier = Input.GetKey(KeyCode.LeftControl) 
                            || Input.GetKey(KeyCode.RightControl) 
                            || Input.GetKey(KeyCode.LeftCommand) 
                            || Input.GetKey(KeyCode.RightCommand);

            if (modifier && Input.GetKeyDown(KeyCode.Z))
            {
                bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                
                if (shift)
                    undoManager.Redo();
                else
                    undoManager.Undo();
            }
        }

        private void OnMainMenu()
        {
            SceneManager.LoadScene("StartScene");
        }
    }
}