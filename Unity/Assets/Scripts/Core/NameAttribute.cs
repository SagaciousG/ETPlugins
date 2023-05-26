using System;

namespace ET
{
    [AttributeUsage(AttributeTargets.All)]
    public class NameAttribute : Attribute
    {
        public string Name;
        public string Tooltips;
        public NameAttribute(string name, string tooltips = "")
        {
            this.Name = name;
            this.Tooltips = tooltips;
        }
    }
}