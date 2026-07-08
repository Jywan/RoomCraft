using RoomCraft.Data;
using RoomCraft.Furniture;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoomCraft.UI
{
    /// <summary>
    /// 선택된 가구의 치수(cm)를 화면 하단에 표시하는 UI 컨트롤러
    /// FurnitureInteraction에서 선택 상태가 바뀔 때마다 갱신한다
    /// </summary>
    public class DimensionDisplay : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject panel;              // 치수 표시 패널(하단)
        [SerializeField] private TextMeshProUGUI nameText;      // 가구 이름
        [SerializeField] private TextMeshProUGUI sizeText;      // 가로 x 세로 x 높이
        [SerializeField] private TextMeshProUGUI posText;       // 현재 위치
        
        [Header("References")]
        [SerializeField] private FurnitureInteraction interaction;
        
        private RectTransform panelRect;        // panel의 RectTransform 캐싱용
        private FurnitureObject lastFurniture;
        private Vector3 lastPos;
        
        private void Start()
        {
            panel.SetActive(false);
            panelRect = panel.GetComponent<RectTransform>();            // 여기서 한번만 호출
        }
        
        private void Update()
        {
            if (interaction.HasSelection)
            {
                ShowInfo(interaction.GetSelectedFurniture());
            }
            else
            {
                HidePanel();
                lastFurniture = null;       // 선택 해제시 캐시 초기화하도록 추가
            }
        }
        

        /// <summary>
        /// 선택된 가구 정보를 패널에 표시한다.
        /// </summary>
        private void ShowInfo(FurnitureObject furniture)
        {
            panel.SetActive(true);
            
            bool furnitureChanged = furniture != lastFurniture;     // 선택된 가구 자체가 바뀌었는지 체크
            Vector3 pos = furniture.transform.position;
            bool posChanged = pos != lastPos;                       // 위치가 바뀌었는지 체크

            if (furnitureChanged)
            {
                FurnitureData data = furniture.GetData();
                nameText.text = data.furnitureName;
                sizeText.text = $"{data.widthCm} x {data.depthCm} x {data.heightCm} cm";
            }

            if (furnitureChanged || posChanged)
            {
                posText.text = $"위치: ({pos.x:F2}, {pos.z:F2})";
            }

            if (furnitureChanged)
            {
                // LayoutRebuilder.ForceRebuildLayoutImmediate(panel.GetComponent<RectTransform>()); 캐싱 최적화로 아래로 변경
                LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
            }
            
            // 마지막 값 재세팅
            lastFurniture = furniture;
            lastPos = pos;
        }
        

        /// <summary>
        /// 가구 미선택 시 패널을 숨김
        /// </summary>
        private void HidePanel()
        {
            panel.SetActive(false);
        }
    }
}