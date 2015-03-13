using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using System.Windows;

namespace Cortana_Quick
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask follow = new WebBrowserTask();
            follow.Uri = new Uri("https://m.twitter.com/BrianoStorm", UriKind.Absolute);
            follow.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var task = new EmailComposeTask
            {
                To = "Brian.ghwt@outlook.com",
                Subject = "Sugestions/Problems with Quick for Cortana"
            };
            task.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            WebBrowserTask follow = new WebBrowserTask();
            follow.Uri = new Uri("https://m.twitter.com/BrianoStorm", UriKind.Absolute);
            follow.Show();
        }
    }
}