using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Globalization;
using Microsoft.Devices;
using System.Collections.Generic;

namespace Cortana_Quick
{
    public partial class TutorialPage : PhoneApplicationPage
    {
        public TutorialPage()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            List<string> beta = new List<string>{"en-US","en-GB"};
            List<string> alpha = new List<string> {"en-CA", "en-IN", "en-AU"};

            if (beta.Contains(getLocalization()))
            {
                task.Uri = new System.Uri("http://www.windowsphone.com/" + getLocalization() + "/how-to/wp8/cortana/start-using-cortana", System.UriKind.RelativeOrAbsolute);
                task.Show();
            }
            else if (alpha.Contains(getLocalization()))
            {
                task.Uri = new System.Uri("http://www.windowsphone.com/" + getLocalization() + "/how-to/wp8/cortana/cortana-alpha", System.UriKind.RelativeOrAbsolute);
                task.Show();
            }
            else
            {
                MessageBox.Show("Sorry, but it seems that your current culture: " + getLocalization() + " don't have support for Cortana or this app isn't compatible with your language, sorry.");
            }
        }

        private string getLocalization()
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
        }
    }
}