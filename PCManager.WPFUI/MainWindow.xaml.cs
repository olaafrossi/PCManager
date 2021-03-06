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
            Console.WriteLine("hello");
            this.SetupApp();
            this.LogTest();

            var rootFrame = NavigationRootPage.RootFrame;
            this.SetBinding(
                TitleBar.IsBackButtonVisibleProperty,
                new Binding { Path = new PropertyPath(Frame.CanGoBackProperty), Source = rootFrame });

            this.SubscribeToResourcesChanged();
        }

        public void LogTest()
        {
            int a = 1;
            int b = 4;
            Log.Logger.Error("i'm messing with ints {a} {b}", a, b);
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
                    }).UseSerilog().Build();

            // launch the class
            var svcPcNetworkListener = ActivatorUtilities.CreateInstance<PcNetworkListener>(host.Services);
            svcPcNetworkListener.Run();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (!e.Cancel)
            {
                if (this == Application.Current.MainWindow)
                {
                    Settings.Default.MainWindowPlacement = this.GetPlacement();
                    Settings.Default.Save();
                }
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            if (this == Application.Current.MainWindow)
            {
                this.SetPlacement(Settings.Default.MainWindowPlacement);
            }
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

        /*protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == FocusManager.FocusedElementProperty)
            {
                Debug.WriteLine("FocusedElement: " + e.NewValue);
            }
        }*/

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            var rootFrame = NavigationRootPage.RootFrame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private void OnResourcesChanged(object sender, EventArgs e)
        {
        }

        [Conditional("DEBUG")]
        private void SubscribeToResourcesChanged()
        {
            Type t = typeof(FrameworkElement);
            EventInfo ei = t.GetEvent("ResourcesChanged", BindingFlags.NonPublic | BindingFlags.Instance);
            Type tDelegate = ei.EventHandlerType;
            MethodInfo h = this.GetType().GetMethod(
                nameof(this.OnResourcesChanged),
                BindingFlags.NonPublic | BindingFlags.Instance);
            Delegate d = Delegate.CreateDelegate(tDelegate, this, h);
            MethodInfo addHandler = ei.GetAddMethod(true);
            object[] addHandlerArgs = { d };
            addHandler.Invoke(this, addHandlerArgs);
        }
    }
}