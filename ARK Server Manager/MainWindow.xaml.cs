﻿using ARK_Server_Manager.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ARK_Server_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance
        {
            get;
            private set;
        }

        public ObservableCollection<TabItem> ServerTabs
        {
            get;
            set;
        }

        private TabItem defaultTab;

        public MainWindow()
        {
            InitializeComponent();
            MainWindow.Instance = this;
            ServerTabs = new ObservableCollection<TabItem>();            
            this.DataContext = this;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AddDefaultServerTab();            

            // We need to load the set of existing servers, or create a blank one if we don't have any...
            var tabAdded = false;
            foreach(var profile in Directory.EnumerateFiles(Config.Default.ConfigDirectory, "*" + Config.Default.ProfileExtension))
            {
                var settings = ServerSettings.LoadFrom(profile);
                AddNewServerTab(settings);
                tabAdded = true;
            }

            if (!tabAdded)
            {
                AddNewServerTab(new ServerSettings());
            }
        }

        private void AddDefaultServerTab()
        {
            this.defaultTab = new TabItem();
            this.defaultTab.Header = "+";
            ServerTabs.Add(this.defaultTab);
        }

        private int AddNewServerTab(ServerSettings settings)
        {
            var newTab = new TabItem();
            var control = new ServerSettingsControl(settings);
            newTab.Content = control;
            newTab.DataContext = control;
            newTab.SetBinding(TabItem.HeaderProperty, new Binding("Settings." + ServerSettings.ProfileNameProperty));
            this.ServerTabs.Insert(this.ServerTabs.Count - 1, newTab);
            return this.ServerTabs.Count - 2;
        }

        private void Servers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tabControl = sender as TabControl;
            if (tabControl != null)
            {
                if (tabControl.SelectedItem == this.defaultTab)
                {
                    tabControl.SelectedIndex = AddNewServerTab(new ServerSettings());                    
                }
            }
        }

        private void SettingsTab_LostFocus(object sender, RoutedEventArgs e)
        {
            Config.Default.Save();
        }

        public void SwitchToSettingsTab()
        {
            this.Tabs.SelectedIndex = 1;
        }
    }
}
