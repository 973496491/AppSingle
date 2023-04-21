using System.Collections.Generic;

namespace AppSingle.Entity
{
    public class ConfigEntity
    {
        public string PreSelectedApkPath { get; set; }

        public string PreSelectedKeySinglePath { get; set; }

        public List<SinglePathEntity> SinglePathList { get; set; }
    }

    public class SinglePathEntity
    {
        public string Path;
        public string Name;
        public string Alias;
        public string Password;
    }
}
