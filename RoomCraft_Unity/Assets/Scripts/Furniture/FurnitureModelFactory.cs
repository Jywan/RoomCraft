using RoomCraft.Data;
using UnityEngine;

namespace RoomCraft.Furniture
{
    
    /// <summary>
    /// 카테고리에 따라 서로 다른 3D 형태의 가구 메시를 조합해서 생성하는 팩토리
    /// 단순 Cube 대신 여러 Cube를 조합해서 가구 실루엣을 만든다
    /// FurnitureInteraction.CreateFurniture()에서 호출된다.
    /// </summary>
    public static class FurnitureModelFactory
    {
        
        /// <summary>
        /// 카테고리에 맞는 가구 GameObject를 생성해서 반환한다.
        /// 내부적으로 빈 부보 오브젝트 아래에 파츠(Cube)들을 조합한다
        /// </summary>
        public static GameObject Create(FurnitureData data)
        {
            switch (data.category)
            {
                case FurnitureCategory.Bed:
                    return CreateBed(data);
                case FurnitureCategory.Desk:
                    return CreateDesk(data);
                case FurnitureCategory.Chair:
                    return CreateChair(data);
                case FurnitureCategory.Sofa:
                    return CreateSofa(data);
                default:
                    return CreateBox(data); // 옷장, 책장, TV, 냉장고, 기타 -> 단순 박스로 (추후 수정하는 방향으로 하거나 유지)
            }
        }

        
        /// <summary>
        /// 침대: 납작한 본체 + 헤드보드
        /// 본체는 낮고 넓은 직육면체, 헤드보드는 뒤쪽에 세운 얇은 판
        /// </summary>
        private static GameObject CreateBed(FurnitureData data)
        {
            Vector3 size = data.GetSizeInMeters();
            GameObject root = new GameObject("BedRoot");
            
            // 본체 매트리스
            GameObject body = CreatePart("Body", root,
                new Vector3(0f, size.y * 0.4f, 0f),
                new Vector3(size.x, size.y * 0.8f, size.z));
            
            // 헤드보드
            GameObject headboard = CreatePart("Headboard", root,
                new Vector3(0f, size.y * 0.75f, size.z / 2f - 0.04f),
                new Vector3(size.x, size.y * 0.5f, 0.04f));

            ApplyColor(root, data.color);
            return root;
        }

        
        /// <summary>
        /// 책상: 상판 + 다리 4개
        /// 상판은 위쪽에 넓고 얇은 판, 다리는 4개 모서리 에 가는 기둥으로
        /// </summary>
        private static GameObject CreateDesk(FurnitureData data)
        {
            Vector3 size = data.GetSizeInMeters();
            GameObject root = new GameObject("DeskRoot");

            float topThickness = 0.03f;
            float legWidth = 0.04f;
            
            // 상판
            CreatePart("Top", root,
                new Vector3(0f, size.y - topThickness / 2f, 0f),
                new Vector3(size.x, topThickness, size.z));
            
            // 다리가 4개
            float legHeight = size.y - topThickness;
            float xOff = size.x / 2f - legWidth / 2f - 0.02f;
            float zOff = size.z / 2f - legWidth / 2f - 0.02f;
            
            CreatePart("Leg_FL", root, new Vector3(-xOff, legHeight / 2f, -zOff), new Vector3(legWidth, legHeight, legWidth));
            CreatePart("Leg_FR", root, new Vector3(xOff, legHeight / 2f, -zOff), new Vector3(legWidth, legHeight, legWidth));
            CreatePart("Leg_BL", root, new Vector3(-xOff, legHeight / 2f, zOff), new Vector3(legWidth, legHeight, legWidth));
            CreatePart("Leg_BR", root, new Vector3(xOff, legHeight / 2f, zOff), new Vector3(legWidth, legHeight, legWidth));
            
            ApplyColor(root, data.color);
            return root;
        }


