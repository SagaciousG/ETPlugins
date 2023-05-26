using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    public class ETEntityViewerSetting : ScriptableObject
    {
        public Color SceneTypeColor = Color.green;
        public Color ComponentColor = Color.white;
        public Color ChildColor = Color.white;

        public List<string> QuickFlags;
    }
}