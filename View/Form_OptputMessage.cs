using AppSingle.Entity;
using AppSingle.Utils;
using Sunny.UI;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace AppSingle.View
{
    public partial class Form_OptputMessage : UIForm
    {
        private readonly SynchronizationContext _SyncContext = null;

        private readonly SinglePathEntity _SingleInfo;
        private readonly string _ApkFolderPath;
        private List<string> _Apks;
        private bool _Singleing = false;
        private string _OutputPath;
        private int _CurrentIndex = 0;

        public Form_OptputMessage(SinglePathEntity singleInfo, string apkFolder)
        {
            InitializeComponent();
            _SyncContext = SynchronizationContext.Current;
            _SingleInfo = singleInfo;
            _ApkFolderPath = apkFolder;

            var thread = new Thread(() => Single());
            thread.Start();
        }

        private void Single()
        {
            var allApkDir = Directory.GetDirectories(_ApkFolderPath);
            _Apks = new List<string>();
            allApkDir.ForEach(dir =>
            {
                var cDirs = Directory.GetDirectories(dir);
                if (cDirs.Length > 0)
                {
                    var cFiles = Directory.GetFiles(cDirs[0]);
                    cFiles.ForEach(f => {
                        if (Path.GetExtension(f).Equals(".apk"))
                        {
                            _Apks.Add(f);
                        }
                    });
                }
            });
            var singleAll = false;
            while (!singleAll)
            {
                if (_Apks.Count == 0)
                {
                    singleAll = true;
                }
                else
                {
                    if (!_Singleing)
                    {
                        _CurrentIndex++;
                        _Singleing = true;
                        var file = _Apks[0];
                        Single(file);
                    }
                    Thread.Sleep(1000);
                }
            }
            _SyncContext.Post(ShowDialog, "全部Apk签名结束, 一共签名" + _CurrentIndex + "个文件.");
        }

        private void Single(string apkFile)
        {
            var path = Path.GetDirectoryName(apkFile);
            var fileName = Path.GetFileName(apkFile);
            var outputPath = Path.Combine(_ApkFolderPath, "Single");
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            _OutputPath = Path.Combine(outputPath, fileName);
            var singlePath = _SingleInfo.Path;
            var alias = _SingleInfo.Alias;
            var password = _SingleInfo.Password;

            string cmd = "jarsigner -verbose -keystore \"{0}\" -storepass {1} -signedjar \"{2}\" \"{3}\" {4}";
            string cmdStr = string.Format(cmd, singlePath, password, _OutputPath, apkFile, alias);

            Process proc = new Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.Arguments = "/c " + cmdStr;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            proc.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            proc.BeginOutputReadLine();
            proc.WaitForExit();
            proc.Close();
            _Apks.RemoveAt(0);
            _Singleing = false;
        }

        private void OutputHandler(object sender, DataReceivedEventArgs outLine)
        {
            if (!string.IsNullOrEmpty(outLine.Data))
            {
                _SyncContext.Post(HandlerTextRefresh, outLine.Data);
            }
        }

        private void HandlerTextRefresh(object text)
        {
            if (!_Singleing) return;
            StringBuilder sb = new StringBuilder(uiRichTextBox1.Text);
            sb.AppendLine((string)text);
            uiRichTextBox1.Text = sb.ToString();
            uiRichTextBox1.SelectionStart = uiRichTextBox1.Text.Length;
            uiRichTextBox1.ScrollToCaret();
        }

        private void ShowDialog(object message)
        {
            DialogUtils.ShowMessageDialog((string)message);
        }
    }
}
