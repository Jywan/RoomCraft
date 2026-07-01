using RoomCraft.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RoomCraft.UI
{
    /// <summary>
    /// StartScene UI 컨트롤러
    /// 새 프로젝트 생성 또는 기존 프로젝트 불러오기 처리한 뒤 EditorScene으로 전환
    /// </summary>
    public class StartSceneUI : MonoBehaviour
    {
        [Header("Main Buttons")]
        [SerializeField] private Button newProjectButton;
        [SerializeField] private Button loadProjectButton;
        
        [Header("New Project Panel")]
        [SerializeField] private GameObject newProjectPanel;
        [SerializeField] private TMP_InputField roomNameInput;
        [SerializeField] private TMP_InputField widthInput;
        [SerializeField] private TMP_InputField depthInput;
        [SerializeField] private TMP_InputField heightInput;
        [SerializeField] private Button confirmNewButton;
        [SerializeField] private Button cancelNewButton;
        
        [Header("Load Panel")]
        [SerializeField] private GameObject loadPanel;
        [SerializeField] private Transform fileListContent;
        [SerializeField] private GameObject fileItemPrefab;
        [SerializeField] private Button cancelLoadButton;

        private void Start()
        {
            newProjectPanel.SetActive(false);
            loadPanel.SetActive(false);
            
            newProjectButton.onClick.AddListener(OpenNewProjectPanel);
            loadProjectButton.onClick.AddListener(OpenLoadPanel);
            confirmNewButton.onClick.AddListener(OnConfirmNewProject);
            cancelNewButton.onClick.AddListener(CloseNewProjectPanel);
            cancelLoadButton.onClick.AddListener(CloseLoadPanel);
        }
        
        
        // ===== 새 프로젝트 =====
        
        private void OpenNewProjectPanel()
        {
            newProjectPanel.SetActive(true);
            roomNameInput.text = "";
            widthInput.text = "";
            depthInput.text = "";
            heightInput.text = "";
        }
        
        private void CloseNewProjectPanel()
        {
            newProjectPanel.SetActive(false);
        }

        private void OnConfirmNewProject()
        {
            string roomName = roomNameInput.text;
            if (string.IsNullOrEmpty(roomName))
                roomName = "내 방";

            float width = ParseFloat(widthInput.text, 4f);
            float depth = ParseFloat(depthInput.text, 3f);
            float height = ParseFloat(heightInput.text, 2.5f);
            
            SessionData.SetNewProject(roomName, width, depth, height);
            SceneManager.LoadScene("EditorScene");
        }
        
        // ===== 불러오기 =====
        private void OpenLoadPanel()
        {
            loadPanel.SetActive(true);
            RefreshFileList();
        }

        private void CloseLoadPanel()
        {
            loadPanel.SetActive(false);
        }

        private void RefreshFileList()
        {
            foreach (Transform child in fileListContent)
            {
                Destroy(child.gameObject);
            }

            string[] files = ProjectManager.GetSavedProjectFiles();

            foreach (string fileName in files)
            {
                GameObject item = Instantiate(fileItemPrefab, fileListContent);
                TextMeshProUGUI text = item.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                    text.text = fileName.Replace(".json", "");

                string file = fileName;
                Button btn = item.GetComponent<Button>();
                btn.onClick.AddListener(() => OnFileSelected(file));
            }
        }

        private void OnFileSelected(string fileName)
        {
            SessionData.SetLoadProject(fileName);
            SceneManager.LoadScene("EditorScene");
        }
        
        // ===== 유틸 =====
        
        private float ParseFloat(string input, float defaultValue)
        {
            if (float.TryParse(input, out float result))
                return result;
            return defaultValue;
        }
    }
}