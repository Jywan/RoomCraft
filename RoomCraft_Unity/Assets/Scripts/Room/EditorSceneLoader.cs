using RoomCraft.CameraSystem;
using RoomCraft.Data;
using RoomCraft.Furniture;
using UnityEngine;

namespace RoomCraft.Room
{
    
    /// <summary>
    /// EditorScene 진입 시 SessionData를 읽어서
    /// 새 프로젝트면 방 생성, 불러오기면 프로젝트 복원
    /// RoomManager 오브젝트에 부착
    /// </summary>
    public class EditorSceneLoader : MonoBehaviour
    {
        [SerializeField] private RoomBuilder roomBuilder;
        [SerializeField] private ProjectManager projectManager;
        [SerializeField] private FurnitureInteraction furnitureInteraction;

        private void Start()
        {
            if (SessionData.CurrentMode == SessionData.Mode.NewProject)
            {
                HandleNewProject();
            }
            else if (SessionData.CurrentMode == SessionData.Mode.LoadProject)
            {
                HandleLoadProject();
            }
        }

        private void HandleNewProject()
        {
            RoomData roomData = new RoomData(
                SessionData.RoomWidth,
                SessionData.RoomDepth,
                SessionData.RoomHeight,
                SessionData.RoomName);
            
            roomBuilder.BuildRoom(roomData);
            
            CameraController cam = FindAnyObjectByType<CameraController>();
            if (cam != null)
            {
                cam.SetTargetPoint(Vector3.zero);
                cam.SwitchMode(CameraMode.TopView);
            }
        }

        private void HandleLoadProject()
        {
            if (string.IsNullOrEmpty(SessionData.LoadFileName))
            {
                Debug.LogWarning("불러올 파일명이 없습니다.");
                return;
            }
            
            ProjectData project = projectManager.LoadProject(SessionData.LoadFileName);
            if (project == null) return;
            
            // 방 재생성
            if (project.roomData != null)
            {
                roomBuilder.BuildRoom(project.roomData);
            }
            
            // 저장된 가구 복원
            foreach (FurnitureSaveData save in project.furnitureList)
            {
                FurnitureData data = save.ToFurnitureData();
                FurnitureObject furniture = furnitureInteraction.CreateFurniture(data);
                furniture.MoveTo(new Vector3(save.posX, 0f, save.posZ));
                furniture.transform.rotation = Quaternion.Euler(0f, save.rotationY, 0f);
            }
        }
    }
}