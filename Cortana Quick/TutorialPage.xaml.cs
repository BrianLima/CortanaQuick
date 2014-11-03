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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new System.Uri("http://www.windowscentral.com/want-cortana-outside-us-heres-how", System.UriKind.RelativeOrAbsolute);
            task.Show();
        }
    }
}