using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace XGame
{
    public static class PSDUtility
    {
        public static PSDSetting PSDSetting
        {
            get
            {
                if (_psdSetting == null)
                {
                    if (File.Exists("Assets/PsdSetting.asset"))
                    {
                        _psdSetting = AssetDatabase.LoadAssetAtPath<PSDSetting>("Assets/PsdSetting.asset");
                    }
                    else
                    {
                        _psdSetting = ScriptableObject.CreateInstance<PSDSetting>();
                        AssetDatabase.CreateAsset(PSDSetting, "Assets/PsdSetting.asset");
                    }
                }

                return _psdSetting;
            }
        }


        public static List<PSDInfo> OpenedPSD = new List<PSDInfo>();
        public static PSDSelectedLayer SelectedLayer;
        public static PSDPngViewWindow.RuntimeInfo RuntimeInfo = new PSDPngViewWindow.RuntimeInfo();
        
        private static PSDSetting _psdSetting;
        
        public static void SaveSetting()
        {
            EditorUtility.SetDirty(_psdSetting);
        }
        
    }

    public class PSDSelectedLayer
    {
        public readonly bool IsPSD;
        public readonly IPSDLayer PSDLayer;
        public readonly GameObject Object;

        public PSDSelectedLayer(IPSDLayer psdLayer)
        {
            IsPSD = true;
            PSDLayer = psdLayer;
        }

        public PSDSelectedLayer(GameObject obj)
        {
            IsPSD = false;
            Object = obj;
        }
    }
}