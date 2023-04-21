using AppSingle.Entity;
using Newtonsoft.Json;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace AppSingle.Utils
{
    class FileUtils
    {
        public readonly static string CONFIG_FILE_NAME = "config.json";

        public static ConfigEntity Read()
        {
            string configPath = Path.Combine(Application.StartupPath, CONFIG_FILE_NAME);
            StreamReader sr = new StreamReader(configPath, false);
            string configStr = sr.ReadToEnd();
            sr.Close();
            return JsonConvert.DeserializeObject<ConfigEntity>(configStr);
        }

        public static ConfigEntity Write(ConfigEntity entiy)
        {
            string configPath = Path.Combine(Application.StartupPath, CONFIG_FILE_NAME);
            if (!File.Exists(configPath))
            {
                var stream = File.Create(configPath);
                stream.Close();
            }
            if (entiy == null)
            {
                entiy = new ConfigEntity();
                var deskTop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                entiy.PreSelectedKeySinglePath = deskTop;
                entiy.SinglePathList = new List<SinglePathEntity>();
            }

            var configJson = JsonConvert.SerializeObject(entiy);
            using (StreamWriter sw = new StreamWriter(configPath, false))
            {
                sw.WriteLine(configJson);
                sw.Close();
            }
            return entiy;
        }

        /// <summary>
        /// 获取指定文件路径
        /// </summary>
        /// <returns></returns>
        public static string GetSelectFilePath(string title = "", string filter = "全部(*.*)|*.*", string defalutPath = "")
        {

            OpenFileDialog dialog = new OpenFileDialog
            {
                Multiselect = false,
                Title = title,
                Filter = filter
            };
            if (!defalutPath.IsNullOrEmpty())
            {
                dialog.InitialDirectory = defalutPath;
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            return "";
        }

        /// <summary>
        /// 获取文件夹路径
        /// </summary>
        public static string GetSelectFolderPath(
            string desc = null,
            string selectPath = null,
            Environment.SpecialFolder rootFolder = Environment.SpecialFolder.Desktop
        )
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog
            {
                RootFolder = rootFolder
            };
            if (desc != null && !desc.Equals(""))
            {
                dialog.Description = desc;
            }
            if (selectPath != null && !selectPath.Equals(""))
            {
                dialog.SelectedPath = selectPath;
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.SelectedPath;
            }
            return "";
        }

        /// <summary>
        /// 复制指定文件夹
        /// </summary>
        public static void CopyDirectory(string sourcePath, string destPath)
        {
            string floderName = Path.GetFileName(sourcePath);
            DirectoryInfo di = Directory.CreateDirectory(Path.Combine(destPath, floderName));
            string[] files = Directory.GetFileSystemEntries(sourcePath);

            foreach (string file in files)
            {
                if (Directory.Exists(file))
                {
                    CopyDirectory(file, di.FullName);
                }
                else
                {
                    File.Copy(file, Path.Combine(di.FullName, Path.GetFileName(file)), true);
                }
            }
        }

        /// <summary>
        /// 打开文件夹并选中指定文件
        /// </summary>
        public static void OpenFolderAndSelectFile(string fileName)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe")
            {
                Arguments = "/e,/select," + fileName
            };
            System.Diagnostics.Process.Start(psi);
        }

        /// <summary>
        /// 将异常打印到LOG文件
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="LogAddress">日志文件地址</param>
        public static void WriteLog(Exception ex, string LogAddress = "")
        {
            //如果日志文件为空，则默认在Debug目录下新建 YYYY-mm-dd_Log.log文件
            if (LogAddress == "")
            {
                LogAddress = Environment.CurrentDirectory + '\\' +
                    DateTime.Now.Year + '-' +
                    DateTime.Now.Month + '-' +
                    DateTime.Now.Day + "_Log.log";
            }

            //把异常信息输出到文件
            StreamWriter sw = new StreamWriter(LogAddress, true);
            sw.WriteLine("当前时间：" + DateTime.Now.ToString());
            sw.WriteLine("异常信息：" + ex.Message);
            sw.WriteLine("异常对象：" + ex.Source);
            sw.WriteLine("调用堆栈：\n" + ex.StackTrace.Trim());
            sw.WriteLine("触发方法：" + ex.TargetSite);
            sw.WriteLine();
            sw.Close();
        }
    }
}
