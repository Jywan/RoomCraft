using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RoomCraft.Data
{
    /// <summary>
    /// 가구 프리셋(즐겨찾기)을 프로젝트와 무관하게 전역으로 저장/불러오기
    /// ProjectManager와 달리 방 하나에 종속되지 않고, 씬을 옮겨다녀도 유지됨
    /// </summary>
    public class FurniturePresetManager : MonoBehaviour
    {
        private string presetFilePath;

        private void Awake()
        {
            presetFilePath = Path.Combine(Application.persistentDataPath, "FurniturePresets.json");
        }


        public void SavePreset(FurnitureData data)
        {
            FurniturePresetList list = LoadAll();
            list.presets.Add(data);
            File.WriteAllText(presetFilePath, JsonUtility.ToJson(list, true));
        }

        public List<FurnitureData> LoadAllPresets()
        {
            return LoadAll().presets;
        }

        public void DeletePreset(string id)
        {
            FurniturePresetList list = LoadAll();
            list.presets.RemoveAll(p => p.id == id);
            File.WriteAllText(presetFilePath, JsonUtility.ToJson(list, true));
        }

        private FurniturePresetList LoadAll()
        {
            if (!File.Exists(presetFilePath))
                return new FurniturePresetList();
            
            string json = File.ReadAllText(presetFilePath);
            return JsonUtility.FromJson<FurniturePresetList>(json);
        }
    }
}