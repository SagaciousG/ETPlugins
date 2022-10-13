using System;
using UnityEditor;
using UnityEngine;
using XGame;

[CustomEditor(typeof (UIList))]
public class UIListEditor: UnityEditor.Editor
{
    private SerializedProperty _padding;
    private SerializedProperty _content;
    private SerializedProperty _viewport;
    private SerializedProperty _templete;
    private SerializedProperty _align;
    private SerializedProperty _alignNum;
    private SerializedProperty _spaceX;
    private SerializedProperty _spaceY;
    private SerializedProperty _layout;
    private SerializedProperty _autoCenter;

    private int _demoNum = 10;
    
    private void OnEnable()
    {
        var list = (UIList) target;
        _padding = serializedObject.FindProperty("_padding");
        _content = serializedObject.FindProperty("_content");
        _viewport = serializedObject.FindProperty("_viewport");
        _templete = serializedObject.FindProperty("_templete");
        _align = serializedObject.FindProperty("_align");
        _alignNum = serializedObject.FindProperty("_alignNum");
        _spaceX = serializedObject.FindProperty("_spaceX");
        _spaceY = serializedObject.FindProperty("_spaceY");
        _layout = serializedObject.FindProperty("_layout");
        _autoCenter = serializedObject.FindProperty("_autoCenter");

        
        
        if (_viewport.objectReferenceValue == null)
        {
            _viewport.objectReferenceValue = list.transform.FindDepth("Viewport")?.GetComponent<RectTransform>();
        }

        if (_content.objectReferenceValue == null)
        {
            _content.objectReferenceValue = list.transform.FindDepth("Content")?.GetComponent<RectTransform>();
        }
        
        // if (_templete.objectReferenceValue == null)
        // {
        //     _templete.objectReferenceValue = list.transform.GetComponentInChildren<UIListCellRender>();
        // }

        if (serializedObject.hasModifiedProperties)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }


    public override void OnInspectorGUI()
    {
        var list = (UIList) target;
        this._demoNum = EditorGUILayout.IntField("DemoCount", this._demoNum);
        EditorGUILayout.LabelField("DataNum", list.DataNum.ToString());
        EditorGUILayout.PropertyField(_autoCenter);
        EditorGUILayout.PropertyField(_content);
        EditorGUILayout.PropertyField(_viewport);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(_templete, new GUIContent("CellRender"));
        if (EditorGUI.EndChangeCheck())
        {
            // var isPrefab = PrefabUtility.IsPartOfPrefabAsset(_templete.objectReferenceValue);
            // if (isPrefab)
            // {
            //     var obj = (UIListCellRender)PrefabUtility.InstantiatePrefab(_templete.objectReferenceValue);
            //     obj.transform.SetParent(list.transform, false);
            //     _templete.objectReferenceValue = obj;
            //     obj.transform.SetAsFirstSibling();
            // }
        }
        
        EditorGUILayout.PropertyField(_padding);
        EditorGUILayout.PropertyField(_layout);

        switch (list.Layout)
        {
            case UIList.ListLayout.Grid:
                EditorGUILayout.PropertyField(_align);
                switch (_align.enumValueIndex)
                {
                    case 0:
                        EditorGUILayout.PropertyField(_alignNum, new GUIContent("ColCount"));
                        break;
                    case 1:
                        EditorGUILayout.PropertyField(_alignNum, new GUIContent("RawCount"));
                        break;
                }

                EditorGUILayout.PropertyField(_spaceX);
                EditorGUILayout.PropertyField(_spaceY);
                break;
            case UIList.ListLayout.Horizontal:
                EditorGUILayout.PropertyField(_spaceX);
                break;
            case UIList.ListLayout.Vertical:
                EditorGUILayout.PropertyField(_spaceY);
                break;
        }

        if (serializedObject.hasModifiedProperties)
        {
            serializedObject.ApplyModifiedProperties();
            list.UpdateLayout();
        }
    }
    
    private void OnSceneGUI()
    {
        var list = (UIList) target;
        if (list.Content == null)
            return;
        if (list.RenderCell == null)
            return;
        var size = list.RenderCell.sizeDelta;
        var pos = list.Content.position;
        Handles.color = Color.green;
        var size3d = new Vector3(size.x / 108, size.y / 108, 0.01f);
        var startPos = new Vector3(pos.x + size3d.x / 2, pos.y - size3d.y / 2, 0);
        for (int i = 0; i < this._demoNum; i++)
        {
            var col = i % list.AlignNum;
            var raw = Mathf.FloorToInt(i * 1f / list.AlignNum);
            var realPos = startPos + new Vector3(col * size3d.x + col * list.SpaceX / 108, -1 * raw * size3d.y - raw * list.SpaceY / 108, 0);
            Handles.DrawWireCube(realPos, size3d);
        }
    }
}
