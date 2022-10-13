using System;

namespace ET
{
    public class TypeDrawerAttribute: Attribute
    {
        public Type DrawerType;
        public bool InheritInclude;
        public bool IsInterface => DrawerType.IsInterface;
        
        public TypeDrawerAttribute(Type target, bool inheritInclude = false)
        {
            DrawerType = target;
            InheritInclude = inheritInclude;
        }
    }
}