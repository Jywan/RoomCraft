using UnityEngine;

namespace RoomCraft.Furniture
{
    
    /// <summary>
    /// 그리드 스밴 유틸리티
    /// 가구 이동 시 지정된 간격으로 위치를 정렬해준다.
    /// FurnitureInteraction HandleDrag에서 MoveTo 전에 호출한다.
    /// </summary>
    public class GridSnap : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private float gridSize = 0.1f;         // 스냅 간격 (m), 0.1 = 10cm 단위
        [SerializeField] private bool snapEnabled = true;

        
        /// <summary>
        /// 위치를 그리드에 스냅한 결과를 반환한다.
        /// X, Z만 스냅하고 Y는 그대로 둔다.
        /// </summary>
        public Vector3 Snap(Vector3 position)
        {
            if (!snapEnabled)
                return position;
            
            float x = Mathf.Round(position.x / gridSize) * gridSize;
            float z = Mathf.Round(position.z / gridSize) * gridSize;
            
            return new Vector3(x, position.y, z);
        }


        /// <summary>
        /// 그리드 스냅 on/off 토글.
        /// </summary>
        public void ToggleSnap()
        {
            snapEnabled = !snapEnabled;
            Debug.Log($"그리드 스냅: {(snapEnabled ? "ON" : "OFF")}");
        }

        
        /// <summary>
        /// 스냅 간격 변경
        /// </summary>
        public void SetGridSize(float size)
        {
            gridSize = Mathf.Max(0.01f, size);    
        }
        
        public bool IsEnabled()
        {
            return snapEnabled;   
        }
        
        public float GetGridSize()
        {
            return gridSize;
        }
    }
}