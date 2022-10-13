using UnityEngine;

namespace XGame
{
    public static class GUIHelper
    {
        public static void Box(Rect rect, Color color)
        {
            GUI.color = color;
            GUI.Box(rect, "", "WhiteBackground");
            GUI.color = Color.white;
        }

        public static TextAnchor TextAlignGUI(TextAnchor align)
        {
            return align;
        }
    }
}