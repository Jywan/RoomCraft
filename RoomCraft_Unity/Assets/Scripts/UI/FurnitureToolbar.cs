using RoomCraft.Furniture;
using UnityEngine;
using UnityEngine.UI;

namespace RoomCraft.UI
{
    /// <summary>
    /// 가구 선택 시에만 표시되는 툴바 (회전/삭제)
    /// FurnitureInteraction의 선택 생태를 매 프레임 확인래서 패널을 켜고 끈다
    /// </summary>
    public class FurnitureToolbar : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject panel;
        
        [Header("Buttons")]
        [SerializeField] private Button rotateLeftButton;
        [SerializeField] private Button rotateRightButton;
        [SerializeField] private Button deleteButton;
        
        [Header("References")]
        [SerializeField] private FurnitureInteraction furnitureInteraction;
        
        [Header("Position")]
        [SerializeField] private float heightOffset = 0.3f;
        
        private RectTransform panelRect;
        private Camera mainCamera;

        private void Start()
        {
            rotateLeftButton.onClick.AddListener(OnRotateLeft);
            rotateRightButton.onClick.AddListener(OnRotateRight);
            deleteButton.onClick.AddListener(OnDelete);
            
            panelRect = panel.GetComponent<RectTransform>();
            mainCamera = Camera.main;
            
            panel.SetActive(false);
        }
        
        private void Update()
        {
            FurnitureObject selected =
                furnitureInteraction != null ? furnitureInteraction.GetSelectedFurniture() : null;
            if (selected == null)
            {
                panel.SetActive(false);
                return;
            }
            
            panel.SetActive(true);
            UpdatePanelPosition(selected);
        }
        
        
        /// <summary>
        /// 선택된 가구 위쪽 화면 좌표로 패널 위치를 갱신한다.
        /// </summary>
        private void UpdatePanelPosition(FurnitureObject selected)
        {
            float heightM = selected.GetData().heightCm / 100f;
            Vector3 worldPos = selected.transform.position + Vector3.up * (heightM + heightOffset);
            panelRect.position = mainCamera.WorldToScreenPoint(worldPos);
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
    }
}