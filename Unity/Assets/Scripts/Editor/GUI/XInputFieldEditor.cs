using Model;
using UnityEditor;
using UnityEditor.UI;
using XGame;

namespace Model
{
    [CustomEditor(typeof(XInputField))]
    public class XInputFieldEditor : InputFieldEditor
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var input = (XInputField) target;
            if (input.textComponent != null)
            {
                input.textComponent.supportRichText = false;
            }
        }
    }
}