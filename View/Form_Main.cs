using AppSingle.Entity;
using AppSingle.Utils;
using AppSingle.View;
using Sunny.UI;
using System;
using System.IO;
using System.Windows.Forms;

namespace AppSingle
{
    public partial class Form_Main : UIForm
    {


        private ConfigEntity _Config;
        private SinglePathEntity _SingleInfo;

        private string _CurrentApkPath = "";

        public Form_Main()
        {
            InitializeComponent();
            InitData();
            Init_ListBoxData();
        }

        private void InitData()
        {
            string configPath = Path.Combine(Application.StartupPath, FileUtils.CONFIG_FILE_NAME);
            if (!File.Exists(configPath))
            {
                _Config = FileUtils.Write(null);
            }
            else
            {
                _Config = FileUtils.Read();
            }
        }

        private void Init_ListBoxData()
        {
            uiListBox1.Items.Clear();
            var list = _Config.SinglePathList;
            foreach (var item in list)
            {
                uiListBox1.Items.Add(item.Name + "   " + item.Path);
            }
        }

        private void uiButton1_Click(object sender, System.EventArgs e)
        {
            var from = new Form_AddKeySotre(this, AddConfigReturn);
            from.Show();
        }

        private void AddConfigReturn()
        {
            _Config = FileUtils.Read();
            Init_ListBoxData();
        }

        private void uiListBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            int index = uiListBox1.SelectedIndex;
            if (index == -1) return;
            var list = _Config.SinglePathList;
            var entity = list[index];
            _SingleInfo = entity;
            uiLabel1.Text = entity.Path;
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            string path = FileUtils.GetSelectFolderPath(selectPath: _Config.PreSelectedApkPath);
            if (path.IsNullOrEmpty()) return;
            _CurrentApkPath = path;
            _Config.PreSelectedApkPath = path;
            _Config = FileUtils.Write(_Config);
            uiLabel2.Text = path;
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            if (_SingleInfo == null)
            {
                DialogUtils.ShowMessageDialog("签名文件不存在");
                return;
            }
            if (!Directory.Exists(_CurrentApkPath))
            {
                DialogUtils.ShowMessageDialog("Apk文件夹不存在");
                return;
            }
            var form = new Form_OptputMessage(_SingleInfo, _CurrentApkPath);
            form.Show();
        }
    }
}
