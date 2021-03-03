// Created by Three Byte Intemedia, Inc. | project: PCManager |
// Created: 2021 02 28
// by Olaaf Rossi

using System;

using PCManager.WPFUI.Controllers;
using Serilog;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using PCManager.DataAccess.Library;

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
            SQLiteCRUD sql = new SQLiteCRUD(MainWindow.GetConnectionString());
            GetLogs(sql);
            Log.Logger.Information("PCManager Info View Started");
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            DataContext = await viewModel.GetDataAsync();
        }

        private void GetLogs(SQLiteCRUD sql)
        {
            var rows = sql.GetAllLogs();
            this.LogGrid.ItemsSource = rows;
        }
    }
}