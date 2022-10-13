﻿﻿using System;
using System.IO;
using System.Reflection;
using Aspose.Hook.Share;
 
namespace Aspose.Hook
{
    public static class Manager
    {
        private static DotNetHook mCompareHook;
        private static DotNetHook mGreaterThanHook;
 
        private const string DATE_COMPARE_METHOD = "op_GreaterThan";
        private const string STRING_COMPARE_METHOD = "Compare";
 
        /// <summary>
        /// 启用hook
        /// </summary>
        public static void StartHook()
        {
            //string compare hook
            if (mCompareHook == null)
            {
                MethodBase stringCompareMethod = typeof(string).FullName.GetMethod(STRING_COMPARE_METHOD, new[] { typeof(string), typeof(string) }, BindingFlags.Static | BindingFlags.Public);
                MethodBase stringCompareMethodNew = typeof(Manager).FullName.GetMethod(nameof(NewCompare), BindingFlags.Static | BindingFlags.NonPublic);
                mCompareHook = new DotNetHook(stringCompareMethod, stringCompareMethodNew);
            }
            mCompareHook.Apply();
 
            //date compare hook
            if (mGreaterThanHook == null)
            {
                MethodBase dateCompareMethod = typeof(DateTime).FullName.GetMethod(DATE_COMPARE_METHOD, new[] { typeof(DateTime), typeof(DateTime) }, BindingFlags.Static | BindingFlags.Public);
                MethodBase dateCompareMethodNew = typeof(Manager).FullName.GetMethod(nameof(NewGreaterThan), BindingFlags.Static | BindingFlags.NonPublic);
                mGreaterThanHook = new DotNetHook(dateCompareMethod, dateCompareMethodNew);
            }
            mGreaterThanHook.Apply();
 
            //search all referenced Aspose assemblies. 
            var assemblies = Assembly.GetCallingAssembly().GetReferencedAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly.Name.StartsWith("Aspose."))
                {
                    var type = Assembly.Load(assembly).GetType(assembly.Name + ".License");
                    if (type == null)
                    {
                        type = Assembly.Load(assembly).GetType(System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(assembly.Name.ToLower()) + ".License");
                    }
                    if (type != null)
                    {
                        var instance = Activator.CreateInstance(type);
                        type.GetMethod("SetLicense", new[] { typeof(Stream) }).Invoke(instance, new[] { new MemoryStream(Convert.FromBase64String("PExpY2Vuc2U+CiAgPERhdGE+CiAgICA8TGljZW5zZWRUbz5TdXpob3UgQXVuYm94IFNvZnR3YXJlIENvLiwgTHRkLjwvTGljZW5zZWRUbz4KICAgIDxFbWFpbFRvPnNhbGVzQGF1bnRlYy5jb208L0VtYWlsVG8+CiAgICA8TGljZW5zZVR5cGU+RGV2ZWxvcGVyIE9FTTwvTGljZW5zZVR5cGU+CiAgICA8TGljZW5zZU5vdGU+TGltaXRlZCB0byAxIGRldmVsb3BlciwgdW5saW1pdGVkIHBoeXNpY2FsIGxvY2F0aW9uczwvTGljZW5zZU5vdGU+CiAgICA8T3JkZXJJRD4xOTA4MjYwODA3NTM8L09yZGVySUQ+CiAgICA8VXNlcklEPjEzNDk3NjAwNjwvVXNlcklEPgogICAgPE9FTT5UaGlzIGlzIGEgcmVkaXN0cmlidXRhYmxlIGxpY2Vuc2U8L09FTT4KICAgIDxQcm9kdWN0cz4KICAgICAgPFByb2R1Y3Q+QXNwb3NlLlRvdGFsIGZvciAuTkVUPC9Qcm9kdWN0PgogICAgPC9Qcm9kdWN0cz4KICAgIDxFZGl0aW9uVHlwZT5FbnRlcnByaXNlPC9FZGl0aW9uVHlwZT4KICAgIDxTZXJpYWxOdW1iZXI+M2U0NGRlMzAtZmNkMi00MTA2LWIzNWQtNDZjNmEzNzE1ZmMyPC9TZXJpYWxOdW1iZXI+CiAgICA8U3Vic2NyaXB0aW9uRXhwaXJ5PjIwMjAwODI3PC9TdWJzY3JpcHRpb25FeHBpcnk+CiAgICA8TGljZW5zZVZlcnNpb24+My4wPC9MaWNlbnNlVmVyc2lvbj4KICAgIDxMaWNlbnNlSW5zdHJ1Y3Rpb25zPmh0dHBzOi8vcHVyY2hhc2UuYXNwb3NlLmNvbS9wb2xpY2llcy91c2UtbGljZW5zZTwvTGljZW5zZUluc3RydWN0aW9ucz4KICA8L0RhdGE+CiAgPFNpZ25hdHVyZT53UGJtNUt3ZTYvRFZXWFNIY1o4d2FiVEFQQXlSR0pEOGI3L00zVkV4YWZpQnd5U2h3YWtrNGI5N2c2eGtnTjhtbUFGY3J0c0cwd1ZDcnp6MytVYk9iQjRYUndTZWxsTFdXeXNDL0haTDNpN01SMC9jZUFxaVZFOU0rWndOQkR4RnlRbE9uYTFQajhQMzhzR1grQ3ZsemJLZFZPZXk1S3A2dDN5c0dqYWtaL1E9PC9TaWduYXR1cmU+CjwvTGljZW5zZT4=")) });
                    }
                }
            }
            // assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
            // foreach (var assembly in assemblies)
            // {
            //     if (assembly.Name.StartsWith("Aspose."))
            //     {
            //         var type = Assembly.Load(assembly).GetType(assembly.Name + ".License");
            //         if (type == null)
            //         {
            //             type = Assembly.Load(assembly).GetType(System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(assembly.Name.ToLower()) + ".License");
            //         }
            //         if (type != null)
            //         {
            //             var instance = Activator.CreateInstance(type);
            //             type.GetMethod("SetLicense", new[] { typeof(Stream) }).Invoke(instance, new[] { new MemoryStream(Convert.FromBase64String("PExpY2Vuc2U+CiAgPERhdGE+CiAgICA8TGljZW5zZWRUbz5TdXpob3UgQXVuYm94IFNvZnR3YXJlIENvLiwgTHRkLjwvTGljZW5zZWRUbz4KICAgIDxFbWFpbFRvPnNhbGVzQGF1bnRlYy5jb208L0VtYWlsVG8+CiAgICA8TGljZW5zZVR5cGU+RGV2ZWxvcGVyIE9FTTwvTGljZW5zZVR5cGU+CiAgICA8TGljZW5zZU5vdGU+TGltaXRlZCB0byAxIGRldmVsb3BlciwgdW5saW1pdGVkIHBoeXNpY2FsIGxvY2F0aW9uczwvTGljZW5zZU5vdGU+CiAgICA8T3JkZXJJRD4xOTA4MjYwODA3NTM8L09yZGVySUQ+CiAgICA8VXNlcklEPjEzNDk3NjAwNjwvVXNlcklEPgogICAgPE9FTT5UaGlzIGlzIGEgcmVkaXN0cmlidXRhYmxlIGxpY2Vuc2U8L09FTT4KICAgIDxQcm9kdWN0cz4KICAgICAgPFByb2R1Y3Q+QXNwb3NlLlRvdGFsIGZvciAuTkVUPC9Qcm9kdWN0PgogICAgPC9Qcm9kdWN0cz4KICAgIDxFZGl0aW9uVHlwZT5FbnRlcnByaXNlPC9FZGl0aW9uVHlwZT4KICAgIDxTZXJpYWxOdW1iZXI+M2U0NGRlMzAtZmNkMi00MTA2LWIzNWQtNDZjNmEzNzE1ZmMyPC9TZXJpYWxOdW1iZXI+CiAgICA8U3Vic2NyaXB0aW9uRXhwaXJ5PjIwMjAwODI3PC9TdWJzY3JpcHRpb25FeHBpcnk+CiAgICA8TGljZW5zZVZlcnNpb24+My4wPC9MaWNlbnNlVmVyc2lvbj4KICAgIDxMaWNlbnNlSW5zdHJ1Y3Rpb25zPmh0dHBzOi8vcHVyY2hhc2UuYXNwb3NlLmNvbS9wb2xpY2llcy91c2UtbGljZW5zZTwvTGljZW5zZUluc3RydWN0aW9ucz4KICA8L0RhdGE+CiAgPFNpZ25hdHVyZT53UGJtNUt3ZTYvRFZXWFNIY1o4d2FiVEFQQXlSR0pEOGI3L00zVkV4YWZpQnd5U2h3YWtrNGI5N2c2eGtnTjhtbUFGY3J0c0cwd1ZDcnp6MytVYk9iQjRYUndTZWxsTFdXeXNDL0haTDNpN01SMC9jZUFxaVZFOU0rWndOQkR4RnlRbE9uYTFQajhQMzhzR1grQ3ZsemJLZFZPZXk1S3A2dDN5c0dqYWtaL1E9PC9TaWduYXR1cmU+CjwvTGljZW5zZT4=")) });
            //         }                    
            //     }               
            // }           
        }
 
        /// <summary>
        /// 停用Hook
        /// </summary>
        public static void StopHook()
        {
            if(mCompareHook != null && mCompareHook.IsEnabled)
            {
                mCompareHook.Remove();
            }
            if (mGreaterThanHook != null && mGreaterThanHook.IsEnabled)
            {
                mGreaterThanHook.Remove();
            }            
        }
 
        private static bool NewGreaterThan(DateTime t1, DateTime t2)
        { 
            if (Assembly.GetCallingAssembly().FullName.StartsWith("Aspose.") && t2.ToString("yyyyMMdd") == "20200827")
            {
                return false;
            }
            else
            {                
                return mGreaterThanHook.Call<bool>(null, t1, t2);
            }
        }
 
        private static int NewCompare(string s1, string s2)
        {
             
            if (Assembly.GetCallingAssembly().FullName.StartsWith("Aspose.") && s2 == "20200827")
            {
                return -1;
            }
            else
            {
                return mCompareHook.Call<int>(null, s1, s2);
            }
        }
    }
}