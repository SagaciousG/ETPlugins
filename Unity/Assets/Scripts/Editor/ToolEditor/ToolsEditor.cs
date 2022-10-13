using UnityEditor;

namespace ET
{
    public static class ToolsEditor
    {
        [MenuItem("Build/编译Excel")]
        public static void ExcelExporter()
        {
#if UNITY_EDITOR_OSX
            const string tools = "./Tool";
#else
            const string tools = ".\\Tool.exe";
#endif
            ShellHelper.Run($"{tools} --AppType=ExcelExporter --Console=1", "../Bin/");
        }
        
        [MenuItem("Build/编译Proto")]
        public static void Proto2CS()
        {
#if UNITY_EDITOR_OSX
            const string tools = "./Tool";
#else
            const string tools = ".\\Tool.exe";
#endif
            ShellHelper.Run($"{tools} --AppType=Proto2CS --Console=1", "../Bin/");
        }
    }
}