using XGame;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XGame
{
    public static class UIExtension
    {
        public static void Display(this UIBehaviour ui, bool show)
        {
            ui.gameObject.SetActive(show);
            // var cg = ui.transform.GetComponent<CanvasGroup>();
            // if (cg == null)
            //     cg = ui.gameObject.AddComponent<CanvasGroup>();
            // cg.alpha = show ? 1 : 0;
            // cg.interactable = show;
            // cg.blocksRaycasts = show;
        }
        
        public static void Display(this RectTransform ui, bool show)
        {
            ui.gameObject.SetActive(show);
            // var cg = ui.transform.GetComponent<CanvasGroup>();
            // if (cg == null)
            //     cg = ui.gameObject.AddComponent<CanvasGroup>();
            // cg.alpha = show ? 1 : 0;
            // cg.interactable = show;
            // cg.blocksRaycasts = show;
        }

        public static bool IsShow(this UIBehaviour ui)
        {
            return ui.gameObject.activeSelf;
            // var cg = ui.transform.GetComponent<CanvasGroup>();
            // if (cg == null)
            //     return true;
            // return cg.alpha > 0;
        }
        
        public static bool IsShow(this RectTransform ui)
        {
            return ui.gameObject.activeSelf;
            // var cg = ui.transform.GetComponent<CanvasGroup>();
            // if (cg == null)
            //     return true;
            // return cg.alpha > 0;
        }

        public static void Alpha(this UIBehaviour ui, float alpha)
        {
            var cg = ui.gameObject.AddComponentNotOwns<CanvasGroup>();
            cg.alpha = alpha;
        }
        
        public static void Alpha(this RectTransform ui, float alpha)
        {
            var cg = ui.gameObject.AddComponentNotOwns<CanvasGroup>();
            cg.alpha = alpha;
        }
        
        public static Vector2 GetPreferredWidthAndHeight(this Text textComponent, string content, int maxWidth = 0)
        {
            float pixelsPerUnit = textComponent.pixelsPerUnit;
            var settings = textComponent.GetGenerationSettings(Vector2.zero);
            float width = textComponent.cachedTextGeneratorForLayout.GetPreferredWidth(content, settings) / pixelsPerUnit;
            float height = 0;
            if (maxWidth == 0)
            {
                height = textComponent.cachedTextGeneratorForLayout.GetPreferredHeight(content,
                    textComponent.GetGenerationSettings(
                        new Vector2(textComponent.GetPixelAdjustedRect().size.x, 0.0f))) / pixelsPerUnit;
            }
            else
            {
                height = textComponent.cachedTextGeneratorForLayout.GetPreferredHeight(content, textComponent.GetGenerationSettings(new Vector2(maxWidth, 0.0f))) / pixelsPerUnit;
            }
            return new Vector2(width, height);
        }

        public static Rect GetRectTransformWorldRect(this RectTransform self, Camera cam)
        {
            Vector3[] corners = new Vector3[4];
            self.GetWorldCorners(corners);
            Vector2 v0 = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
            Vector2 v1 = RectTransformUtility.WorldToScreenPoint(cam, corners[2]);
            Rect rect = new Rect(v0, v1 - v0);
            return rect;
        }

        public static Rect InnerRectToScreenRect(this RectTransform self, Rect rect, Camera cam)
        {
            var min = self.TransformPoint(rect.min);
            var max = self.TransformPoint(rect.max);
            Vector2 v0 = RectTransformUtility.WorldToScreenPoint(cam, min);
            Vector2 v1 = RectTransformUtility.WorldToScreenPoint(cam, max);
            Rect res = new Rect(v0, v1 - v0);
            return res;
        }
    }
}