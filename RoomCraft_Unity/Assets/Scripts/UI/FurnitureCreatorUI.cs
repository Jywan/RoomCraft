using System.Collections.Generic;
using RoomCraft.Data;
using RoomCraft.Furniture;
using TMPro;
using UnityEngine;
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
        
        [Header("Color Picker")]
        [SerializeField] private ColorPickerUI colorPicker;
        
        [Header("References")]
        [SerializeField] private FurnitureInteraction furnitureInteraction;
        [SerializeField] private FurniturePresetManager presetManager;
        
        [Header("Preset")]
        [SerializeField] private TMP_Dropdown presetDropdown;
        [SerializeField] private Button savePresetButton;
        
        private Color selectedColor = Color.white;
        private List<FurnitureData> currentPresets = new List<FurnitureData>();
        private TextMeshProUGUI savePresetButtonLabel;
        
        /// <summary>
        /// 초기화: 버튼 이벤트 연결, 드롭다운 옵션 세팅
        /// </summary>
        private void Start()
        {
            // 버튼 이벤트 연결
            createButton.onClick.AddListener(OnCreateClicked);
            cancelButton.onClick.AddListener(OnCancelClicked);
            savePresetButton.onClick.AddListener(OnSavePresetClicked);
            presetDropdown.onValueChanged.AddListener(OnPresetSelected);
            
            // 카테고리 드롭다운 옵션 세팅
            SetupCategoryDropdown();
            
            // 색상 버튼 이벤트 연결
            SetupColorButtons();
            
            // 색상 피커 이벤트 연결
            colorPicker.OnColorChanged += SelectColor;
            
            savePresetButtonLabel = savePresetButton.GetComponentInChildren<TextMeshProUGUI>();
            
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
            FurnitureData data = BuildFurnitureDataFromForm();
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
            colorPicker.SetColor(Color.white);
            
            RefreshPresetDropdown();
        }

        public FurnitureData BuildFurnitureDataFromForm()
        {
            string furnitureName = nameInput.text;
            if (string.IsNullOrEmpty(furnitureName))
                furnitureName = "이름없는 가구";
            
            float width, depth, height;
            if (!float.TryParse(widthInput.text, out width)) width = 50f;
            if (!float.TryParse(depthInput.text, out depth)) depth = 50f;
            if (!float.TryParse(heightInput.text, out height)) height = 50f;
            
            width = Mathf.Clamp(width, 1f, 500f);
            depth =  Mathf.Clamp(depth, 1f, 500f);
            height = Mathf.Clamp(height, 1f, 500f);
            
            FurnitureCategory category = (FurnitureCategory)categoryDropdown.value;
            
            FurnitureData data = new FurnitureData(furnitureName, category, width, depth, height);
            data.color = selectedColor;
            return data;
        }

        private void OnSavePresetClicked()
        {
            FurnitureData data = BuildFurnitureDataFromForm();
            presetManager.SavePreset(data);
            RefreshPresetDropdown();
            presetDropdown.SetValueWithoutNotify(currentPresets.Count);     // 방금 저장된 프리셋 항목을 선택한 상태로 전환

            if (savePresetButtonLabel != null)
            {
                savePresetButtonLabel.text = "저장됨!";
                Invoke(nameof(ResetSavePresetButtonLabel), 1f);
            }
        }

        private void ResetSavePresetButtonLabel()
        {
            savePresetButtonLabel.text = "프리셋으로 저장";
        }

        private void OnPresetSelected(int index)
        {
            if (index <= 0 || index > currentPresets.Count) return;     // 0번은 "프리셋 선택"
            
            FurnitureData preset = currentPresets[index - 1];
            nameInput.text = preset.furnitureName;
            widthInput.text = preset.widthCm.ToString();
            depthInput.text = preset.depthCm.ToString();
            heightInput.text = preset.heightCm.ToString();
            categoryDropdown.value = (int)preset.category;
            SelectColor(preset.color);
        }

        private void RefreshPresetDropdown()
        {
            currentPresets = presetManager.LoadAllPresets();
            
            presetDropdown.ClearOptions();
            List<string> options = new List<string> { "프리셋 선택" };
            foreach (FurnitureData preset in currentPresets)
                options.Add(preset.furnitureName);
            
            presetDropdown.AddOptions(options);
            presetDropdown.SetValueWithoutNotify(0);
        }
    }
}