using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using QuickDatabase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Windows.Phone.Speech.Synthesis;
using Windows.Phone.Speech.VoiceCommands;

namespace Cortana_Quick
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        SpeechSynthesizer talk;

        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            //// Create a new button and set the text value to the localized string from AppResources.
            //ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
            //appBarButton.Text = AppResources.AppBarButtonText;
            //ApplicationBar.Buttons.Add(appBarButton);

            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem("settings");
            appBarMenuItem.Click += appBarMenuItem_Click;
            ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

        void appBarMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

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
            
            bool delete = StorageHelper.GetSetting("AUTO_DELETE", true);
            int days = StorageHelper.GetSetting("MAXIMUM_DATE", 1);
            
            Notes notes = new Notes();
            notes.DestroyOldNotes(days, delete);           
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
                    HandleAskCommands(result);
                }
            }
        }

        private async void HandleAskCommands(string question)
        {
            string[] words = question.Split(' ');
            List<String> results = new List<string>();
            Notes note = new Notes();
            
            for (int i = 0; i < words.Length; i++)
            {
                results.AddRange(note.GetSimilarNotes(words[i]));
            }

            try
            {
                await talk.SpeakTextAsync(results[0]);
            }
            catch (Exception exception)
            {
                throw new Exception("Error when trying to use TTS", exception);
            }
        }

        /// <summary>
        /// We found a note command, so we save it
        /// </summary>
        /// <param name="note"></param>
        private async void HandleNoteCommands(string text)
        {
            var a = MessageBox.Show(text, "Heard you say:", MessageBoxButton.OKCancel);

            //ToDo Write a verification method to make sure that Cortana and our user are friends
            if (a != MessageBoxResult.Cancel)
            {
                Notes note = new Notes();
                note.date = DateTime.Now;
                note.note = text;
                note.Save();
                note = null;
            }
        }
        
        /// <summary>
        /// If Cortana gave us a Null or empty string or '...' there was a connection problem 
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

        private void DeleteNoteClick(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            //Todo Delete single note
        }
    }
}