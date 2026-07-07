using System.Collections.Generic;
using RoomCraft.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RoomCraft.UI
{
    /// <summary>
    /// 완전 독립된 커스텀 방 모약 편집 패널
    /// 그리드 점을 순서대로 클릭해서 다각형을 그리고, 그대로 방 생성에 사용하는 방향으로 재개발
    /// </summary>
    public class CustomRoomPanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_InputField roomNameInput;
        [SerializeField] private TMP_InputField widthInput;
        [SerializeField] private TMP_InputField depthInput;
        [SerializeField] private TMP_InputField heightInput;
        [SerializeField] private RectTransform gridArea;
        [SerializeField] private GameObject pointPrefab;
        [SerializeField] private GameObject linePrefab;
        [SerializeField] private Button generateGridButton;
        [SerializeField] private Button undoButton;
        [SerializeField] private Button clearButton;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        
        [Header("Settings")]
        [SerializeField] private float cellSize = 0.5f;
        [SerializeField] private float cellPixelSize = 40f;
        [SerializeField] private float snapCloseRadius = 20f;
        
        private List<Vector2> clickedPoints = new List<Vector2>();
        private List<GameObject> pointMarkers = new List<GameObject>();
        private List<GameObject> lineSegments = new List<GameObject>();
        private bool isClosed = false;
        private float gridWidth;
        private float gridDepth;
        private float effectiveCellPx;

        private void Awake()
        {
            generateGridButton.onClick.AddListener(GenerateGrid);
            undoButton.onClick.AddListener(UndoLastPoint);
            clearButton.onClick.AddListener(ClearAll);
            confirmButton.onClick.AddListener(OnConfirm);
            cancelButton.onClick.AddListener(ClosePanel);
            panel.SetActive(false);
        }

        public void OpenPanel()
        {
            panel.SetActive(true);
            roomNameInput.text = "";
            widthInput.text = "";
            depthInput.text = "";
            heightInput.text = "";
            ClearGrid();
            ClearAll();
        }

        private void ClosePanel()
        {
            panel.SetActive(false);
        }

        /// <summary>탭 전환 등 외부(StartSceneUI)에서 패널을 닫을 때 사용</summary>
        public void ClosePanelExternally()
        {
            ClosePanel();
        }

        /// <summary>커스텀 탭의 취소 버튼에 StartSceneUI가 추가로 리스너를 걸 수 있도록 노출</summary>
        public Button CancelButton => cancelButton;

        /// <summary>
        /// 가로/세로 입력값 기준으로 점 그리드를 생성한다.
        /// </summary>
        private void GenerateGrid()
        {
            ClearGrid();
            ClearAll();
            
            gridWidth = ParseFloat(widthInput.text, 4f);
            gridDepth = ParseFloat(depthInput.text, 3f);
            
            int cols = Mathf.Max(1, Mathf.RoundToInt(gridWidth / cellSize)) + 1;
            int rows = Mathf.Max(1, Mathf.RoundToInt(gridDepth / cellSize)) + 1;

            // 그리드가 GridArea보다 커지지 않도록 점 간격을 자동으로 축소
            effectiveCellPx = cellPixelSize;
            if (cols > 1)
                effectiveCellPx = Mathf.Min(effectiveCellPx, gridArea.rect.width / (cols - 1));
            if (rows > 1)
                effectiveCellPx = Mathf.Min(effectiveCellPx, gridArea.rect.height / (rows - 1));

            float totalWidth = (cols - 1) * effectiveCellPx;
            float totalHeight = (rows - 1) * effectiveCellPx;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    float x = c * effectiveCellPx - totalWidth / 2f;
                    float y = r * effectiveCellPx - totalHeight / 2f;
                    Vector2 pos = new Vector2(x, y);
                    
                    GameObject point = Instantiate(pointPrefab, gridArea);
                    point.GetComponent<RectTransform>().anchoredPosition = pos;
                    
                    Button btn = point.GetComponent<Button>();
                    btn.onClick.AddListener(() => OnPointClicked(pos));
                    
                    pointMarkers.Add(point);
                }
            }
        }

        private void OnPointClicked(Vector2 pos)
        {
            if (isClosed) return;

            if (clickedPoints.Count >= 3 && Vector2.Distance(pos, clickedPoints[0]) < snapCloseRadius)
            {
                DrawLine(clickedPoints[clickedPoints.Count - 1], clickedPoints[0]);
                isClosed = true;
                return;
            }
            
            if (clickedPoints.Count > 0)
                DrawLine(clickedPoints[clickedPoints.Count - 1], pos);
            
            clickedPoints.Add(pos);
        }
        
        private void DrawLine(Vector2 from, Vector2 to)
        {
            GameObject line = Instantiate(linePrefab, gridArea);
            RectTransform rt = line.GetComponent<RectTransform>();
            
            Vector2 mid = (from + to) / 2f;
            float length = Vector2.Distance(from, to);
            float angle = Mathf.Atan2(to.y - from.y, to.x - from.x) * Mathf.Rad2Deg;

            rt.anchoredPosition = mid;
            rt.sizeDelta = new Vector2(length, rt.sizeDelta.y);
            rt.localRotation = Quaternion.Euler(0f, 0f, angle);
            
            lineSegments.Add(line);
        }
        
        private void UndoLastPoint()
        {
            if (isClosed)
            {
                RemoveListLine();
                isClosed = false;
                return;
            }

            if (clickedPoints.Count == 0) return;
            
            clickedPoints.RemoveAt(clickedPoints.Count - 1);
            RemoveListLine();
        }
        
        private void RemoveListLine()
        {
            if (lineSegments.Count == 0) return;
            Destroy(lineSegments[lineSegments.Count - 1]);
            lineSegments.RemoveAt(lineSegments.Count - 1);
        }

        private void ClearAll()
        {
            foreach (GameObject line in lineSegments)
                Destroy(line);
            lineSegments.Clear();
            
            clickedPoints.Clear();
            isClosed = false;
        }

        private void ClearGrid()
        {
            foreach (GameObject point in pointMarkers)
                Destroy(point);
            pointMarkers.Clear();
        }

        private void OnConfirm()
        {
            if (!isClosed || clickedPoints.Count < 3)
            {
                Debug.LogWarning("점을 3개 이상 찍고 도형을 닫아야 방을 만들수 있음!");
                return;
            }
            
            string roomName = roomNameInput.text;
            if (string.IsNullOrEmpty(roomName))
                roomName = "내 방";
            
            float height = ParseFloat(heightInput.text , 2.5f);
            
            List<Vector2> vertices = new List<Vector2>();
            foreach (Vector2 p in clickedPoints)
                vertices.Add(p / effectiveCellPx * cellSize);
            
            SessionData.CustomVertices = vertices;
            SessionData.SetNewProject(roomName, gridWidth, gridDepth, height, RoomShape.Custom);
            SceneManager.LoadScene("EditorScene");
        }

        private float ParseFloat(string input, float defaultValue)
        {
            if (float.TryParse(input, out float result))
                return result;
            return defaultValue;
        }
    }
}