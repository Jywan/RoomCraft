using RoomCraft.CameraSystem;
using RoomCraft.Data;
using RoomCraft.Furniture;
using UnityEngine;

namespace RoomCraft.Room
{
    public class RoomTestSetup : MonoBehaviour
    {
        private void Start()
        {
            // 방 설정 UI가 방 생성을 담당하므로 여기서는 생성하지 않는 방향으로 진행
        }

        private void Update()
        {
            // UI 입력 중이면 단축키 무시
            if (UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != null)
                return;

            // T 키로 시점 전환 테스트
            if (Input.GetKeyDown(KeyCode.T))
            {
                CameraController cam = FindAnyObjectByType<CameraController>();
                if (cam != null)
                {
                    cam.ToggleView();
                }
            }
            
            // G 키로 그리드 스냅 토글
            if (Input.GetKeyDown(KeyCode.G))
            {
                GridSnap grid = FindObjectOfType<GridSnap>();
                if (grid != null)
                    grid.ToggleSnap();
            }
            
            // S 키로 저장 테스트 
            if (Input.GetKeyDown(KeyCode.S))
            {
                ProjectManager pm = FindAnyObjectByType<ProjectManager>();
                if (pm != null)
                {
                    RoomData room = new RoomData(4f, 3f, 2.5f, "테스트 방");
                    pm.SaveProject("테스트_프로젝트", room);
                }
            }
            
            // L 키로 불러오기 테스트
            if (Input.GetKeyDown(KeyCode.L))
            {
                ProjectManager pm = FindAnyObjectByType<ProjectManager>();
                if (pm != null)
                {
                    string[] files = ProjectManager.GetSavedProjectFiles();
                    if (files.Length > 0)
                    {
                        ProjectData project = pm.LoadProject(files[0]);
                        if (project != null)
                        {
                            // 기존 가구 전부 삭제
                            FurnitureObject[] existing = FindObjectsByType<FurnitureObject>(FindObjectsSortMode.None);
                            foreach (FurnitureObject obj in existing)
                                Destroy(obj.gameObject);
                            
                            // 저장된 가구 복원
                            FurnitureInteraction interaction = FindAnyObjectByType<FurnitureInteraction>();
                            foreach (FurnitureSaveData save in project.furnitureList)
                            {
                                FurnitureData data = save.ToFurnitureData();
                                FurnitureObject furniture = interaction.CreateFurniture(data);
                                furniture.MoveTo(new Vector3(save.posX, 0f, save.posZ));
                                furniture.transform.rotation = Quaternion.Euler(0f, save.rotationY, 0f);
                            }
                        }
                    }
                }
            }
            
        }
    }
}