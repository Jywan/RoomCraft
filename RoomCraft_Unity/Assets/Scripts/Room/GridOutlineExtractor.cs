using System.Collections.Generic;
using UnityEngine;

namespace RoomCraft.Room
{
    /// <summary>
    /// 그리드 셀 선택 상태로부터 외곽선(다각형)을 추출
    /// 선택 영역이 하나로 이어져 있다고 가정 (구멍/분리된 영역/대각선 연결은 지원X - 나중에 추가 가능할수도 있음.)
    /// </summary>
    public static class GridOutlineExtractor
    {
        public static List<Vector2> ExtraOutline(bool[,] cells, int rows, int cols, float cellSize)
        {
            List<(Vector2Int from, Vector2Int to)> edges = new List<(Vector2Int from, Vector2Int to)>();

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (!cells[r, c]) continue;
                    
                    if (!IsSelected(cells, rows, cols, r - 1, c))
                        edges.Add((new Vector2Int(c, r), new Vector2Int(c + 1, r)));
                    
                    if (!IsSelected(cells, rows, cols, r + 1, c))
                        edges.Add((new Vector2Int(c + 1, r + 1), new Vector2Int(c, r + 1)));
                    
                    if (!IsSelected(cells, rows, cols, r, c - 1))
                        edges.Add((new Vector2Int(c, r + 1), new Vector2Int(c, r)));
                    
                    if (!IsSelected(cells, rows, cols, r, c + 1))
                        edges.Add((new Vector2Int(c + 1, r), new Vector2Int(c + 1, r + 1)));
                }
            }
            
            List<Vector2Int> loop = ChainEdge(edges);

            float halfW = cols * cellSize / 2;
            float halfD = rows * cellSize / 2;
            
            List<Vector2> vertices = new List<Vector2>();
            foreach (Vector2Int p in loop)
                vertices.Add(new Vector2(p.x * cellSize - halfW, p.y * cellSize - halfD));
            
            return vertices;
        }
        
        private static bool IsSelected(bool[,] cells, int rows, int cols, int r, int c)
        {
            if (r < 0 || r >= rows || c < 0 || c >= cols) return false;
            return cells[r, c];
        }

        
        /// <summary>
        /// 방향성 있는 테두리 번들을 이어붙여 하나의 폐곡선 꼭짓점 리스트로 만듦
        /// </summary>
        private static List<Vector2Int> ChainEdge(List<(Vector2Int from, Vector2Int to)> edges)
        {
            Dictionary<Vector2Int, Vector2Int> nextPoint = new Dictionary<Vector2Int, Vector2Int>();
            foreach (var edge in edges)
                nextPoint[edge.from] = edge.to;

            List<Vector2Int> loop = new List<Vector2Int>();
            if (edges.Count == 0) return loop;
            
            Vector2Int start = edges[0].from;
            Vector2Int current = start;

            int guard = 0;

            do
            {
                loop.Add(current);
                if (!nextPoint.TryGetValue(current, out current))
                    break;
                guard++;
            } while (current != start && guard < edges.Count + 1);
            
            return loop;
        }
    }
}