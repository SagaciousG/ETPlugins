using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UI;

namespace XGame
{
    public class PSDSetting : ScriptableObject
    {
        public string PsdFolder;
        public string UIFolder;
        public Vector2 ScreenSize = new Vector2(1920, 1080);
        public Font DefaultFont;
        public List<string> ComponentTypes = new List<string>();

        public List<string> LatesdFile = new List<string>();
        public List<string> LockedFolder = new List<string>();

        public TreeViewState FileMenuState = new TreeViewState();
    }
}