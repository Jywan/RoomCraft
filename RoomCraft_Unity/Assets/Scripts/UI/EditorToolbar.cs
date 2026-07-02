using System;
using RoomCraft.CameraSystem;
using RoomCraft.Furniture;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace RoomCraft.UI
{
    /// <summary>
    /// EditorScene 하단 툴바
    /// 키보드 단축키와 UI 버튼 모두 같은 기능을 실행
    /// </summary>
    public class EditorToolbar : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button viewToggleButton;
        [SerializeField] private Button gridToggleButton;
        [SerializeField] private Button rotateLeftButton;
        [SerializeField] private Button rotateRightButton;
        [SerializeField] private Button deleteButton;
        [SerializeField] private Button saveButton;
        
        [Header("References")]
        [SerializeField] private FurnitureInteraction furnitureInteraction;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private GridSnap gridSnap;
        [SerializeField] private EditorSaveUI editorSaveUI;
        
        private void Start()
        {
            mainMenuButton.onClick.AddListener(OnMainMenu);
            viewToggleButton.onClick.AddListener(OnViewToggle);
            gridToggleButton.onClick.AddListener(OnGridToggle);
            rotateLeftButton.onClick.AddListener(OnRotateLeft);
            rotateRightButton.onClick.AddListener(OnRotateRight);
            deleteButton.onClick.AddListener(OnDelete);
            saveButton.onClick.AddListener(OnSave);
        }

        private void Update()
        {
            // UI 입력 중이면 단축키 무시
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
                return;
            
            if (Input.GetKeyDown(KeyCode.T))
                OnViewToggle();
            if (Input.GetKeyDown(KeyCode.G))
                OnGridToggle();
        }


        private void OnViewToggle()
        {
            if (cameraController != null)
                cameraController.ToggleView();
        }

        private void OnGridToggle()
        {
            if (gridSnap != null)
                gridSnap.ToggleSnap();
        }

        private void OnRotateLeft()
        {
            if (furnitureInteraction != null)
                furnitureInteraction.RotateSelected(-45f);
        }

        private void OnRotateRight()
        {
            if (furnitureInteraction != null)
                furnitureInteraction.RotateSelected(45f);
        }

        private void OnDelete()
        {
            if (furnitureInteraction != null)
                furnitureInteraction.DeleteSelected();
        }
        
        private void OnMainMenu()
        {
            SceneManager.LoadScene("StartScene");
        }

        private void OnSave()
        {
            if (editorSaveUI != null)
                editorSaveUI.OpenSavePanel();
        }
    }
}