using RoomCraft.Data;
using RoomCraft.Room;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoomCraft.UI
{
    /// <summary>
    /// EditorScene 전용 저장 UI
    ///  저장 버튼 -> 이름 입력 팝업 -> 저장 처리
    /// </summary>
    public class EditorSaveUI : MonoBehaviour
    {
        [Header("Main")] 
        [SerializeField] private Button saveButton;
        
        
        [Header("Save panel")]
        [SerializeField] private GameObject savePanel;
        [SerializeField] private TMP_InputField saveNameInput;
        [SerializeField] private Button confirmSaveButton;
        [SerializeField] private Button cancelSaveButton;
        
        [Header("References")]
        [SerializeField] private ProjectManager projectManager;
        [SerializeField] private RoomBuilder roomBuilder;

        private void Start()
        {
            savePanel.SetActive(false);
            
        }


        private void OpenSavePanel()
        {
            savePanel.SetActive(true);
            saveNameInput.text = "";
        }

        private void CloseSavePanel()
        {
            savePanel.SetActive(false);
        }

        private void OnConfirmSave()
        {
            string projectName = saveNameInput.text;
            if (string.IsNullOrEmpty(projectName))
                projectName = "내 프로젝트";
            
            RoomData roomData = roomBuilder.GetCurrentRoomData();
            if (roomData == null)
            {
                Debug.Log("방이 생성되지 않았습니다.");
                return;
            }
            
            projectManager.SaveProject(projectName, roomData);
            savePanel.SetActive(false);
            Debug.Log($"저장 완료: {projectName}");
        }
    }
}