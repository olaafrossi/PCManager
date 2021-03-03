// Created by Three Byte Intemedia, Inc. | project: PCManager |
// Created: 2021 03 01
// by Olaaf Rossi

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;

using Windows.ApplicationModel;
using Windows.Foundation.Metadata;

using OSVersionHelper;

using PCManager.WPFUI.Models;

using Serilog;

namespace PCManager.WPFUI.Controllers
{
    public class PCManagerInfoController
    {
        private static ObservableCollection<PCManagerAppInfoModel> output;

        public async Task<IEnumerable<PCManagerAppInfoModel>> GetDataAsync()
        {
            output = new ObservableCollection<PCManagerAppInfoModel>();
            
            Log.Logger.Information("dfdfd");
            
            PCManagerAppInfoModel appInfoModel = new PCManagerAppInfoModel();

            output.Add(
                new PCManagerAppInfoModel
                    {
                        AssemblyVersion = GetAssemblyVersion(),
                        AssemblyFileVersion = GetAssemblyFileVersion(),
                        AssemblyInformationVersion = GetAssemblyInformationVersion(),
                        DotNetInfo = GetDotNetInfo(),
                        InstallLocation = GetInstallLocation(),
                        PackageVersion = GetPackageVersion(),
                        AppInstallerUri = GetAppInstallerUri(),
                        PackageChannel = GetPackageChannel(),
                        DisplayName = GetDisplayName(),
                        MSIXVersionNumber = $"MSIX Version # {this.GetMsixPackageVersion().ToString()}"
                    });
            return output;
        }


        public Version GetMsixPackageVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var manifestPath = assembly.Location.Replace(assembly.ManifestModule.Name, "") + @"..\AppxManifest.xml";
            if (File.Exists(manifestPath))
            {
                var xDoc = XDocument.Load(manifestPath);
                return new Version(
                    xDoc.Descendants().First(e => e.Name.LocalName == "Identity").Attributes()
                        .First(a => a.Name.LocalName == "Version").Value);
            }
            return new Version(0, 0, 0, 0);
        }

        internal static string GetAppInstallerUri()
        {
            string result;

            if (!WindowsVersionHelper.HasPackageIdentity) return "Not packaged";

            if (ApiInformation.IsMethodPresent("Windows.ApplicationModel.Package", "GetAppInstallerInfo"))
            {
                var aiUri = GetAppInstallerInfoUri(Package.Current);
                if (aiUri != null)
                {
                    result = $"MSIX Installer URL: {aiUri}";
                }
                else
                {
                    result = "not present";
                }
            }
            else
            {
                result = "Not Available";
            }
            return result;
        }

        internal static string GetAssemblyFileVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string output = fileVersionInfo.FileVersion;
            return $"Assembly File Version: {output}";
        }

        internal static string GetAssemblyInformationVersion()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                string output = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion;
                return $"Assembly Information Version: {output}";
            }
            catch (Exception e)
            {
                return e.ToString();
                //throw;
            }
        }

        internal static string GetAssemblyVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyVersion = assembly.GetName().Version;
            string output = assemblyVersion.ToString();
            return $"Assembly Version: {output}";
        }

        internal static string GetDisplayName()
        {
            if (WindowsVersionHelper.HasPackageIdentity)
            {
                return $"MSIX Package Display Name: {Package.Current.DisplayName}";
            }

            return "No Display Name, Not MSIX packaged";
        }

        internal static string GetDotNetInfo()
        {
            var runTimeDir = new FileInfo(typeof(string).Assembly.Location);
            var entryDir = new FileInfo(Assembly.GetEntryAssembly().Location);
            var isSelfContaied = runTimeDir.DirectoryName == entryDir.DirectoryName;

            var result = ".NET Framework Install Type - ";
            if (isSelfContaied)
            {
                result += "Self Contained Deployment";
            }
            else
            {
                result += "Framework Dependent Deployment";
            }

            return result;
        }

        internal static string GetInstallLocation()
        {
            return $"Application is installed at this location: {Assembly.GetExecutingAssembly().Location}";
        }

        internal static string GetPackageChannel()
        {
            if (WindowsVersionHelper.HasPackageIdentity)
            {
                return
                    $"Package Channel: {Package.Current.Id.Name.Substring(Package.Current.Id.Name.LastIndexOf('.') + 1)}";
            }

            return "No Package Channel";
        }

        internal static string GetPackageVersion()
        {
            if (WindowsVersionHelper.HasPackageIdentity)
            {
                return
                    $"MSIX Package Version: {Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}.{Package.Current.Id.Version.Revision}";
            }

            return "Not MSIX packaged";
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Uri GetAppInstallerInfoUri(Package p)
        {
            var aiInfo = p.GetAppInstallerInfo();
            if (aiInfo != null)
            {
                return aiInfo.Uri;
            }

            return null;
        }
    }
}