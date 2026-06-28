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
            RoomBuilder builder = GetComponent<RoomBuilder>();
            
            // 테스트 4m x 3m x 2.5 방
            RoomData testRoom = new RoomData(4f, 3f, 2.5f, "테스트방");
            builder.BuildRoom(testRoom);
            
            // 카메라를 방 중심으로 설정
            CameraController cam = FindAnyObjectByType<CameraController>();
            if (cam != null)
            {
                cam.SetTargetPoint(Vector3.zero);
            }
            
            // 충돌 감지에 방 크기 전달
            FurnitureBounds bounds = FindAnyObjectByType<FurnitureBounds>();
            if (bounds != null)
            {
                bounds.SetRoomSize(testRoom.width, testRoom.depth);
            }
            
            // 테스트 가구 생성
            FurnitureInteraction interaction = FindObjectOfType<FurnitureInteraction>();
            if (interaction != null)
            {
                // 책상: 120cm x 60cm x 75cm
                FurnitureData desk = new FurnitureData("테스트 책상", FurnitureCategory.Desk, 120f, 60f, 75f);
                desk.color = new Color(0.6f, 0.4f, 0.2f); // 갈색
                interaction.CreateFurniture(desk);
                
                // 침대: 200cm x 150cm x 40cm
                FurnitureData bed = new FurnitureData("테스트 침대",  FurnitureCategory.Bed, 200f, 150f, 40f);
                bed.color = new Color(0.8f, 0.8f, 0.9f); // 연보라
                FurnitureObject bedObj = interaction.CreateFurniture(bed);
                bedObj.MoveTo(new Vector3(1f, 0f, 0.5f));   // 오른쪽으로 배치
                
            }
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
                    string[] files = pm.GetSavedProjectFiles();
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