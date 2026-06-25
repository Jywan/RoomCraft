using UnityEngine;
using System;

namespace RoomCraft.Data
{
    [Serializable]
    public class RoomData
    {
        public float width;     // 가로 (m)
        public float depth;     // 세로 (m)
        public float height;    // 높이 (m)
        public string roomName;

        public RoomData(float width, float depth, float height, string roomName = "새 방")
        {
            this.width = width;
            this.depth = depth;
            this.height = height;
            this.roomName = roomName;
        }
    }
}