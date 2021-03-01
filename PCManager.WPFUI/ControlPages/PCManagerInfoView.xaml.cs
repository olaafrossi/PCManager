// Created by Three Byte Intemedia, Inc. | project: PCManager |
// Created: 2021 02 28
// by Olaaf Rossi

using PCManager.WPFUI.Controllers;
using PCManager.WPFUI.Models;
using System;
using System.Windows;
using System.Windows.Controls;

using Serilog;

namespace PCManager.WPFUI.ControlPages
{
    /// <summary>
    ///     Interaction logic for PCManagerInfoView.xaml
    /// </summary>
    public partial class PCManagerInfoView
    {
        private readonly PCManagerInfoController viewModel = new();

        public PCManagerInfoView()
        {
            Loaded += OnLoaded;
            InitializeComponent();
            WriteLine($"{DateTime.Now} | app starting");
            for (int i = 0; i < 50; i++)
            {
                WriteLine($"{DateTime.Now} num: {i}");
            }

        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            DataContext = await viewModel.GetDataAsync();
        }

        public void WriteLine(string input)
        {
            Dispatcher.Invoke(() =>
                {
                    PCManagerAppLogText.AppendText($"{input} \n");
                    PCManagerAppLogText.ScrollToEnd();
                });
        }
    }
}