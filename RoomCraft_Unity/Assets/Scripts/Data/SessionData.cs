namespace RoomCraft.Data
{
    
    /// <summary>
    /// 씬간 데이터 전달용 static 클래스
    /// StartScene에서 설정 -> EditorScene에서 읽어감
    /// </summary>
    public static class SessionData
    {
        public enum Mode
        {
            NewProject,
            LoadProject
        }

        public static Mode CurrentMode { get; set; }
        
        // 새 프로젝트일 때 사용
        public static string RoomName { get; set; }
        public static float RoomWidth { get; set; }
        public static float RoomDepth { get; set; }
        public static float RoomHeight { get; set; }
        
        // 불러오기일 때 사용
        public static string LoadFileName { get; set; }


        /// <summary>
        /// 새 프로젝트 모드로 설정
        /// </summary>
        public static void SetNewProject(string name, float width, float depth, float height)
        {
            CurrentMode = Mode.NewProject;
            RoomName = name;
            RoomWidth = width;
            RoomDepth = depth;
            RoomHeight = height;
            LoadFileName = null;
        }

        
        /// <summary>
        /// 불러오기 모드를 설정
        /// </summary>
        public static void SetLoadProject(string fileName)
        {
            CurrentMode = Mode.LoadProject;
            LoadFileName = fileName;
            RoomName = null;
        }
    }
    
}