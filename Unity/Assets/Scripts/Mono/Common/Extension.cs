using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace XGame
{
    public static class Extension
    {
        
        /// <summary>
        /// 计算一个Vector3绕指定轴旋转指定角度后所得到的向量。
        /// </summary>
        /// <param name="source">旋转前的源Vector3</param>
        /// <param name="axis">旋转轴</param>
        /// <param name="angle">旋转角度</param>
        /// <returns>旋转后得到的新Vector3</returns>
        public static Vector3 Rotate(this Vector3 source, Vector3 axis, float angle)
        {
            Quaternion q = Quaternion.AngleAxis(angle, axis); // 旋转系数
            return q * source; // 返回目标点
        }
        
        /// <summary>
        /// 替换字符串末尾位置中指定的字符串
        /// </summary>
        /// <param name="s">源串</param>
        /// <param name="searchStr">查找的串</param>
        public static string TrimEndString(this string s, string searchStr)
        {
            var result = s;
            try
            {
                if (string.IsNullOrEmpty(result))
                {
                    return result;
                }
                if (s.Length < searchStr.Length)
                {
                    return result;
                }
                if (s.IndexOf(searchStr, s.Length - searchStr.Length, searchStr.Length, StringComparison.Ordinal) > -1)
                {
                    result = s.Substring(0, s.Length - searchStr.Length);
                }
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }
        
        /// <summary>
        /// 替换字符串起始位置(开头)中指定的字符串
        /// </summary>
        /// <param name="s">源串</param>
        /// <param name="searchStr">查找的串</param>
        /// <returns></returns>
        public static string TrimStartString(this string s, string searchStr)
        {
            var result = s;
            try
            {
                if (string.IsNullOrEmpty(result))
                {
                    return result;
                }
                if (s.Length < searchStr.Length)
                {
                    return result;
                }
                if (s.IndexOf(searchStr, 0, searchStr.Length, StringComparison.Ordinal) > -1)
                {
                    result = s.Substring(searchStr.Length);
                }
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }

        public static bool Contains(this Rect rect, Rect r)
        {
            return r.xMax <= rect.xMax
                   && r.yMax <= rect.yMax
                   && r.xMin >= rect.xMin
                   && r.yMin >= rect.yMin;
        }
        
        public static void AddRange<T1, T2>(this Dictionary<T1, T2> map, Dictionary<T1, T2> values)
        {
            foreach (var kv in values)
            {
                map.Add(kv.Key, kv.Value);
            }
        }

        public static T AddComponentNotOwns<T>(this GameObject obj) where T : Component
        {
            if (obj.GetComponent<T>() == null)
                return obj.AddComponent<T>();
            return obj.GetComponent<T>();
        }

        public static bool ContainKey<T>(this List<T> list, int key)
        {
            if (key < 0 || list.Count < key)
                return false;
            return true;
        }

        public static Transform[] FindAllChildren(this GameObject obj)
        {
            var list = new List<Transform>();
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                var child = obj.transform.GetChild(i);
                var children = child.gameObject.FindAllChildren();
                list.Add(child);
                list.AddRange(children);
            }

            return list.ToArray();
        }

        public static Rect Rect(this Bounds box)
        {
            var r = new Rect(Vector2.zero, new Vector2(box.size.x, box.size.z));
            r.center = new Vector2(box.center.x, box.center.z);
            return r;
        }
        
        public static void Display(this GameObject obj, bool show)
        {
            obj.SetActive(show);
            // if (obj.GetComponent<RectTransform>() != null)
            // {
            //     var cg = obj.GetComponent<CanvasGroup>();
            //     if (cg == null)
            //         cg = obj.AddComponent<CanvasGroup>();
            //     cg.alpha = show ? 1 : 0;
            //     cg.interactable = show;
            //     cg.blocksRaycasts = show;
            // }
            // else
            // {
            //     obj.SetActive(show);
            // }
        }

        public static bool IsShow(this GameObject obj)
        {
            return obj.activeSelf;
            // if (obj.GetComponent<RectTransform>() != null)
            // {
            //     var cg = obj.GetComponent<CanvasGroup>();
            //     if (cg == null)
            //         return true;
            //     return cg.alpha > 0;
            // }
            // else
            // {
            //     return obj.activeSelf;
            // }
        }
        
        public static T Last<T>(this T[] self)
        {
            return self[self.Length - 1];
        }

        public static T Last<T>(this List<T> self)
        {
            if (self.Count > 0)
                return self[self.Count - 1];
            return default;
        }

        
        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToInitialUpper(this string str)
        {
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }
        
        public static string PathSymbolFormat(this string pathStr)
        {
            return pathStr.Replace("\\", "/");
        }

        public static string ToUnityPath(this string path)
        {
            return path.Replace("\\", "/").Replace($"{Application.dataPath}", "Assets");
        }

        public static Transform FindDepth(this Transform self, string name)
        {
            for (int i = 0; i < self.childCount; i++)
            {
                var trans = self.GetChild(i);
                if (trans.name == name)
                    return trans;
                var res = trans.FindDepth(name);
                if (res != null)
                    return res;
            }

            return null;
        }

        public static void OnClick(this Graphic graphic, Action action)
        {
            var btn = graphic.gameObject.AddComponentNotOwns<UIClickListener>();
            btn.AddClick(action);
        }
        
        public static void OnClick<T>(this Graphic graphic, Action<T> action, T argc)
        {
            var btn = graphic.gameObject.AddComponentNotOwns<UIClickListener>();
            btn.AddClick(action, argc);
        }
        
        public static void OnClick(this GameObject obj, Action action)
        {
            var graphic = obj.GetComponent<Graphic>();
            if (graphic == null)
                graphic = obj.AddComponent<EmptyGraphic>();
            var btn = graphic.gameObject.AddComponentNotOwns<UIClickListener>();
            btn.AddClick(action);
        }
        
        public static void OnClick<T>(this GameObject obj, Action<T> action, T argc)
        {
            var graphic = obj.GetComponent<Graphic>();
            if (graphic == null)
                graphic = obj.AddComponent<EmptyGraphic>();
            var btn = graphic.gameObject.AddComponentNotOwns<UIClickListener>();
            btn.AddClick(action, argc);
        }

        public static void OffClick(this Graphic graphic, Action action)
        {
            var btn = graphic.gameObject.AddComponentNotOwns<UIClickListener>();
            btn.RemoveClick(action);
        }
        
        public static void OffClick<T>(this Graphic graphic, Action<T> action)
        {
            var btn = graphic.gameObject.AddComponentNotOwns<UIClickListener>();
            btn.RemoveClick(action);
        }

        public static float Normalize(this float num)
        {
            if (num == 0)
                return 0;
            return num / Mathf.Abs(num);
        }
        
        public static int NormalizeToInt(this float num)
        {
            if (num == 0)
                return 0;
            return Mathf.RoundToInt(num / Mathf.Abs(num));
        }

        public static Transform[] Children(this Transform transform)
        {
            var trs = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                trs[i] = transform.GetChild(i);
            }

            return trs;
        }
        
        public static bool TryMatchOne<T>(this IEnumerable<T> itor, Func<T, bool> compare, out T result)
        {
            foreach (var item in itor)
            {
                if (compare.Invoke(item))
                {
                    result = item;
                    return true;
                }
            }

            result = default;
            return false;
        }

        public static T FindComponentFromRootObjs<T>(this Scene scene, string name = null) where T : MonoBehaviour
        {
            foreach (var o in scene.GetRootGameObjects())
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (o.name != name)
                        continue;
                }

                if (o.GetComponent<T>() != null)
                    return o.GetComponent<T>();
            }

            return null;
        }

        public static string[] ToStringArray<T>(this IEnumerable<T> itor)
        {
            var list = new List<string>();
            foreach (var obj in itor)
            {
                list.Add(obj.ToString());
            }

            return list.ToArray();
        }

        public static Rect CloseIn(this Rect rect, Rect containIn)
        {
            var x = Mathf.Clamp(rect.center.x, containIn.xMin + rect.width / 2, containIn.xMax - rect.width / 2);
            var y = Mathf.Clamp(rect.center.y, containIn.yMin + rect.height / 2, containIn.yMax - rect.height / 2);
            rect.center = new Vector2(x, y);
            return rect;
        }

        public static int Symbol(this int num)
        {
            return num / Mathf.Abs(num);
        }
        
        public static void ClearChildren(this Transform self)
        {
            for (int i = self.childCount - 1; i >= 0; i--)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(self.GetChild(i).gameObject);
                }
                else
                {
                    Object.DestroyImmediate(self.GetChild(i).gameObject);
                }
            }
        }

        public static string[] ToStringArray(this IEnumerable list)
        {
            var res = new List<string>();
            foreach (object o in list)
            {
                res.Add(o.ToString());
            }

            return res.ToArray();
        }
    }
}