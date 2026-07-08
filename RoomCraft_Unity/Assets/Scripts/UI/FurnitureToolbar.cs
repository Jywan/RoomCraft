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
        
        [Header("Preview")]
        [SerializeField] private Image previewImage;
        [SerializeField] private Sprite[] categoryIcons;              // FurnitureCategory enum 순서와 동일하게 9개로 지정
        
        [Header("Rotation")]
        [SerializeField] private Slider rotationSlider;
        
        private void Start()
        {
            rotateLeftButton.onClick.AddListener(OnRotateLeft);
            rotateRightButton.onClick.AddListener(OnRotateRight);
            deleteButton.onClick.AddListener(OnDelete);
            rotationSlider.onValueChanged.AddListener(OnRotationSliderChanged);
            
            panel.SetActive(false);
        }
        
        private void Update()
        {
            bool hasSelection = furnitureInteraction.HasSelection;
            panel.SetActive(hasSelection);
    
            if (hasSelection)
            {
                FurnitureObject selected = furnitureInteraction.GetSelectedFurniture();
                UpdatePreview(selected);
                rotationSlider.SetValueWithoutNotify(selected.GetRotationY());
            }
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
        
        /// <summary>
        /// 선택된 가구의 카테고리에 맞는 아이콘을 미리보기에 표시
        /// </summary>
        private void UpdatePreview(FurnitureObject selected)
        {
            int index = (int)selected.GetData().category;
            if (categoryIcons == null || index < 0 || index >= categoryIcons.Length) return;
            previewImage.sprite = categoryIcons[index];
        }
        
        private void OnRotationSliderChanged(float angleY)
        {
            if (furnitureInteraction != null)
                furnitureInteraction.SetSelectedRotation(angleY);
        }
    }
}