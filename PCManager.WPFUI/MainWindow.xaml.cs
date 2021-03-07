// Created by Three Byte Intemedia, Inc. | project: PCManager |
// Created: 2021 02 27
// by Olaaf Rossi

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModernWpf.Controls;

using PCManager.WPFUI.Controllers;
using PCManager.WPFUI.Helpers;
using PCManager.WPFUI.Properties;
using Serilog;

using ThreeByteLibrary.Dotnet;

using Frame = System.Windows.Controls.Frame;

namespace PCManager.WPFUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.SetupApp();
            var rootFrame = NavigationRootPage.RootFrame;
        }

        public void SetupApp()
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Build()).WriteTo
                .SQLite(Properties.Resources.SQLiteDBPath).CreateLogger();

            var host = Host.CreateDefaultBuilder().ConfigureServices(
                (context, services) =>
                    {
                        services.AddTransient<IPcNetworkListener, PcNetworkListener>();
                    })
                .UseSerilog()
                .Build();

            // launch the class
            var svcPcNetworkListener = ActivatorUtilities.CreateInstance<PcNetworkListener>(host.Services);
            svcPcNetworkListener.Run();
            svcPcNetworkListener.MessageHit += SvcPcNetworkListener_MessageHit;
        }

        private void SvcPcNetworkListener_MessageHit(object sender, PcNetworkListener.PCNetworkListenerMessages e)
        {
            PCNetworkListenerController netListenerController = new PCNetworkListenerController();
            netListenerController.SvcPcNetworkListener_MessageHit(sender, e);
        }

        private static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Properties.Resources.LocalDataFolder).AddJsonFile(
                Properties.Resources.AppSettingsFile,
                false,
                true);
        }

        private static void CreateLocalDirectoryForAppFiles()
        {
            AppSettings jsonSettings = new AppSettings();
            string desiredFolder = Properties.Resources.LocalDataFolder;

            //Ensure the directory exists
            if (Directory.Exists(desiredFolder) is false)
            {
                Directory.CreateDirectory(desiredFolder);
            }

            string file = $"{desiredFolder}appsettings.json";
            var options = new JsonSerializerOptions { WriteIndented = true };

            string jsonString = JsonSerializer.Serialize(jsonSettings, options);
            File.WriteAllText(file, jsonString);
        }
    }
}