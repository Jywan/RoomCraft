using System.Collections.Generic;
using RoomCraft.Data;
using RoomCraft.Furniture;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

namespace RoomCraft.UI
{
    /// <summary>
    /// 가구 생성 팝업 UI를 관리하는 컨트롤러.
    /// 사용자가 카테고리, 치수(cm), 이름, 색상을 입력하면
    /// FurnitureData를 만들어 FurnitureInteraction에 전달한다.
    /// Canvas > Panel 구조의 UI에 붙인다.
    /// </summary>
    public class FurnitureCreatorUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject panel;                          // 팝업 패널
        [SerializeField] private TMP_Dropdown categoryDropdown;             // 카테고리 드롭다운
        [SerializeField] private TMP_InputField nameInput;                  // 가구 이름 입력
        [SerializeField] private TMP_InputField widthInput;                 // 가로 (cm)
        [SerializeField] private TMP_InputField depthInput;                 // 세로 (cm)
        [SerializeField] private TMP_InputField heightInput;                // 높이 (cm)
        [SerializeField] private Button createButton;                       // 생성 버튼 
        [SerializeField] private Button cancelButton;                       // 취소 버튼
        [SerializeField] private Image colorPreview;                        // 색상 미리보기
        
        [Header("Color Buttons")]
        [SerializeField] private Button[] colorButtons;                     // 색상 선택 버튼들
        
        [Header("References")]
        [SerializeField] private FurnitureInteraction furnitureInteraction;
        
        private Color selectedColor = Color.white;
        
        /// <summary>
        /// 초기화: 버튼 이벤트 연결, 드롭다운 옵션 세팅
        /// </summary>
        private void Start()
        {
            // 버튼 이벤트 연결
            createButton.onClick.AddListener(OnCreateClicked);
            cancelButton.onClick.AddListener(OnCancelClicked);
            
            // 카테고리 드롭다운 옵션 세팅
            SetupCategoryDropdown();
            
            // 색상 버튼 이벤트 연결
            SetupColorButtons();
            
            // 시작 시 패널 숨김
            panel.SetActive(false);
        }
        
        
        /// <summary>
        /// 카테고리 드롭다운에 enum 값들을 옵션으로 추가한다.
        /// </summary>
        private void SetupCategoryDropdown()
        {
            categoryDropdown.ClearOptions();
            var options = new List<string>();
            
            // FurnitureCategory enum 값들을 한글로 매핑
            options.Add("침대");
            options.Add("책상");
            options.Add("의자");
            options.Add("옷장");
            options.Add("소파");
            options.Add("책장");
            options.Add("TV/모니터");
            options.Add("냉장고");
            options.Add("기타");
            
            categoryDropdown.AddOptions(options);
        }

        
        /// <summary>
        /// 미리 정의된 색상 버튼들에 클릭 이벤트를 연결한다.
        /// 각버튼의 Image 색상을 선택 색으로 지정.
        /// </summary>
        private void SetupColorButtons()
        {
            if (colorButtons == null) return;

            foreach (Button btn in colorButtons)
            {
                Color btnColor = btn.GetComponent<Image>().color;
                
                btn.onClick.AddListener(() => SelectColor(btnColor));
            }
        }

        
        /// <summary>
        /// 색상 선택 시 호출. 미리보기 업데이트.
        /// </summary>
        private void SelectColor(Color color)
        {
            selectedColor = color;
            if (colorPreview != null)
                colorPreview.color = color;
        }


        private void OnCreateClicked()
        {
            // 입력값 파싱
            string furnitureName = nameInput.text;
            if (string.IsNullOrEmpty(furnitureName))
                furnitureName = "이름없는 가구";
            
            float width, depth, height;
            if (!float.TryParse(widthInput.text, out width)) width = 50f;
            if (!float.TryParse(depthInput.text, out depth)) depth = 50f;
            if (!float.TryParse(heightInput.text, out height)) height = 50f;
            
            // 최소/최대 제한 (1cm ~ 500cm)
            width = Mathf.Clamp(width, 1f, 500f);
            depth = Mathf.Clamp(depth, 1f, 500f);
            height = Mathf.Clamp(height, 1f, 500f);
            
            // 카테고리 변환
            FurnitureCategory category = (FurnitureCategory)categoryDropdown.value;
            
            // 데이터 변환
            FurnitureData data = new FurnitureData(furnitureName, category, width, depth, height);
            data.color = selectedColor;
            
            // 가구 형성
            furnitureInteraction.CreateFurniture(data);
            
            // 팝업 닫기
            panel.SetActive(false);
        }

        
        /// <summary>
        /// 취소 버튼: 팝업을 닫는다
        /// </summary>
        private void OnCancelClicked()
        {
            panel.SetActive(false);
        }


        public void OpenPanel()
        {
            panel.SetActive(true);
            nameInput.text = "";
            widthInput.text = "";
            depthInput.text = "";
            heightInput.text = "";
            categoryDropdown.value = 0;
            selectedColor = Color.white;
            if (colorPreview != null)
                colorPreview.color = Color.white;
        }
    }
}