using RoomCraft.Data;
using UnityEngine;

namespace RoomCraft.Room
{
    public class RoomBuilder : MonoBehaviour
    {
        [Header("Room Settings")]
        [SerializeField] private Material floorMaterial;
        [SerializeField] private Material wallMaterial;
        
        private GameObject currentRoom;
        private RoomData currentRoomData;

        public void BuildRoom(RoomData data)
        {
            // 기존 방 제거
            if (currentRoom != null)
                Destroy(currentRoom);
            
            currentRoomData = data;
            
            currentRoom = new GameObject($"Room_{data.roomName}");

            CreateFloor(data);
            CreateWalls(data);
        }

        private void CreateFloor(RoomData data)
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor";
            floor.transform.parent = currentRoom.transform;
            
            // 바닥 : 가로 x 세로, 두께 0.1m
            floor.transform.localScale = new Vector3(data.width, 0.1f, data.depth);
            floor.transform.localPosition = new Vector3(0f, -0.05f, 0f);
            
            if (floorMaterial != null)
                floor.GetComponent<Renderer>().material = floorMaterial;

            floor.layer = LayerMask.NameToLayer("Default");
            floor.tag = "Floor";
        }

        private void CreateWalls(RoomData data)
        {
            float wallThickness = 0.1f;
            
            // 벽 4면 생성: 앞, 뒤, 좌, 우
            CreateWall("Wall_Back", data,
                new Vector3(0f, data.height / 2f, data.depth / 2f),
                new Vector3(data.width, data.height, wallThickness));
            
            CreateWall("Wall_Front", data,
                new Vector3(0f, data.height / 2f, -data.depth / 2f),
                new Vector3(data.width, data.height, wallThickness));
            
            CreateWall("Wall_Left", data,
                new Vector3(-data.width / 2f, data.height / 2f, 0f),
                new Vector3(wallThickness, data.height, data.depth));
            
            CreateWall("Wall_Right", data,
                new Vector3(data.width / 2f, data.height / 2f, 0f),
                new Vector3(wallThickness, data.height, data.depth));

        }

        private void CreateWall(string name, RoomData data, Vector3 position, Vector3 scale)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.parent = currentRoom.transform;
            wall.transform.localPosition = position;
            wall.transform.localScale = scale;
            
            if (wallMaterial != null)
                wall.GetComponent<Renderer>().material = wallMaterial;
            
            wall.tag = "Wall";
        }

        public void ClearRoom()
        {
            if (currentRoom != null)
                Destroy(currentRoom);
        }
        
        public RoomData GetCurrentRoomData()
        {
            return currentRoomData;
        }
    }
}
