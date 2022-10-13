using UnityEngine;

namespace XGame
{
    public abstract class UIComponent : MonoBehaviour
    {
        public abstract void OnUIShow();
        public abstract void OnUIClose();
        public abstract void OnUIDispose();
    }
}