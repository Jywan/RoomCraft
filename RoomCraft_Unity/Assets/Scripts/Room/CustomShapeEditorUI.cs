using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoomCraft.Room
{
    /// <summary>
    /// 새 프로젝트 화면의 "커스텀" 탭에 내장되는 그리드 셀 편집기
    /// 그리드를 만들고 셀 토글을 처리, 확인시 꼭짓점 리스트를 뽑아줌
    /// </summary>
    public class CustomShapeEditorUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private RectTransform gridContainer;
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private TextMeshProUGUI sizeInfoText;
        
        [Header("Settings")]
        [SerializeField] private float cellSize = 0.5f;         // 셀 하나의 실제 크기(m)
        [SerializeField] private float cellPixelSize = 45f;     // 셀 하나의 화면 크기(px)
        [SerializeField] private Color onColor = Color.white;
        [SerializeField] private Color offColor = new Color(0.3f, 0.3f, 0.3f, 1f);

        private bool[,] cells;
        private int rows;
        private int cols;
        
        /// <summary>
        /// 주언진 가로/세로(m) 기준으로 그리드를 새로 만든다. 탭 진입시 호출
        /// </summary>
        public void RebuildGrid(float width, float depth)
        {
            cols = Mathf.Max(1, Mathf.RoundToInt(width / cellSize));
            rows = Mathf.Max(1, Mathf.RoundToInt(depth / cellSize));

            if (sizeInfoText != null)
                sizeInfoText.text = $"가로 {width:F1}m x 세로 {depth:F1}m (칸당 {cellSize:F1}m x {cellSize:F1}m)";
            
            foreach (Transform child in gridContainer)
                Destroy(child.gameObject);

            cells = new bool[rows, cols];
            gridLayoutGroup.cellSize = new Vector2(cellPixelSize, cellPixelSize);
            gridLayoutGroup.constraintCount = cols;

            for (int r = rows - 1; r >= 0; r--)
            {
                for (int c = 0; c < cols; c++)
                {
                    cells[r, c] = false;
                    
                    GameObject cellObj = Instantiate(cellPrefab, gridContainer);
                    Image img = cellObj.GetComponent<Image>();
                    img.color = offColor;
                    
                    int capturedR = r;
                    int capturedC = c;
                    Button btn = cellObj.GetComponent<Button>();
                    btn.onClick.AddListener(() => ToggleCell(capturedR, capturedC, img));
                }
            }

        }

        private void ToggleCell(int r, int c, Image img)
        {
            cells[r, c] = !cells[r, c];
            img.color = cells[r, c] ? onColor : offColor;
        }
        
        /// <summary>
        /// 현재 그리드 상태로부터 꼭짓점 리스트를 추출한다.
        /// </summary>
        public List<Vector2> GetVertices()
        {
            if (cells == null) return new List<Vector2>();
            return GridOutlineExtractor.ExtraOutline(cells, rows, cols, cellSize);
        }
    }
}