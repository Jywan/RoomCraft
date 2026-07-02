using System.Collections.Generic;
using System.IO;
using RoomCraft.Furniture;
using UnityEngine;

namespace RoomCraft.Data
{
    
    /// <summary>
    /// 프로젝트 저장/불러오기를 담당하는 매니저
    /// JSON 파일을 Application.persistentDataPath에 저장한다.
    /// 씬하나만 존재.
    /// </summary>
    public class ProjectManager : MonoBehaviour
    {
        private string saveDirectory;

        private void Awake()
        {
            saveDirectory = Path.Combine(Application.persistentDataPath, "Projects");
            
            // 저장 폴더가 없으면 생성
            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);
        }

        
        /// <summary>
        /// 현재 씬의 방 + 가구 배치를 JSON으로 저장한다.
        /// 파일명은 프로젝트 이름 기반으로..
        /// </summary>
        public void SaveProject(string projectName, RoomData roomData)
        {
            ProjectData project = new ProjectData();
            project.projectName = projectName;
            project.roomData = roomData;
            
            // 씬에 있는 모든 가구를 수집
            FurnitureObject[] allFurniture = FindObjectsByType<FurnitureObject>(FindObjectsSortMode.None);
            project.furnitureList = new List<FurnitureSaveData>();

            foreach (FurnitureObject obj in allFurniture)
            {
                project.furnitureList.Add(FurnitureSaveData.FromFurnitureObject(obj));
            }
            
            // JSON 직렬화
            string json = JsonUtility.ToJson(project, true);
            
            // 파일 저장
            string fileName = SanitizeFileName(projectName) + ".json";
            string filePath = Path.Combine(saveDirectory, fileName);
            File.WriteAllText(filePath, json);
            
            Debug.Log($"프로젝트 저장완료: {filePath}");
        }
        
        
        /// <summary>
        /// JSON 파일에서 프로젝트를 불러온다.
        /// 방을 재생성하고 가구를 배치
        /// </summary>
        public ProjectData LoadProject(string fileName)
        {
            string filePath = Path.Combine(saveDirectory, fileName);

            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"파일 없음: {filePath}");
                return null;
            }
            
            string json = File.ReadAllText(filePath);
            ProjectData project = JsonUtility.FromJson<ProjectData>(json);
            
            Debug.Log($"프로젝트 불러오기 완료: {project.projectName} (가구 {project.furnitureList.Count}개)");
            return project;
        }
        
    
        /// <summary>
        /// 저장된 프로젝트 파일 목록을 반환한다.
        /// 씬 전환을 추가하여 static으로 변경
        /// </summary>
        public static string[] GetSavedProjectFiles()
        {
            string dir = Path.Combine(Application.persistentDataPath, "Projects");

            if (!Directory.Exists(dir))
                return new string[0];
            
            string[] files = Directory.GetFiles(dir, "*.json");
            
            // 파일명만 추출
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileName(files[i]);
            }
            
            return files;
        }
        
        
        /// <summary>
        /// 저장된 프로젝트를 삭제한다.
        /// </summary>
        public void DeleteProject(string fileName)
        {
            string filepath = Path.Combine(saveDirectory, fileName);
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
                Debug.Log($"프로젝트 삭제: {fileName}");
            }
        }
        
        
        /// <summary>
        /// 파일명에 사용할 수 없는 문자를 제거
        /// </summary>
        private string SanitizeFileName(string name)
        {
            char[] invalid = Path.GetInvalidFileNameChars();
            foreach (char c in invalid)
            {
                name = name.Replace(c, '_');
            }
            return name;
        }
    }
}