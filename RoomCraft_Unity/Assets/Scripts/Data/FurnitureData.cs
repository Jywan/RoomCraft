using System;
using UnityEngine;

namespace RoomCraft.Data
{
    
    /// <summary>
    /// 가구 한개의 정보를 담는 데이터 클래스
    /// 카테고리, 치수(cm), 이름, 색상 등을 저장한다.
    /// JSON 직렬화를 위해 [Serializable]를 적용
    /// </summary>
    [Serializable]
    public class FurnitureData
    {
        public string id;                       // 고유 식별자
        public string furnitureName;            // 사용자가 지정한 이름 
        public FurnitureCategory category;
        
        // 치수 (cm 단위로 입력받지만, Unity 내부에서는 m로 변환해서 사용)
        public float widthCm;       // 가로 cm
        public float depthCm;       // 세로 cm
        public float heightCm;      // 높이 cm

        public Color color;

        public FurnitureData(string name, FurnitureCategory category, float widthCm, float depthCm, float heightCm)
        {
            this.id = Guid.NewGuid().ToString();
            this.furnitureName = name;
            this.category = category;
            this.widthCm = widthCm;
            this.depthCm = depthCm;
            this.heightCm = heightCm;
            this.color = Color.white;
        }
        
        
        /// <summary>
        /// cm를 m로 변환한 Unity용 크기 반환,
        /// Unity에서 1 unit = 1m 이므로 100으로 나눈다.
        /// </summary>
        public Vector3 GetSizeInMeters()
        {
            return new Vector3(widthCm / 100f, heightCm / 100f, depthCm/ 100f);
        }
    }

    public enum FurnitureCategory
    {
        Bed,
        Desk,
        Chair,
        Wardrobe,
        Sofa,
        Bookshelf,
        TV,
        Fridge,
        Other
    }
}