using UnityEngine;
using UnityEngine.UI;

namespace RoomCraft.UI
{
    /// <summary>
    /// 전체 UI 흐름을 관리하는 최상위 매니저
    /// 가구 추가 버튼 등 메인 UI 버튼들의 이벤트를 연결한다.
    /// </summary>
    public class UIManager :  MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button addFurnitureButton;     // 가구 추가버튼
        [SerializeField] private FurnitureCreatorUI creatorUI;  // 가구 생성 팝업
        
        private void Start()
        {
            addFurnitureButton.onClick.AddListener(OnAddFurnitureClicked);    
        }

        
        /// <summary>
        /// 가구 추가 버튼 클릭 시 생성 팝업을 연다.
        /// </summary>
        private void OnAddFurnitureClicked()
        {
            creatorUI.OpenPanel();
        }
        
    }
}