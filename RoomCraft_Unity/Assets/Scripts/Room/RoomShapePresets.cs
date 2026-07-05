using System.Collections.Generic;
using RoomCraft.Data;
using UnityEngine;

namespace RoomCraft.Room
{
    /// <summary>
    /// 방 모양 프리셋별로 꼭짓점 리스트를 생성한다.
    /// </summary>
    public static class RoomShapePresets
    {
        public static List<Vector2> GetVertices(RoomShape shape, float width, float depth)
        {
            switch (shape)
            {
                case RoomShape.LShape:
                    return LShapeVertices(width, depth);
                default:
                    return RectangleVertices(width, depth);
            }
        }

        private static List<Vector2> RectangleVertices(float width, float depth)
        {
            float halfW = width / 2f;
            float halfD = depth / 2f;
            return new List<Vector2>
            {
                new Vector2(-halfW, -halfD),
                new Vector2(halfW, -halfD),
                new Vector2(halfW, halfD),
                new Vector2(-halfW, halfD),
            };
        }
        
        /// <summary>
        /// 우측 상단 사분면을 잘라낸 L자형 (6꼭짓점)
        /// </summary>
        private static List<Vector2> LShapeVertices(float width, float depth)
        {
            float halfW = width / 2f;
            float halfD = depth / 2f;
            return new List<Vector2>
            {
                new Vector2(-halfW, -halfD),
                new Vector2(halfW, -halfD),
                new Vector2(halfW, 0f),
                new Vector2(0f, 0f),
                new Vector2(0f, halfD),
                new Vector2(-halfW, halfD),
            };
        }
    }
}