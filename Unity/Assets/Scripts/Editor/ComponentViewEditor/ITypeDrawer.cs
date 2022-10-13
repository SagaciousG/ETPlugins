using System;

namespace ET
{
    public interface ITypeDrawer
    {
        object DrawAndGetNewValue(Type memberType, string memberName, object value, object target);
    }
}