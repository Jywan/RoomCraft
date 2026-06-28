using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoomCraft.Data
{
    /// <summary>
    /// 하나의 프로젝트(레이아웃) 전체 정보를 담는 클래스.
    /// 방 정보 + 배치된 가구 목록을 포함한다.
    /// JSON으로 직렬화해서 파일로 저장/불러오기 한다.
    /// </summary>
    [Serializable]
    public class ProjectData
    {
        public string projectName;
        public string createDate;
        public RoomData roomData;
        public List<FurnitureSaveData> furnitureList;

        public ProjectData()
        {
            projectName = "새 프로젝트";
            createDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            furnitureList = new List<FurnitureSaveData>();
        }
    }
    
    
    /// <summary>
    /// 가구 하나의 저장용 데이터
    /// FurnitureData + 씬에서의 위치/회전 정보를 포함한다.
    /// Color는 JSON 직렬화가 안 되므로 float 4개로 분리 저장
    /// </summary>
    [Serializable]
    public class FurnitureSaveData
    {
        public string id;
        public string furnitureName;
        public int categoryIndex;
        public float widthCm;
        public float depthCm;
        public float heightCm;
        
        // Color 를 float으로 분리 (Unity Color는 JSON 직렬화 안됨)
        public float colorR;
        public float colorG;
        public float colorB;
        public float colorA;
        
        // 씬 에서의 위치 회전
        public float posX;
        public float posZ;
        public float rotationY;
        
        /// <summary>
        /// FurnitureObject에서 저장용 데이터를 추출한다.
        /// </summary>
        public static FurnitureSaveData FromFurnitureObject(Furniture.FurnitureObject obj)
        {
            var data = obj.GetData();
            var save = new FurnitureSaveData();

            save.id = data.id;
            save.furnitureName = data.furnitureName;
            save.categoryIndex = (int)data.category;
            save.widthCm = data.widthCm;
            save.depthCm = data.depthCm;
            save.heightCm = data.heightCm;

            save.colorR = data.color.r;
            save.colorG = data.color.g;
            save.colorB = data.color.b;
            save.colorA = data.color.a;
            
            save.posX = obj.transform.position.x;
            save.posZ = obj.transform.position.z;
            save.rotationY = obj.transform.eulerAngles.y;

            return save;
        }

        
        /// <summary>
        /// 저장 데이터를 FurnitureData로 복원
        /// </summary>
        public FurnitureData ToFurnitureData()
        {
            FurnitureData data = new FurnitureData(
                furnitureName,
                (FurnitureCategory)categoryIndex,
                widthCm, depthCm, heightCm
            );
            data.id = id;
            data.color = new Color(colorR, colorG, colorB, colorA);
            return data;
        }
    }
}