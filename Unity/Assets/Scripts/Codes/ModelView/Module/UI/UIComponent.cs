using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    /// <summary>
    /// 管理Scene上的UI
    /// </summary>
    [ComponentOf(typeof(Scene))]
    public class UIComponent: Entity, IAwake
    {
        public Dictionary<string, UI> UIs = new Dictionary<string, UI>();
        public Dictionary<string, Type> Name2Type = new Dictionary<string, Type>();
    }
}