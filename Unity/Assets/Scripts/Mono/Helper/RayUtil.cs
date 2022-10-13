using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace XGame
{
    public static class RayUtil
    {
        /// <summary>
        /// 和地面交点
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetPlaneInteractivePoint(Camera cam, Vector3 screenPos, float plane=0)
        {
            var ray = cam.ScreenPointToRay(screenPos);
            Vector3 dir = ray.direction;

            if (dir.y.Equals(0)) return Vector3.zero;
            float num = (plane - ray.origin.y) / dir.y;
            return ray.origin + ray.direction * num;
        }

        public static RayCastResult RayCast(Camera cam, Vector3 pos, int layerMask)
        {
            var ray = cam.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out var hitInfo, 1000, layerMask))
            {
                return new RayCastResult(){HasValue = true, HitInfo = hitInfo,
                    Obj = hitInfo.collider.gameObject};
            }
            return new RayCastResult(){HasValue = false};
        }
        
        public static bool IsOverUI()
        {
            if (EventSystem.current && EventSystem.current.IsPointerOverGameObject())
            {
                PointerEventData data = new(EventSystem.current);
                data.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                List<RaycastResult> raycastResult = new List<RaycastResult>();
                EventSystem.current.RaycastAll(data, raycastResult);
                if (raycastResult.Count > 0)
                {
                    foreach (var result in raycastResult)
                    {
                        var root = result.gameObject.GetComponentInParent<ReferenceCollector>();
                        if (root != null && !root.CompareTag("UIRaycastIgnore"))
                        {
                            return true;
                        }
                    }

                }
            }
            return false;
        }
        
    }
    
    public struct RayCastResult
    {
        public bool HasValue;
        public GameObject Obj;
        public RaycastHit HitInfo;
    }
}