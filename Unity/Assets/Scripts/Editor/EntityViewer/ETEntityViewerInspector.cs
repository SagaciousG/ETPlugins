using System;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public partial class ETEntityViewer
    {
        private class Inspector : AAreaBase
        {
            public Inspector(ETEntityViewer parent, Func<Rect> getPosition): base(parent, getPosition)
            {
            }

            public override void OnGUI()
            {
                if (_currentNode == null)
                {
                    return;
                }
                EditorGUILayout.TextField("InstanceId", _currentNode.Entity.InstanceId.ToString());
                EditorGUILayout.TextField("Id", _currentNode.Entity.Id.ToString());
                EditorGUILayout.TextField("Zone", _currentNode.Entity.DomainZone().ToString());
                
                TypeDrawHelper.BeginDraw(_currentNode.Entity);
            }
        }
    }
}