using System;

namespace ET
{
    public class DefaultStructDrawer : ITypeDrawer
    {
        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            TypeDrawHelper.DrawObject(value, memberName);
            return value;
        }
    }
}