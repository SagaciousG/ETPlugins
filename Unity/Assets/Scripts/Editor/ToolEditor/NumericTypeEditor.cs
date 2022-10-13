using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ET;
using UnityEditor;
using UnityEngine;

namespace XGame
{
    public class NumericTypeEditor : EditorWindow
    {
        [MenuItem("Tools/NumericType")]
        static void ShowWin()
        {
            GetWindow<NumericTypeEditor>().Show();
        }
        private enum NumericPlusType
        {
            Base = 1,
            Add,
            Pct,
            FinalAdd,
            FinalPct,
        }

        private const string filePath = "Assets/Scripts/Codes/Model/Share/Module/Numeric/NumericType.cs";

        private List<TypeContent> _typeLines = new List<TypeContent>();

        private static Regex _typeMatch = new Regex(@"public const int (.*) *= *(\d+) *;");
        private static Regex _plusMatch = new Regex(@"public const int (.*) *=.*;");
        private static Regex _noteMatch = new Regex(@"//(.*)");

        private Vector2 _scroll1;
        private Vector2 _scroll2;
        private void OnEnable()
        {
            var lines = File.ReadAllLines(filePath).ToList();
            TypeContent typeContent = null;
            foreach (string line in lines)
            {
                if (_typeMatch.IsMatch(line))
                {
                    var matchName = _typeMatch.Match(line).Groups[1].Value.Trim();
                    if (matchName == "Max")
                        continue;
                    
                    var matchNum = Convert.ToInt32(_typeMatch.Match(line).Groups[2].Value.Trim());
                    
                    typeContent = new TypeContent();
                    typeContent.Name = matchName;
                    typeContent.Num = matchNum;
                    if (_noteMatch.IsMatch(line))
                    {
                        typeContent.Note = _noteMatch.Match(line).Groups[1].Value;
                    }
                    
                    this._typeLines.Add(typeContent);
                }
                else if (_plusMatch.IsMatch(line))
                {
                    var matchName = _plusMatch.Match(line).Groups[1].Value.Trim();
                    if (Enum.TryParse<NumericPlusType>(matchName.Replace(typeContent.Name, ""), out var value))
                    {
                        typeContent.PlusTypes[value.ToString()] = true;
                    }
                }
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("保存", "Tab onlyOne", GUILayout.Width(80), GUILayout.Height(20)))
            {
                var sb = new StringBuilder();
                sb.AppendLine("namespace ET");
                sb.AppendLine("{");
                sb.AppendLine("\tpublic static class NumericType");
                sb.AppendLine("\t{");
                sb.AppendLine("\t\tpublic const int Max = 10000;");
                foreach (TypeContent typeContent in this._typeLines)
                {
                    sb.AppendLine("\t\t");
                    var note = string.IsNullOrEmpty(typeContent.Note)? "" : $" //{typeContent.Note}";
                    sb.AppendLine($"\t\tpublic const int {typeContent.Name} = {typeContent.Num};{note}");
                    foreach (var kv in typeContent.PlusTypes)
                    {
                        if (kv.Value)
                        {
                            var val = (int)Enum.Parse<NumericPlusType>(kv.Key);
                            sb.AppendLine($"\t\tpublic const int {typeContent.Name}{kv.Key} = {typeContent.Name} * 10 + {val};");
                        }
                    }
                }
                sb.AppendLine("\t}");
                sb.AppendLine("}");
                File.WriteAllText(filePath, sb.ToString());
                AssetDatabase.Refresh();
            }
            
            if (GUILayout.Button("新建", "Tab onlyOne", GUILayout.Width(80), GUILayout.Height(20)))
            {
                var num = this._typeLines.Last()?.Num ?? 1000;
                this._typeLines.Add(new TypeContent(){Name = "__New", Num = num + 1});
            }
            
            if (GUILayout.Button("整理", "Tab onlyOne", GUILayout.Width(80), GUILayout.Height(20)))
            {
                for (int i = 0; i < this._typeLines.Count; i++)
                {
                    var content = this._typeLines[i];
                    content.Num = 1000 + i;
                }
            }
            
            EditorGUILayout.EndHorizontal();
            this._scroll1 = EditorGUILayout.BeginScrollView(this._scroll1);
            for (int i = 0; i < this._typeLines.Count; i++)
            {
                var content = this._typeLines[i];

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button($"{content.Name}    {content.Num}"))
                {
                    content.Show = !content.Show;
                }

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    this._typeLines.RemoveAt(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();

                if (content.Show)
                {
                    EditorGUILayout.BeginVertical("Box");
                    content.Name = EditorGUILayout.TextField("Name", content.Name);
                    content.Num = EditorGUILayout.IntField("Num", content.Num);
                    content.Note = EditorGUILayout.TextField("注释", content.Note);
                    foreach (string n in Enum.GetNames(typeof(NumericPlusType)))
                    {
                        content.PlusTypes.TryGetValue(n, out var value);
                        content.PlusTypes[n] = EditorGUILayout.Toggle(n, value);
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private class TypeContent
        {
            public string Name;
            public int Num;
            public bool Show;
            public string Note;
            public Dictionary<string, bool> PlusTypes = new Dictionary<string, bool>();
        }
    }
}