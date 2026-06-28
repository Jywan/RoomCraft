using RoomCraft.Data;
using RoomCraft.Furniture;
using TMPro;
using UnityEngine;

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
        
        private void Start()
        {
            panel.SetActive(false);
        }
        
        private void Update()
        {
            FurnitureObject selected = interaction.GetSelectedFurniture();

            if (selected != null)
            {
                ShowInfo(selected);
            }
            else
            {
                HidePanel();
            }
        }
        

        /// <summary>
        /// 선택된 가구 정보를 패널에 표시한다.
        /// </summary>
        private void ShowInfo(FurnitureObject furniture)
        {
            panel.SetActive(true);

            FurnitureData data = furniture.GetData();

            nameText.text = data.furnitureName;
            sizeText.text = $"{data.widthCm} x {data.depthCm} x {data.heightCm} cm";
            
            Vector3 pos = furniture.transform.position;
            posText.text = $"위치: ({pos.x:F2}, {pos.z:F2})";
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