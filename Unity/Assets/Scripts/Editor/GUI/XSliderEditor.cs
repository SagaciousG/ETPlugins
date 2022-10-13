using UnityEditor;
using UnityEditor.UI;
using XGame;

namespace Model
{
    [CustomEditor(typeof(XSlider))]
    public class XSliderEditor : SliderEditor
    {
        private SerializedProperty _textField;

        protected override void OnEnable()
        {
            base.OnEnable();
            _textField = serializedObject.FindProperty("_sliderText");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_textField);
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}