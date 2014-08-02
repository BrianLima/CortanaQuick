using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Cortana_Quick.Resources;
using Windows.Phone.Speech.VoiceCommands;
using System.Threading.Tasks;
using QuickDatabase;

namespace Cortana_Quick
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        /// <summary>
        /// We need to check if the app called by Cortana and then handle the voice commands, else whe install the voice commands
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //ToDo Fix double navigation, somehow the app is calling OnNavigated two times after resuming
            //if (e.NavigationMode == NavigationMode.New)
            {
                string voiceCommandName;
                if (NavigationContext.QueryString.TryGetValue("voiceCommandName", out voiceCommandName))
                {
                    HandleVoiceCommand(voiceCommandName);
                }
                else
                {
                    Task.Run(() => InstallVoiceCommands());
                }
            }

            base.OnNavigatedTo(e);

            Notes notes = new Notes();
            NotesList.ItemsSource = notes.GetAllNotes();
        }

        //After identifying the voice activation we need to check if it is a Note or Ask Command
        private void HandleVoiceCommand(string voiceCommandName)
        {
            string result;
            if (NavigationContext.QueryString.TryGetValue("NoteKeyWords", out result))
            {
                if (HandleConnectionProblems(result))
                {
                    HandleNoteCommands(result);
                }
            }
            else if (NavigationContext.QueryString.TryGetValue("AskKeyWords", out result))
            {
                if (HandleConnectionProblems(result))
                {
                    //ToDo handle ask commands
                }
            }
        }

        /// <summary>
        /// We found a note command, so we save it
        /// </summary>
        /// <param name="note"></param>
        private void HandleNoteCommands(string text)
        {
            Notes note = new Notes();
            note.date = DateTime.Now;
            note.note = text;
            note.Save();
        }
        
        /// <summary>
        /// If Cortana gave us a Null or empty string or ... there was a connection problem 
        /// </summary>
        /// <param name="text">The string Cortana gave us</param>
        /// <returns>false if a connection problem as found || true if everything is ok </returns>
        private Boolean HandleConnectionProblems(string text)
        {
            if (String.IsNullOrEmpty(text) || text == "...")
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Install the voice commands to Cortana
        /// </summary>
        private async void InstallVoiceCommands()
        {
            const string Path = "ms-appx:///VoiceDefinition.xml";
            try
            {
                Uri file = new Uri(Path, UriKind.Absolute);

                await VoiceCommandService.InstallCommandSetsFromFileAsync(file);
            }
            catch (Exception vcdEx)
            {
                MessageBox.Show(vcdEx.Message);
            }
        }

        private void NoteDetailClick(object sender, RoutedEventArgs e)
        {

        }
    }
}