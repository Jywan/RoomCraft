using System;
using System.Collections.Generic;

namespace RoomCraft.Data
{
    /// <summary>
    /// FurnitureData 리스트를 JsonUtility로 직렬화하기 위한 래퍼
    /// (JsonUtility는 최상위 List를 직접 직렬화 못 함)
    /// </summary>
    [Serializable]
    public class FurniturePresetList
    {
        public List<FurnitureData> presets = new List<FurnitureData>();
    }
}