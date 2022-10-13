using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace XGame
{

    public static class CMDHelper
    {
// // 打开记事本
//         System.Diagnostics.Process.Start("notepad.exe");        
// // 打开计算器
//         System.Diagnostics.Process.Start("calc.exe ");                
// // 打开注册表
//         System.Diagnostics.Process.Start("regedit.exe ");           
// // 打开画图板
//         System.Diagnostics.Process.Start("mspaint.exe ");        
// // 打开写字板
//         System.Diagnostics.Process.Start("write.exe ");              
// // 打开播放器
//         System.Diagnostics.Process.Start("mplayer2.exe ");        
// // 打开任务管理器
//         System.Diagnostics.Process.Start("taskmgr.exe ");          
// // 打开事件查看器
//         System.Diagnostics.Process.Start("eventvwr.exe ");          
// // 打开系统信息
//         System.Diagnostics.Process.Start("winmsd.exe ");           
// // 打开Windows版本信息
//         System.Diagnostics.Process.Start("winver.exe ");              
// // 发邮件
//         System.Diagnostics.Process.Start("mailto: "+ address);    

        public static string RunCmd(string command)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";         //确定程序名
            p.StartInfo.Arguments = "/c " + command;   //确定程式命令行
            p.StartInfo.UseShellExecute = false;      //Shell的使用
            p.StartInfo.RedirectStandardInput = true;  //重定向输入
            p.StartInfo.RedirectStandardOutput = true; //重定向输出
            p.StartInfo.RedirectStandardError = true;  //重定向输出错误
            p.StartInfo.CreateNoWindow = true;        //设置置不显示示窗口
            p.Start();   
            return p.StandardOutput.ReadToEnd();      //输出出流取得命令行结果果
        }        


        /// <summary>
        /// 构建Process对象，并执行
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="args">命令的参数</param>
        /// <param name="workingDri">工作目录</param>
        /// <returns>Process对象</returns>
        private static Process CreateCmdProcess(string cmd, string args, string workingDir = "")
        {
            var en = System.Text.UTF8Encoding.UTF8;
            if (Application.platform == RuntimePlatform.WindowsEditor)
                en = System.Text.Encoding.GetEncoding("gb2312");

            var pStartInfo = new System.Diagnostics.ProcessStartInfo(cmd);
            pStartInfo.Arguments = args;
            pStartInfo.CreateNoWindow = false;
            pStartInfo.UseShellExecute = false;
            pStartInfo.RedirectStandardError = true;
            pStartInfo.RedirectStandardInput = true;
            pStartInfo.RedirectStandardOutput = true;
            pStartInfo.StandardErrorEncoding = en;
            pStartInfo.StandardOutputEncoding = en;
            if (!string.IsNullOrEmpty(workingDir))
                pStartInfo.WorkingDirectory = workingDir;
            return System.Diagnostics.Process.Start(pStartInfo);
        }

        /// <summary>
        /// 运行命令,不返回stderr版本
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="args">命令的参数</param>
        /// <param name="workingDri">工作目录</param>
        /// <returns>命令的stdout输出</returns>
        public static string RunCmdNoErr(string cmd, string args, string workingDri = "")
        {
            var p = CreateCmdProcess(cmd, args, workingDri);
            var res = p.StandardOutput.ReadToEnd();
            p.Close();
            return res;
        }

        /// <summary>
        /// 运行命令,不返回stderr版本
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="args">命令的参数</param>
        /// <param name="input">StandardInput</param>
        /// <param name="workingDri">工作目录</param>
        /// <returns>命令的stdout输出</returns>
        public static string RunCmdNoErr(string cmd, string args, string[] input, string workingDri = "")
        {
            var p = CreateCmdProcess(cmd, args, workingDri);
            if (input != null && input.Length > 0)
            {
                for (int i = 0; i < input.Length; i++)
                    p.StandardInput.WriteLine(input[i]);
            }

            var res = p.StandardOutput.ReadToEnd();
            p.Close();
            return res;
        }

        /// <summary>
        /// 运行命令
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="args">命令的参数</param>
        /// <returns>string[] res[0]命令的stdout输出, res[1]命令的stderr输出</returns>
        public static string[] RunCmd(string cmd, string args, string workingDir = "")
        {
            string[] res = new string[2];
            var p = CreateCmdProcess(cmd, args, workingDir);
            res[0] = p.StandardOutput.ReadToEnd();
            res[1] = p.StandardError.ReadToEnd();
#if !UNITY_IOS
            res[2] = p.ExitCode.ToString();
#endif
            p.Close();
            return res;
        }

        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="absPath">文件夹的绝对路径</param>
        public static void OpenFolderInExplorer(string absPath)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
                RunCmdNoErr("explorer.exe", absPath);
            else if (Application.platform == RuntimePlatform.OSXEditor)
                RunCmdNoErr("open", absPath.Replace("\\", "/"));
        }
    }
}