﻿﻿using System;
using System.Reflection;
 
namespace Aspose.Hook.Share
{
    public static class Extensions
    {
        public static MethodInfo GetMethod(this string type, string method, BindingFlags flags = BindingFlags.Default,bool breakOnFind = true)
        {
            MethodInfo found = null;
            foreach (var mInfo in Type.GetType(type).GetMethods(flags))
            {
                if (mInfo.Name != method) continue;
 
                found = mInfo;
                if (breakOnFind)
                    break;
            }
 
            return found;
        }
        public static MethodInfo GetMethod(this string type, string method,Type[] parameterTypes, BindingFlags flags = BindingFlags.Default)
        {
            return Type.GetType(type).GetMethod(method, flags, null, parameterTypes, null);
        }
 
    }
}