// Created by Three Byte Intemedia, Inc. | project: PCManager |
// Created: 2021 02 28
// by Olaaf Rossi

using PCManager.WPFUI.Controllers;
using Serilog;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PCManager.WPFUI.ControlPages
{
    /// <summary>
    ///     Interaction logic for PCManagerInfoView.xaml
    /// </summary>
    public partial class PCManagerInfoView : INotifyPropertyChanged
    {
        private readonly PCManagerInfoController viewModel = new();

        private static string logFilePath = "log.txt";

        private string _fileText;

        //INotifyPropertyChanged members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public PCManagerInfoView()
        {
            DataContext = this;
            Loaded += OnLoaded;
            InitializeComponent();
           
        }

        public string FileText
        {
            get
            {
                return _fileText;
            }
            set
            {
                _fileText = value;
                OnPropertyChanged(FileText);
            }
        }

        public void ReadFile(string path)
        {
            using (FileStream stream = File.Open(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        FileText = stream.ToString();
                    }
                }
            }

            //FileText = File.OpenRead(logFilePath).ToString();

            OnPropertyChanged(FileText);
            //WriteLine(FileText);
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            DataContext = await viewModel.GetDataAsync();
            ReadFile(logFilePath);
        }

        //public void WriteLine(string input)
        //{
        //    Dispatcher.Invoke(() =>
        //        {
        //            PCManagerAppLogText.AppendText($"{input} \n");
        //            PCManagerAppLogText.ScrollToEnd();
        //        });
        //}

        private void AddToLogOnClick(object sender, RoutedEventArgs e)
        {
            Log.Logger.Information("happy");
        }
    }
}