using UnityEngine;

namespace XGame
{
    public static class ScreenUtil
    {
        public static Vector2 HalfScreen => new Vector2(Screen.width / 2f, Screen.height / 2f);

        public static bool IsInScreen(Camera cam, Vector3 worldPos)
        {
            Vector3 viewPos = cam.WorldToViewportPoint(worldPos);
            Vector3 dir = (worldPos - cam.transform.position).normalized;
            float dot = Vector3.Dot(cam.transform.forward, dir);
            if (dot > 0 && viewPos.x > 0 && viewPos.x < 1 && viewPos.y > 0 && viewPos.y < 1)
            {
                return true;
            }

            return false;
        }
        
    }
}