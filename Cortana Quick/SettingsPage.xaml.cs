using Microsoft.Phone.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Cortana_Quick
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        List<numbers> source = new List<numbers>();
        
        public SettingsPage()
        {
            InitializeComponent();
            
            
            for (int i = 0; i < 31; i++)
            {
                source.Add(new numbers() { Value = i + 1 });
            }

            CheckDelete.IsChecked = StorageHelper.GetSetting("AUTO_DELETE", false);
            CheckVerify.IsChecked = StorageHelper.GetSetting("VERIFY", true);
            this.listPicker.ItemsSource = source;
            
            int days = StorageHelper.GetSetting("MAXIMUM_DATE", 1);
            
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

        private void CheckVerify_Checked(object sender, RoutedEventArgs e)
        {
            StorageHelper.StoreSetting("VERIFY", true, true);
        }

        private void CheckVerify_Unchecked(object sender, RoutedEventArgs e)
        {
            //Bug It seems that this setting isn't being correctly stored for some unknown reason
            StorageHelper.StoreSetting("VERIFY", false, true);
        }
    }

    class numbers
    {
        public int Value { get; set; }
    }
}