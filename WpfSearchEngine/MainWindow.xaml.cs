using HtmlAgilityPackCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace WpfSearchEngine
{
    public partial class MainWindow : Window
    {
        private readonly GoogleSearchService _searchService;

        public MainWindow()
        {
            InitializeComponent();
            _searchService = new GoogleSearchService("AIzaSyBFkpnkdDorzSJFQG7evn5JontTYMRh9Jk", "b71fc5ac20de049b9");
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var results = await _searchService.SearchAsync(txtSearch.Text,
                int.Parse(((ComboBoxItem)cmbResultsCount.SelectedItem).Content.ToString()));

            lvResults.ItemsSource = results;
            lblResultsCount.Text = $"تعداد نتایج: {results.Count}";
        }
    }
}