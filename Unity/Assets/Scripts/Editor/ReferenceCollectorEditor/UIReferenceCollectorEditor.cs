using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using XGame;
//Object并非C#基础中的Object，而是 UnityEngine.Object
using Object = UnityEngine.Object;

//自定义ReferenceCollector类在界面中的显示与功能
[CustomEditor(typeof (UIReferenceCollector))]
//没有该属性的编辑器在选中多个物体时会提示“Multi-object editing not supported”
[CanEditMultipleObjects]
public class UIReferenceCollectorEditor: Editor
{

	private UIReferenceCollector referenceCollector;
	
	private void DelNullReference()
	{
		var dataProperty = serializedObject.FindProperty("data");
		for (int i = dataProperty.arraySize - 1; i >= 0; i--)
		{
			var gameObjectProperty = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("gameObject");
			if (gameObjectProperty.objectReferenceValue == null)
			{
				dataProperty.DeleteArrayElementAtIndex(i);
			}
		}
	}

	private void OnEnable()
	{
		//将被选中的gameobject所挂载的ReferenceCollector赋值给编辑器类中的ReferenceCollector，方便操作
		referenceCollector = (UIReferenceCollector) target;
	}

	public override void OnInspectorGUI()
	{
		//使ReferenceCollector支持撤销操作，还有Redo，不过没有在这里使用
		Undo.RecordObject(referenceCollector, "Changed Settings");
		var dataList = referenceCollector.data;
		//开始水平布局，如果是比较新版本学习U3D的，可能不知道这东西，这个是老GUI系统的知识，除了用在编辑器里，还可以用在生成的游戏中
		GUILayout.BeginHorizontal();
		//下面几个if都是点击按钮就会返回true调用里面的东西
		if (GUILayout.Button("自动引用"))
		{
			this.AutoFind();
		}
		if (GUILayout.Button("全部删除"))
		{
			referenceCollector.data.Clear();
		}
		if (GUILayout.Button("删除空引用"))
		{
			DelNullReference();
		}
		if (GUILayout.Button("排序"))
		{ 
			referenceCollector.data.Sort((x, y) => string.Compare(x.key, y.key, StringComparison.Ordinal));
			EditorUtility.SetDirty(this);
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(this.referenceCollector.gameObject);
		if (string.IsNullOrEmpty(path))
			GUI.enabled = false;
		if (GUILayout.Button("生成脚本"))
		{
			this.GenerateScript();	
		}
		GUI.enabled = true;

		if (GUILayout.Button("复制引用代码"))
		{
			var sb = new StringBuilder();
			foreach (UIReferenceCollectorData data in this.referenceCollector.data)
			{
				sb.AppendLine($"var {data.key} = referenceCollector.GetComponentFromGO<{data.Components[data.SelectIndex]}>(\"{data.key}\");");
			}
			TextEditor t = new TextEditor();
			t.text = sb.ToString();
			t.OnFocus();
			t.Copy();
		}

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.LabelField("拖入文件或对象到此处添加引用");
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		var toggleWidth = 30;
		var nameWidth = 100;
		var objRefWidth = 100;
		var removeWidth = 30;
		var style = new GUIStyle("AM HeaderStyle");
		style.alignment = TextAnchor.MiddleCenter;
		EditorGUILayout.BeginHorizontal();
		// EditorGUILayout.LabelField("Click", style, GUILayout.Width(toggleWidth));
		EditorGUILayout.LabelField("Field", style, GUILayout.Width(nameWidth));
		EditorGUILayout.LabelField("Ref", style, GUILayout.Width(objRefWidth));
		EditorGUILayout.LabelField("Component", style);
		EditorGUILayout.LabelField("", GUILayout.Width(removeWidth));
		EditorGUILayout.EndHorizontal();
		
		var delList = new List<int>();
		SerializedProperty property;
		//遍历ReferenceCollector中data list的所有元素，显示在编辑器中
		for (int i = referenceCollector.data.Count - 1; i >= 0; i--)
		{
			var data = referenceCollector.data[i];
			
			GUILayout.BeginHorizontal();
			//这里的知识点在ReferenceCollector中有说
			// data.addClick = EditorGUILayout.Toggle(data.addClick, GUILayout.Width(toggleWidth));
			data.key = EditorGUILayout.TextField(data.key, GUILayout.Width(nameWidth));
			var obj = EditorGUILayout.ObjectField(data.gameObject, typeof(UnityEngine.Object), true, GUILayout.Width(objRefWidth));
			if (!obj.Equals(data.gameObject))
			{
				delList.Add(i);
				AddReference(referenceCollector.data, obj.name, obj);
			}
			data.SelectIndex = EditorGUILayout.Popup(data.SelectIndex, data.Components.ToArray());
			if (GUILayout.Button("X", GUILayout.Width(removeWidth)))
			{
				//将元素添加进删除list
				delList.Add(i);
			}
			GUILayout.EndHorizontal();
		}
		var eventType = Event.current.type;
		//在Inspector 窗口上创建区域，向区域拖拽资源对象，获取到拖拽到区域的对象
		if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
		{
			// Show a copy icon on the drag
			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

			if (eventType == EventType.DragPerform)
			{
				DragAndDrop.AcceptDrag();
				foreach (var o in DragAndDrop.objectReferences)
				{
					AddReference(referenceCollector.data, o.name, o);
				}
			}

			Event.current.Use();
		}

		//遍历删除list，将其删除掉
		foreach (var i in delList)
		{
			referenceCollector.data.RemoveAt(i);
		}

		if (GUI.changed)
		{
			serializedObject.ApplyModifiedProperties();
			serializedObject.UpdateIfRequiredOrScript();
			EditorUtility.SetDirty(this.target);
		}
	}

	//添加元素，具体知识点在ReferenceCollector中说了
	private void AddReference(List<UIReferenceCollectorData> collector, string key, Object obj)
	{
		var element = new UIReferenceCollectorData();
		element.key = key;
		element.gameObject = obj;
		var arr = element.Components;
		switch (obj.GetType().Name)
		{
			case "GameObject":
				element.IsGameObject = true;
				var go = (GameObject) obj;
				var com = go.GetComponents(typeof(Component));
				foreach (var component in com)
				{
					var type = component.GetType();
					arr.Add(type.Name);
					element.ComponentType.Add(type.FullName.Replace($".{type.Name}", ""));
				}
				break;
			default:
				arr.Add(obj.GetType().Name);
				break;
		}

		element.SelectIndex = GetBestChoice(element.Components.ToArray());
		collector.Add(element);
		
	}

	private int GetBestChoice(string[] types)
	{
		var setting = AssetDatabase.LoadAssetAtPath<PSDSetting>("Assets/PsdSetting.asset");
		var dic = setting.ComponentTypes;
		var min = 0;
		var minIndex = 0;
		for (int i = 0; i < types.Length; i++)
		{
			var index = dic.IndexOf(types[i]);
			if (index > min)
			{
				minIndex = i;
				min = index;
			}
		}

		return minIndex;
	}
	
	private void AutoFind()
	{
		referenceCollector.data.Clear();
		FindInChild(referenceCollector.transform);
	}

	private void FindInChild(Transform node)
	{
		var count = node.childCount;
		for (int i = 0; i < count; i++)
		{
			var child = node.GetChild(i);
			if (child.GetComponent<UIReferenceCollector>() != null)
				continue;
			if (Regex.IsMatch(child.name, @"^[a-z].*"))
			{
				AddReference(referenceCollector.data, child.name, child.gameObject);
			}
			FindInChild(child);
		}
	}

	private void GenerateScript()
	{
		var prefab = this.referenceCollector.gameObject;
		var assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefab);

		if (string.IsNullOrEmpty(assetPath))
			return;
		var cutPath = assetPath.Substring("Assets/Bundles/UI/".Length);
		cutPath = cutPath.Replace($"/{this.referenceCollector.name}.prefab", "");
		var scriptPath = "Assets/Scripts/Codes/ModelView/UI";
		var systemPath = "Assets/Scripts/Codes/HotfixView/UI";

		var useSB = new StringBuilder();
		var fieldSB = new StringBuilder();
		var initSB = new StringBuilder();
		//----生成Component
		useSB.AppendLine("using UnityEngine.UI;");
		useSB.AppendLine("using UnityEngine;");
		useSB.AppendLine("using XGame;");
		HashSet<string> _usedType = new HashSet<string>();
		foreach (var data in this.referenceCollector.data)
		{
			var typeName = data.ComponentType[data.SelectIndex];
			if (typeName == "UnityEngine"
			    || typeName == "UnityEngine.UI"
			    || typeName == "XGame"
			    )
				continue;
			if (_usedType.Contains(typeName))
				continue;
			useSB.AppendLine($"using {typeName};");
			_usedType.Add(typeName);
		}

		foreach (var data in this.referenceCollector.data)
		{
			fieldSB.AppendLine($"\t\tpublic {data.Components[data.SelectIndex]} {data.key};");
		}
		
		var componentTemp = File.ReadAllText("Assets/Scripts/Editor/ReferenceCollectorEditor/ComponentTemplete.txt");
		componentTemp = componentTemp.Replace("[USING]", useSB.ToString());
		componentTemp = componentTemp.Replace("[NAME]", this.referenceCollector.name);
		componentTemp = componentTemp.Replace("[FIELDS]", fieldSB.ToString());
		
		if (!Directory.Exists($"{scriptPath}Gen/{cutPath}"))
			Directory.CreateDirectory($"{scriptPath}Gen/{cutPath}");
		File.WriteAllText($"{scriptPath}Gen/{cutPath}/{this.referenceCollector.name}Component.cs", componentTemp);

		if (!File.Exists($"{scriptPath}/{cutPath}/{this.referenceCollector.name}ComponentEx.cs"))
		{
			var componentEx = File.ReadAllText("Assets/Scripts/Editor/ReferenceCollectorEditor/ComponentEx.txt");
			componentEx = componentEx.Replace("[NAME]", this.referenceCollector.name);
			if (!Directory.Exists($"{scriptPath}/{cutPath}"))
				Directory.CreateDirectory($"{scriptPath}/{cutPath}");
			File.WriteAllText($"{scriptPath}/{cutPath}/{this.referenceCollector.name}ComponentEx.cs", componentEx);
		}

		
		//System---------------
		foreach (var data in this.referenceCollector.data)
		{
			initSB.AppendLine($"\t\t\t\tself.{data.key} = rc.GetComponentFromGO<{data.Components[data.SelectIndex]}>(\"{data.key}\");");
		}
		
		var systemTemp = File.ReadAllText("Assets/Scripts/Editor/ReferenceCollectorEditor/SystemTemplete.txt");
		systemTemp = systemTemp.Replace("[USING]", useSB.ToString());
		systemTemp = systemTemp.Replace("[NAME]", this.referenceCollector.name);
		systemTemp = systemTemp.Replace("[INIT]", initSB.ToString());
		if (!Directory.Exists($"{systemPath}Gen/{cutPath}"))
			Directory.CreateDirectory($"{systemPath}Gen/{cutPath}");
		File.WriteAllText($"{systemPath}Gen/{cutPath}/{this.referenceCollector.name}ComponentSystem.cs", systemTemp);
		
		if (!File.Exists($"{systemPath}/{cutPath}/{this.referenceCollector.name}ComponentSystemEx.cs"))
		{
			var systemTempEx = File.ReadAllText("Assets/Scripts/Editor/ReferenceCollectorEditor/SystemExTemplete.txt");
			systemTempEx = systemTempEx.Replace("[NAME]", this.referenceCollector.name);
			if (!Directory.Exists($"{systemPath}/{cutPath}"))
				Directory.CreateDirectory($"{systemPath}/{cutPath}");
			File.WriteAllText($"{systemPath}/{cutPath}/{this.referenceCollector.name}ComponentSystemEx.cs", systemTempEx);
		}
		
		//Event---------------
		var eventTemp = File.ReadAllText("Assets/Scripts/Editor/ReferenceCollectorEditor/EventTemplete.txt");
		eventTemp = eventTemp.Replace("[NAME]", this.referenceCollector.name);
		if (!Directory.Exists($"{systemPath}Gen/{cutPath}"))
			Directory.CreateDirectory($"{systemPath}Gen/{cutPath}");
		File.WriteAllText($"{systemPath}Gen/{cutPath}/{this.referenceCollector.name}Event.cs", eventTemp);


		var uiType = "Assets/Scripts/Codes/ModelView/Client/Module/UI/UIType.cs";
		var fields = new StringBuilder();
		fields.AppendLine("namespace ET.Client");
		fields.AppendLine("{");
		fields.AppendLine("\tpublic static class UIType");
		fields.AppendLine("\t{");
		var existKey = new HashSet<string>();
		if (File.Exists(uiType))
		{
			var regex = new Regex(@"public const string (\w*) = .*;");
			var lines = File.ReadAllLines(uiType);
			foreach (string line in lines)
			{
				if (regex.IsMatch(line))
				{
					var res = regex.Match(line);
					existKey.Add(res.Groups[1].Value);
					fields.AppendLine(line);
				}
			}
		}

		if (!existKey.Contains(this.referenceCollector.name))
		{
			fields.AppendLine($"\t\tpublic const string {this.referenceCollector.name} = \"{this.referenceCollector.name}\";");
			
		}

		fields.AppendLine("\t}");
		fields.AppendLine("}");
		File.WriteAllText(uiType, fields.ToString());
		
		
		AssetDatabase.Refresh();
	}
	
}
