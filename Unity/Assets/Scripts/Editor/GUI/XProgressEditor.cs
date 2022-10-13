using System;
using Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XGame;

namespace Model
{
    [CustomEditor(typeof(XProgress))]
    public class XProgressEditor : Editor
    {
        private SerializedObject _sliderObj;
        private Editor _sliderEditor;
        private bool _editorShow;
        private void OnEnable()
        {
            var progress = (XProgress) target;
            progress.Slider ??= progress.GetComponent<XImage>();
            if (progress.Slider != null)
            {
                progress.Value = progress.Slider.fillAmount;
                progress.Slider.type = Image.Type.Filled;
                _sliderObj = new SerializedObject(progress.Slider);
                _sliderEditor = CreateEditor(_sliderObj.targetObject);
            }
        }

        public override void OnInspectorGUI()
        {
            var progress = (XProgress) target;
            progress.Slider = (XImage) EditorGUILayout.ObjectField("Fill Image", progress.Slider, typeof(XImage), true);
            if (progress.Slider == null)
            {
                EditorGUILayout.HelpBox("Slider is Empty", MessageType.Error);
            }
            else
            {
                _editorShow = EditorGUILayout.BeginFoldoutHeaderGroup(_editorShow, "Fill Image Property", "box");
                if (_editorShow)
                {
                    EditorGUILayout.BeginVertical("box");
                    _sliderEditor.OnInspectorGUI();
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
        
    }
}