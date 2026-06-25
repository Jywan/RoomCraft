using RoomCraft.CameraSystem;
using RoomCraft.Data;
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
        }

        private void Update()
        {
            // T 키로 시점 전환 테스트
            if (Input.GetKeyDown(KeyCode.T))
            {
                CameraController cam = FindAnyObjectByType<CameraController>();
                if (cam != null)
                {
                    cam.ToggleView();
                }
            }
        }
    }
}