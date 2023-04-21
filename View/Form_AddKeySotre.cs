using AppSingle.Entity;
using AppSingle.Utils;
using Sunny.UI;
using System;
using System.IO;
using System.Windows.Forms;

namespace AppSingle.View
{
    public partial class Form_AddKeySotre : UIForm
    {

        private UIForm _Form;
        private readonly Action _AddConfigAction;

        private ConfigEntity _Config;

        public Form_AddKeySotre(UIForm form, Action addConfigAction)
        {
            InitializeComponent();
            InitData();

            this._AddConfigAction = addConfigAction;
            this._Form = form;
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

        private void uiLabel1_Click(object sender, EventArgs e)
        {
            var path = FileUtils.GetSelectFilePath(defalutPath: _Config.PreSelectedKeySinglePath);
            uiLabel1.Text = path;
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            var path = uiLabel1.Text;
            if (path.IsNullOrEmpty())
            {
                DialogUtils.ShowMessageDialog("签名文件不能为空");
                return;
            }
            if (!Path.GetExtension(path).Equals(".jks"))
            {
                DialogUtils.ShowMessageDialog("签名文件必须为jks");
                return;
            }
            string alias = uiTextBox2.Text;
            if (alias.IsNullOrEmpty())
            {
                DialogUtils.ShowMessageDialog("别名不能为空");
                return;
            }
            string password = uiTextBox3.Text;
            if (password.IsNullOrEmpty())
            {
                DialogUtils.ShowMessageDialog("密码不能为空");
                return;
            }
            var singlePathList = _Config.SinglePathList;
            var entity = new SinglePathEntity
            {
                Path = path,
                Alias = alias,
                Password = password,
                Name = uiTextBox1.Text
            };
            singlePathList.Add(entity);
            _Config.SinglePathList = singlePathList;
            _Config.PreSelectedKeySinglePath = Path.GetDirectoryName(path);
            _Config = FileUtils.Write(_Config);
            _Form.Invoke(_AddConfigAction);
            Close();
        }
    }
}
