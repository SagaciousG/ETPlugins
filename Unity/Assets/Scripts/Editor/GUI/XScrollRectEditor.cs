using Model;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using XGame;

namespace Model
{
    [CustomEditor(typeof(XScrollRect))]
    public class XScrollRectEditor : ScrollRectEditor
    {
        private SerializedProperty _content;
        private SerializedProperty _viewport;

        protected override void OnEnable()
        {
            _content = serializedObject.FindProperty("m_Content");
            _viewport = serializedObject.FindProperty("m_Viewport");

        
            var scrollRect = (XScrollRect) target;
        
            if (_viewport.objectReferenceValue == null)
            {
                _viewport.objectReferenceValue = scrollRect.transform.FindDepth("Viewport")?.GetComponent<RectTransform>();
            }

            if (_content.objectReferenceValue == null)
            {
                _content.objectReferenceValue = scrollRect.transform.FindDepth("Content")?.GetComponent<RectTransform>();
            }
            
            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
            base.OnEnable();
        }
    }
}