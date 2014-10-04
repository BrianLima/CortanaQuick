using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using System.ComponentModel;
using Microsoft.Phone.Shell;

namespace Cortana_Quick
{
    public partial class CortanaOverlay : UserControl
    {
        private PhoneApplicationPage _page;
        private Color _originalTrayColor;
        private readonly TaskCompletionSource<MessageBoxResult> _completionSource;

        private CortanaOverlay()
        {
            InitializeComponent();
            _completionSource = new TaskCompletionSource<MessageBoxResult>();
        }

        public static Task<MessageBoxResult> ShowAsync(string message, string caption, string yesButtonText, string noButtonText = null)
        {
            CortanaOverlay msgBox = new CortanaOverlay();
            msgBox.HeaderTextBlock.Text = caption;
            msgBox.MessageTextBlock.Text = message;
            msgBox.YesButton.Content = yesButtonText;
            if (string.IsNullOrWhiteSpace(noButtonText))
            {
                msgBox.NoButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                msgBox.NoButton.Content = noButtonText;
            }
            msgBox.Insert();

            return msgBox._completionSource.Task;
        }

        private void Insert()
        {
            // Make an assumption that this is within a phone application that is developed "normally"
            var frame = Application.Current.RootVisual as Microsoft.Phone.Controls.PhoneApplicationFrame;
            _page = frame.Content as PhoneApplicationPage;

            // store the original color, and change the tray to the chrome brush
            _originalTrayColor = SystemTray.BackgroundColor;
            SystemTray.BackgroundColor = ((SolidColorBrush)Application.Current.Resources["PhoneChromeBrush"]).Color;

            bool shouldBuffer = SystemTray.Opacity < 1;
            if (shouldBuffer)
            {
                // adjust the margin to account for the page shifting up
                Margin = new Thickness(0, -32, 0, 0);
                var margin = MessagePanel.Margin;
                MessagePanel.Margin = new Thickness(margin.Left, 64, margin.Right, margin.Bottom);
            }

            _page.BackKeyPress += Page_BackKeyPress;

            // assume the child is a Grid, span all of the rows
            var grid = System.Windows.Media.VisualTreeHelper.GetChild(_page, 0) as Grid;
            if (grid.RowDefinitions.Count > 0)
            {
                Grid.SetRowSpan(this, grid.RowDefinitions.Count);
            }
            grid.Children.Add(this);

            // Create a transition like the regular MessageBox
            SwivelTransition transitionIn = new SwivelTransition();
            transitionIn.Mode = SwivelTransitionMode.BackwardIn;

            ITransition transition = transitionIn.GetTransition(LayoutRoot);
            EventHandler transitionCompletedHandler = null;
            transitionCompletedHandler = (s, e) =>
            {
                transition.Completed -= transitionCompletedHandler;
                transition.Stop();
            };
            transition.Completed += transitionCompletedHandler;
            transition.Begin();

            if (_page.ApplicationBar != null)
            {
                // Hide the app bar so they cannot open more message boxes
                _page.ApplicationBar.IsVisible = false;
            }
        }

        private void Remove(MessageBoxResult result)
        {
            _page.BackKeyPress -= Page_BackKeyPress;

            var frame = (PhoneApplicationFrame)Application.Current.RootVisual;
            var page = frame.Content as PhoneApplicationPage;
            var grid = VisualTreeHelper.GetChild(page, 0) as Grid;

            // Create a transition like the regular MessageBox
            SwivelTransition transitionOut = new SwivelTransition();
            transitionOut.Mode = SwivelTransitionMode.BackwardOut;

            ITransition transition = transitionOut.GetTransition(LayoutRoot);
            EventHandler transitionCompletedHandler = null;
            transitionCompletedHandler = (s, e) =>
            {
                transition.Completed -= transitionCompletedHandler;
                SystemTray.BackgroundColor = _originalTrayColor;
                transition.Stop();
                grid.Children.Remove(this);
                if (page.ApplicationBar != null)
                {
                    page.ApplicationBar.IsVisible = true;
                }
                _completionSource.SetResult(result);
            };
            transition.Completed += transitionCompletedHandler;
            transition.Begin();
        }

        private void Page_BackKeyPress(object sender, CancelEventArgs e)
        {
            Remove(MessageBoxResult.Cancel);
            e.Cancel = true;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Remove(MessageBoxResult.Yes);
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Remove(MessageBoxResult.No);
        }
    }
}