        /// <summary>
        /// 의자: 좌석 + 등받이 + 다리 4개
        /// 좌석은 중간높이에 등받이는 뒤쪽에 세로로
        /// </summary>
        private static GameObject CreateChair(FurnitureData data)
        {
            Vector3 size = data.GetSizeInMeters();
            GameObject root = new GameObject("ChairRoot");

            float seatHeight = size.y * 0.45f;
            float seatThickness = 0.03f;
            float legWidth = 0.03f;
            
            // 좌석
            CreatePart("Seat", root,
                new Vector3(0f, seatHeight, 0f),
                new Vector3(size.x, seatThickness, size.z));
            
            // 등받이
            CreatePart("Back", root,
                new Vector3(0f, size.y * 0.72f, size.z / 2f - 0.03f),
                new Vector3(size.x, size.y * 0.5f, 0.03f));
            
            // 다리 4개
            float xOff = size.x / 2f - legWidth / 2f - 0.01f;
            float zOff = size.z / 2f - legWidth / 2f - 0.01f;

            CreatePart("Leg_FL", root, new Vector3(-xOff, seatHeight / 2f, -zOff), new Vector3(legWidth, seatHeight, legWidth));
            CreatePart("Leg_FR", root, new Vector3(xOff, seatHeight / 2f, -zOff), new Vector3(legWidth, seatHeight, legWidth));
            CreatePart("Leg_BL", root, new Vector3(-xOff, seatHeight / 2f, zOff), new Vector3(legWidth, seatHeight, legWidth));
            CreatePart("Leg_BR", root, new Vector3(xOff, seatHeight / 2f, zOff), new Vector3(legWidth, seatHeight, legWidth));
            
            ApplyColor(root, data.color);
            return root;
        }

        
        /// <summary>
        /// 소파: 넓고 낮은 본체 + 등받이 + 팔걸이 2개
        /// </summary>
        private static GameObject CreateSofa(FurnitureData data)
        {
            Vector3 size = data.GetSizeInMeters();
            GameObject root = new GameObject("SofaRoot");
            
            float seatHeight = size.y * 0.45f;
            float armWidth = 0.08f;
            
            // 좌석
            CreatePart("Seat", root,
                new Vector3(0f, seatHeight / 2f, 0f),
                new Vector3(size.x - armWidth * 2f, seatHeight, size.z));
            
            // 등받이
            CreatePart("Back", root,
                new Vector3(0f, size.y * 0.6f, size.z /2f - 0.06f),
                new Vector3(size.x - armWidth * 2f, size.y * 0.5f, 0.08f));
            
            // 팔걸이 좌측
            CreatePart("Arm_L", root,
                new Vector3(-size.x / 2f + armWidth, size.y * 0.4f, 0f),
                new Vector3(armWidth, size.y * 0.6f, size.z));
            
            // 팔걸이 우측
            CreatePart("Arm_R", root,
                new Vector3(size.x / 2f - armWidth, size.y * 0.4f, 0f),
                new Vector3(armWidth, size.y * 0.6f, size.z));
            
            ApplyColor(root, data.color);
            return root;
        }

        
        /// <summary>
        /// 기본 박스: 옷장, 책장, TV, 냉장고, 기타
        /// 단순 Cube 하나로 처리
        /// </summary>
        private static GameObject CreateBox(FurnitureData data)
        {
            Vector3 size = data.GetSizeInMeters();
            GameObject root = new GameObject("BoxRoot");

            CreatePart("Body", root,
                new Vector3(0f, size.y / 2f, 0f),
                size);
            
            ApplyColor(root, data.color);
            return root;
        }
        
        // ===== 유틸 =====
        
        /// <summary>
        /// 파츠(Cube) 하나를 생성해서 부모에 붙인다.
        /// Collider는 isTrigger로 설정 (물리 충돌 방지)
        /// </summary>
        private static GameObject CreatePart(string name, GameObject parent, Vector3 localPos, Vector3 scale)
        {
            GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.name = name;
            part.transform.parent = parent.transform;
            part.transform.localPosition = localPos;
            part.transform.localScale = scale;
            
            // 물리 충돌 수정
            Object.Destroy(part.GetComponent<Collider>());
            
            return part;
        }
        
        
        /// <summary>
        /// root 아래 모든 Renderer에 같은 색을 적용한다.
        /// </summary>
        private static void ApplyColor(GameObject root, Color color)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                r.material.color = color;
            }
        }
    }
}