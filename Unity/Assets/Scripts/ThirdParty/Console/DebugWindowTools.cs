using UnityEngine;

namespace XGame
{
    public class DebugWindowTools : DebugWindowBase
    {
        private Vector2 _scrollPos; 

        private float _timeScale = 1;

        protected override void OnDrawWindow(int id)
        {
            var richText = new GUIStyle();
            richText.richText = true;
            GUILayout.BeginHorizontal();
            GUILayout.Label($"<color=white>TimeScale   {Time.timeScale:F2}</color>", richText);
            if (GUILayout.Button("reset", GUILayout.Width(60)))
            {
                _timeScale = 1;
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("0", GUILayout.Width(20));
            _timeScale = GUILayout.HorizontalSlider(_timeScale, 0.01f, 2f);
            GUILayout.Label("2", GUILayout.Width(20));
            GUILayout.EndHorizontal();

            _scrollPos = GUILayout.BeginScrollView(_scrollPos, GUILayout.MinHeight(200), GUILayout.MaxHeight(_windowRect.height - 300));
            if (GUILayout.Button("打开缓存目录", GUILayout.Width(100)))
            {
#if UNITY_EDITOR
                System.Diagnostics.Process.Start("explorer.exe", Application.persistentDataPath.Replace('/', '\\'));
#elif UNITY_ANDROID

                    try
                    {
                        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                        AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                        AndroidJavaClass androidSystem = new AndroidJavaClass("com.gztools.androidkits.KitHelper");
                        androidSystem.CallStatic<string>(
                            "openAssignFolder",
                            context,
                            Application.persistentDataPath.Substring(Application.persistentDataPath.IndexOf("Android", StringComparison.Ordinal))
                            );
                    }
                    catch (Exception e)
                    {
                        
                    }
#endif
            }

            GUILayout.EndScrollView();

        }
    }
}