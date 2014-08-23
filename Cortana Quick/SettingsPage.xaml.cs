using Cortana_Quick.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
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
            BuildLocalizedApplicationBar();

            bool delete = StorageHelper.GetSetting("AUTO_DELETE", false);
            int days = StorageHelper.GetSetting("MAXIMUM_DATE", 1);
            bool verify = StorageHelper.GetSetting("VERIFY_INPUT", true);

            CheckDelete.IsChecked = delete;
            CheckVerify.IsChecked = verify;

            for (int i = 0; i < 31; i++)
            {
                source.Add(new numbers() { Value = i + 1 });
            }

            this.listPicker.ItemsSource = source;
            this.listPicker.SelectedIndex = days -= 1;
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            //Todo Update button's icons
            ApplicationBarIconButton appBarButtonSave = new ApplicationBarIconButton(new Uri("/Assets/check.png", UriKind.Relative));
            appBarButtonSave.Text = AppResources.AppBarButtonTextSave;
            ApplicationBar.Buttons.Add(appBarButtonSave);
            appBarButtonSave.Click += AppBarButtonSave_Click;

            ApplicationBarIconButton appBarButtonDelete = new ApplicationBarIconButton(new Uri("/Assets/close.png", UriKind.Relative));
            appBarButtonDelete.Text = AppResources.AppBarButtonTextDelete;
            ApplicationBar.Buttons.Add(appBarButtonDelete);
            appBarButtonDelete.Click += AppBarButtonDelete_Click;
        }

        private void AppBarButtonDelete_Click(object sender, EventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void AppBarButtonSave_Click(object sender, EventArgs e)
        {
            StorageHelper.StoreSetting("VERIFY_INPUT", this.CheckVerify.IsChecked, true);
            StorageHelper.StoreSetting("MAXIMUM_DATE", this.listPicker.SelectedIndex + 1, true);
            StorageHelper.StoreSetting("AUTO_DELETE", this.CheckDelete.IsChecked, true);

            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void listPicker_SelectionChanged(object sender, SelectionChangedEventArgs e) 
        { 
            //StorageHelper.StoreSetting("MAXIMUM_DATE", this.listPicker.SelectedIndex + 1, true); 
        } 
    }

    class numbers
    {
        public int Value { get; set; }
    }
}