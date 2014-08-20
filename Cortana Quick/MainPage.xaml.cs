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
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using System.Linq;

namespace Cortana_Quick
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static List<String> phrases = new List<String> {"are", "my","is", "on"
		};


        // Constructor
        public MainPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
            //UpdateLiveTile();
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
            appBarMenuItem.Click += AppBarMenuItem_Click;
            ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

        void AppBarMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// We need to check if the app called by Cortana and then handle the voice commands, else whe install the voice commands
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
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
            
            bool delete = StorageHelper.GetSetting("AUTO_DELETE", false);
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
                if (!phrases.Contains(words[i]))
                {
                    results.AddRange(note.GetSimilarNotes(words[i]));
                }
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

            if (a != MessageBoxResult.Cancel)
            {
                Notes note = new Notes();
                note.date = DateTime.Now;
                note.note = text;
                note.Save();
                note = null;
                UpdateLiveTile(text);
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
            var t = sender as MenuItem;
            if (t != null)
            {
                Notes note = t.DataContext as Notes;
                string id = note.id.ToString();
                NavigationService.Navigate(new Uri("/NoteDetailPage.xaml?parameter=" + id, UriKind.RelativeOrAbsolute));
            }
        }

        private void DeleteNoteClick(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                Notes note = mi.DataContext as Notes;
                if (!note.DestroyNote()) MessageBox.Show("Error while deleting your note");
                NotesList.ItemsSource = note.GetAllNotes();
            }
        }

        private async void UpdateLiveTile(string note)
        {
            ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault();
            StandardTileData standardData = new StandardTileData
            {
                BackContent = note,
                BackTitle = "Cortana Quick",
                BackgroundImage = new Uri("/Assets/ApplicationIcon.png", UriKind.Relative),
                Title = "Cortana Quick"
            };
            tile.Update(standardData);

        }
    }
}