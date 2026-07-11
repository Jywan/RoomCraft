using System.Collections.Generic;
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
        private const float EyeHeight = 1.6f;           // 1인칭 시점

        public const float WallThickness = 0.1f;       // 벽 두께. WallSnap 등 외부에서도 참조
        
        public void BuildRoom(RoomData data)
        {
            // 기존 방 제거
            if (currentRoom != null)
                Destroy(currentRoom);
            
            currentRoomData = data;
            
            currentRoom = new GameObject($"Room_{data.roomName}");

            List<Vector2> outline = data.GetVertices();
            
            CreateFloor(data, outline);
            CreateWalls(data, outline);
        }
        
        /// <summary>
        /// 다각형 외곽선을 삼각분할해서 바닥 메쉬를 생성!
        /// </summary>
        private void CreateFloor(RoomData data, List<Vector2> outline)
        {
            List<int> triangles = PolygonUtils.Triangulate(outline);

            // (x, y) 평면 삼각형을 (x, 0, y)로 매핑하면 법선이 아래를 향해서, 각 삼각형의 마지막 두 인덱스를 뒤집어 위를 향하게 한다
            for (int i = 0; i + 2 < triangles.Count; i += 3)
            {
                (triangles[i + 1], triangles[i + 2]) = (triangles[i + 2], triangles[i + 1]);
            }

            Vector3[] vertices3D = new Vector3[outline.Count];
            Vector2[] uvs = new Vector2[outline.Count];
            for (int i = 0; i < outline.Count; i++)
            {
                vertices3D[i] = new Vector3(outline[i].x, 0f, outline[i].y);
                uvs[i] = outline[i];
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices3D;
            mesh.uv = uvs;
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            GameObject floor = new GameObject("Floor");
            floor.transform.parent = currentRoom.transform;
            floor.transform.localPosition = new Vector3(0f, -0.01f, 0f);

            floor.AddComponent<MeshFilter>().mesh = mesh;
            MeshRenderer renderer = floor.AddComponent<MeshRenderer>();
            if (floorMaterial != null)
                renderer.material = floorMaterial;

            floor.AddComponent<MeshCollider>().sharedMesh = mesh;

            floor.layer = LayerMask.NameToLayer("Default");
            floor.tag = "Floor";
        }
        
        /// <summary>
        /// 외곽선 꼭짓점을 순회하면서 각 변마다 벽 하나씩 생성
        /// </summary>
        private void CreateWalls(RoomData data, List<Vector2> outline)
        {
            for (int i = 0; i < outline.Count; i++)
            {
                Vector2 start = outline[i];
                Vector2 end = outline[(i + 1) % outline.Count];
                CreateWallSegment($"Wall_{i}", data, start, end, WallThickness);
            }
        }
        
        private void CreateWallSegment(string name, RoomData data, Vector2 start, Vector2 end, float wallThickness)
        {
            Vector2 mid = (start + end) / 2f;
            float length = Vector2.Distance(start, end);
            
            float dx = end.x - start.x;
            float dz = end.y - start.y;
            float angle = Mathf.Atan2(-dz, dx) * Mathf.Rad2Deg;
            
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.parent = currentRoom.transform;
            wall.transform.localPosition = new Vector3(mid.x, data.height / 2f, mid.y);
            wall.transform.localRotation = Quaternion.Euler(0f, angle, 0f);
            wall.transform.localScale = new Vector3(length, data.height, wallThickness);
            
            if (wallMaterial != null)
                wall.GetComponent<Renderer>().material = wallMaterial;
            
            wall.tag = "Wall";
        }
        
        
        /// <summary>
        /// 1인칭 시점 시작 위치. 바닥 삼각분할의 첫번때 삼각형 무게중심을 씀
        /// 오목한 방 모양이어도 항상 바닥 위의 한점이 보장됨
        /// </summary>
        public Vector3 GetFirstPersonSpawnPoint()
        {
            if (currentRoom == null) return Vector3.zero;
            
            List<Vector2> outline = currentRoomData.GetVertices();
            List<int> triangles = PolygonUtils.Triangulate(outline);
            
            if (triangles.Count < 3)  return Vector3.zero;

            Vector2 a = outline[triangles[0]];
            Vector2 b = outline[triangles[1]];
            Vector2 c = outline[triangles[2]];
            Vector2 centroid = (a + b + c) / 3f;
            
            return new Vector3(centroid.x, EyeHeight, centroid.y);
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
