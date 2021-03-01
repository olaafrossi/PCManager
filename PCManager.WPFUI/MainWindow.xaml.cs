// Created by Three Byte Intemedia, Inc. | project: PCManager |
// Created: 2021 02 27
// by Olaaf Rossi

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using ModernWpf.Controls;
using PCManager.WPFUI.Helpers;
using PCManager.WPFUI.Navigation;
using PCManager.WPFUI.Properties;

namespace PCManager.WPFUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var rootFrame = NavigationRootPage.RootFrame;
            SetBinding(TitleBar.IsBackButtonVisibleProperty,
                new Binding { Path = new PropertyPath(System.Windows.Controls.Frame.CanGoBackProperty), Source = rootFrame });

            SubscribeToResourcesChanged();
        }

        public void SetupApp()
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.EventLog("ThreeByteCrestronNetworkMonitor",
                    "Application", ".", false,
                    "{Message}", restrictedToMinimumLevel: LogEventLevel.Verbose, eventIdProvider: null,
                    formatProvider: null)
                .CreateLogger();

            // write our first log message
            WriteLine($"{DateTime.Now:HH:mm:ss.fff} | Application Starting");

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                    {
                        services.AddTransient<IPcNetworkListener, PcNetworkListener>();
                    })
                .UseSerilog()
                .Build();
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath("C:\\ThreeByteIntermedia\\CrestronNetworkMonitor\\Settings\\")
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile(
                    $"appsettings.json.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                    true)
                .AddEnvironmentVariables();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

                if (this == Application.Current.MainWindow)
                {
                    this.SetPlacement(Settings.Default.MainWindowPlacement);
                }
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

        [Conditional("DEBUG")]
        private void SubscribeToResourcesChanged()
        {
            Type t = typeof(FrameworkElement);
            EventInfo ei = t.GetEvent("ResourcesChanged", BindingFlags.NonPublic | BindingFlags.Instance);
            Type tDelegate = ei.EventHandlerType;
            MethodInfo h = GetType().GetMethod(nameof(OnResourcesChanged), BindingFlags.NonPublic | BindingFlags.Instance);
            Delegate d = Delegate.CreateDelegate(tDelegate, this, h);
            MethodInfo addHandler = ei.GetAddMethod(true);
            object[] addHandlerArgs = { d };
            addHandler.Invoke(this, addHandlerArgs);
        }

        private void OnResourcesChanged(object sender, EventArgs e)
        {
        }
    }
}