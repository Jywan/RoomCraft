using UnityEngine;
using System;
using System.Collections.Generic;

namespace RoomCraft.Data
{
    [Serializable]
    public class RoomData
    {
        public float width;     // 가로 (m), 사각형 프리셋 및 하위 호환용
        public float depth;     // 세로 (m)
        public float height;    // 높이 (m)
        public string roomName;
        
        public List<Vector2> vertices;      // 방 외곡선 (XZ 평명, 중심 기준 상대 좌표), null이면 width/depth로 사각형 생성
        
        public RoomData(float width, float depth, float height, string roomName = "새 방")
        {
            this.width = width;
            this.depth = depth;
            this.height = height;
            this.roomName = roomName;
            this.vertices = null;
        }

        public RoomData(List<Vector2> vertices, float height, string roomName = "새 방")
        {
            this.vertices = vertices;
            this.height = height;
            this.roomName = roomName;
            
            // width/depth는 바운딩 박스 크기로 채워서 표시/호환용으로 남겨두는 방향으로
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;
            foreach (Vector2 v in vertices)
            {
                minX = Mathf.Min(minX, v.x);
                maxX = Mathf.Max(maxX, v.x);
                minY = Mathf.Min(minY, v.y);
                maxY = Mathf.Max(maxY, v.y);
            }
            this.width = maxX - minX;
            this.depth = maxY - minY;
        }

        
        /// <summary>
        /// cm를 m로 변환한 unity용 크기 반환
        /// unity에서 1 unit = 1m 이므로 100으로 나눈다.
        /// </summary>
        public Vector3 GetSizeInMeters()
        {
            return new Vector3(width / 100f, height / 100f, depth / 100f);
        }

        
        /// <summary>
        /// 실제러 사용할 꼭짓점 목록 반환
        /// vertices가 없으면(사각형 기본값적용) width/depth로 사각형을 생성해서 돌려준다
        /// </summary>
        public List<Vector2> GetVertices()
        {
            if (vertices != null && vertices.Count >= 3) 
                return vertices;

            float halfW = width / 2f;
            float halfD = depth / 2f;
            return new List<Vector2>
            {
                new Vector2(-halfW, -halfD),
                new Vector2(halfW, -halfD),
                new Vector2(halfW, halfD),
                new Vector2(-halfW, halfD)
            };
        }
    }
}