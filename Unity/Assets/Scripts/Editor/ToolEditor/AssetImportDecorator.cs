using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace XGame
{
    public class AssetImportDecorator : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            if (textureImporter.assetPath.StartsWith("Assets/Bundles/Image"))
            {
                textureImporter.textureType = TextureImporterType.Sprite; //1-自动设置类型 
            
                TextureImporterPlatformSettings settingAndroid = textureImporter.GetPlatformTextureSettings("Android"); //new TextureImporterPlatformSettings();
                settingAndroid.overridden = true;
                if (textureImporter.DoesSourceTextureHaveAlpha())
                    settingAndroid.format = TextureImporterFormat.ASTC_6x6;
                else
                    settingAndroid.format = TextureImporterFormat.ASTC_6x6;
                textureImporter.SetPlatformTextureSettings(settingAndroid);

                TextureImporterPlatformSettings settingIos = textureImporter.GetPlatformTextureSettings("iPhone"); //new TextureImporterPlatformSettings();
                settingIos.overridden = true;
                if (textureImporter.DoesSourceTextureHaveAlpha())
                    settingIos.format = TextureImporterFormat.ASTC_6x6;
                else
                    settingIos.format = TextureImporterFormat.ASTC_6x6;
                textureImporter.SetPlatformTextureSettings(settingIos);


                textureImporter.mipmapEnabled = false;
                textureImporter.isReadable = false;
            }
        }
    }
}