using RoomCraft.CameraSystem;
using RoomCraft.Furniture;
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
            // if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
            //     return;
            if (EventSystem.current.currentSelectedGameObject != null)
                return;
            
            if (Input.GetKeyDown(KeyCode.T))
                cameraController.ToggleView();
            if (Input.GetKeyDown(KeyCode.G))
                gridSnap.ToggleSnap();
        }

        private void OnMainMenu()
        {
            SceneManager.LoadScene("StartScene");
        }
    }
}