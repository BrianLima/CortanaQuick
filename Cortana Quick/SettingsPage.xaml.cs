using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Cortana_Quick
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        List<numbers> source = new List<numbers>();
        
        bool delete;
        int days;
        public SettingsPage()
        {
            InitializeComponent();
            delete = StorageHelper.GetSetting("AUTO_DELETE", false);
            days = StorageHelper.GetSetting("MAXIMUM_DATE", 1);
            for (int i = 0; i < 31; i++)
            {
                source.Add(new numbers() { Value = i + 1 });
            }

            if (delete)
            {
                CheckDelete.IsChecked = true;
            }
            else
            {
                CheckDelete.IsChecked = false;
            }
            
            this.listPicker.ItemsSource = source;
            this.listPicker.SelectedIndex = days -= 1;
        }

        private void CheckDelete_Checked(object sender, RoutedEventArgs e)
        {
            this.listPicker.IsEnabled = true;
            StorageHelper.StoreSetting("AUTO_DELETE", true, true);
        }

        private void CheckDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            this.listPicker.IsEnabled = false;
            StorageHelper.StoreSetting("AUTO_DELETE", false, true);
        }

        private void listPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StorageHelper.StoreSetting("MAXIMUM_DATE", this.listPicker.SelectedIndex + 1, true);
        }
    }
    public class numbers
    {
        public int Value { get; set; }
    }
}