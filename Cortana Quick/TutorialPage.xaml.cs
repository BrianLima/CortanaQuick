using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

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
            task.Uri = new System.Uri("http://www.windowsphone.com/en-us/how-to/wp8/cortana/start-using-cortana", System.UriKind.RelativeOrAbsolute);
            task.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new System.Uri("http://www.windowsphone.com/en-IN/how-to/wp8/cortana/cortana-alpha", System.UriKind.RelativeOrAbsolute);
            task.Show();
        }
    }
}