using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCManager.WPFUI.Models
{
    public class PCManagerAppInfoModel
    {
        public string AssemblyFileVersion { get; set; }

        public string AssemblyInformationVersion { get; set; }

        public string AssemblyVersion { get; set; }

        public string DotNetInfo { get; set; }

        public string InstallLocation { get; set; }

        public string PackageVersion { get; set; }

        public string AppInstallerUri { get; set; }

        public string PackageChannel { get; set; }

        public string DisplayName { get; set; }

        public string MSIXVersionNumber { get; set; }
    }
}
