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
        }
    }
}