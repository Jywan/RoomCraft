using System.Collections.Generic;
using UnityEngine;

namespace RoomCraft.Room
{
    /// <summary>
    /// 다각형 관련 유틸리티
    /// Ear Clipping으로 단순 다각형(오목한 모양 포함)을 삼각분할
    /// </summary>
    public static class PolygonUtils
    {
        /// <summary>
        /// 다각형을 삼각분할해서 인덱스 리스트를 반환한다 (3개씩 묶어서 삼각형 하나)
        /// </summary>
        public static List<int> Triangulate(List<Vector2> polygon)
        {
            List<int> indices = new List<int>();
            List<Vector2> points = new List<Vector2>(polygon);
            
            // Ear Clipping은 반시계방향(CCW) 기준이라, 시계방향이면 뒤집는다.
            if (SignedArea(points) < 0f)
                points.Reverse();
            
            List<int> remaining = new List<int>();
            for (int i = 0; i < points.Count; i++)
                remaining.Add(i);

            int guard = 0;
            while (remaining.Count > 3 && guard < 1000)
            {
                guard++;
                bool earFound = false;

                for (int i = 0; i < remaining.Count; i++)
                {
                    int iPrev = remaining[(i -1 + remaining.Count) % remaining.Count];
                    int iCurr = remaining[i];
                    int iNext = remaining[(i + 1) % remaining.Count];

                    Vector2 a = points[iPrev];
                    Vector2 b = points[iCurr];
                    Vector2 c = points[iNext];

                    if (!IsConvex(a, b, c)) continue;

                    bool anyInside = false;
                    for (int j = 0; j < remaining.Count; j++)
                    {
                        int idx = remaining[j];
                        if (idx == iPrev || idx == iCurr || idx == iNext) continue;
                        if (PointInTriangle(points[idx], a, b, c))
                        {
                            anyInside = true;
                            break;
                        }
                    }
                    
                    if (anyInside) continue;
                    
                    indices.Add(iPrev);
                    indices.Add(iCurr);
                    indices.Add(iNext);
                    remaining.RemoveAt(i);
                    earFound = true;
                    break;
                }

                if (!earFound) break;       // 자기 교차 등 이상 다각형이면 여기서 중단분기를 둠
            }

            if (remaining.Count == 3)
            {
                indices.Add(remaining[0]);
                indices.Add(remaining[1]);
                indices.Add(remaining[2]);
            }
            
            return indices;
        }
        
        
        /// <summary>
        /// 점이 다각형 내부에 있는지 판정 (Ray Casting)
        /// </summary>
        public static bool PointInPolygon(Vector2 point, List<Vector2> polygon)
        {
            bool inside = false;
            int n = polygon.Count;

            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                Vector2 pi = polygon[i];
                Vector2 pj = polygon[j];
                
                // 판정 로직
                bool intersects = ((pi.y > point.y) != (pj.y > point.y)) &&
                                  (point.x < (pj.x - pi.x) * (point.y - pi.y) / (pj.y - pi.y) + pi.x);
                
                if (intersects)
                    inside = !inside;
            }
            return inside;
        }

        private static float SignedArea(List<Vector2> pts)
        {
            float area = 0f;
            for (int i = 0; i < pts.Count; i++)
            {
                Vector2 p1 = pts[i];
                Vector2 p2 = pts[(i + 1) % pts.Count];
                area += (p1.x * p2.y - p2.x * p1.y);
            }

            return area / 2f;
        }
        
        private static bool IsConvex(Vector2 a, Vector2 b, Vector2 c)
        {
            float cross = (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
            return cross > 0f;
        }

        private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            float d1 = Cross(p, a, b);
            float d2 = Cross(p, b, c);
            float d3 = Cross(p, c, a);
            
            bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);
            
            return !(hasNeg && hasPos);
        }

        private static float Cross(Vector2 p, Vector2 a, Vector2 b)
        {
            return (b.x - a.x) * (p.y - a.y) - (b.y - a.y) * (p.x - a.x);
        }
    }
}