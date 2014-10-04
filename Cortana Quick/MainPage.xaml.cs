using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using QuickDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Windows.Phone.Speech.Synthesis;
using Windows.Phone.Speech.VoiceCommands;

namespace Cortana_Quick
{
    public partial class MainPage : PhoneApplicationPage
    {  
        SpeechSynthesizer talk;
        public static List<String> phrases = new List<String> {"are", "my","is", "on", "where", "i", "left", "does", "did", "put", "kept"};

        public MainPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem("settings");
            appBarMenuItem.Click += AppBarMenuItem_Click;
            ApplicationBar.MenuItems.Add(appBarMenuItem);

            ApplicationBarMenuItem appBarMenuItemTutorial = new ApplicationBarMenuItem("tutorial");
            appBarMenuItemTutorial.Click += appBarMenuItemTutorial_Click;
            ApplicationBar.MenuItems.Add(appBarMenuItemTutorial);

            ApplicationBarMenuItem appBarMenuItemReview = new ApplicationBarMenuItem("review and rate me!");
            appBarMenuItemReview.Click += appBarMenuItemReview_Click;
            ApplicationBar.MenuItems.Add(appBarMenuItemReview);
        }

        void appBarMenuItemTutorial_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/TutorialPage.xaml", UriKind.Relative));
        }

        void appBarMenuItemReview_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask task = new MarketplaceReviewTask();
            task.Show();
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
            base.OnNavigatedTo(e);

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
                    HandleAskCommands(result);
                }
            }
        }

        private async void HandleAskCommands(string question)
        {
            question = question.Replace("?", String.Empty);
            string[] words = question.Split(' ');
            List<Notes> results = new List<Notes>();
            Notes note = new Notes();

            for (int i = 0; i < words.Length; i++)
            {
                if (!phrases.Contains(words[i]))
                {
                    results.AddRange(note.GetSimilarNotes(words[i]));
                }
            }

            if (results.Count > 0)
            {
                //Here is where is the magic, selecting the most frequent note found using the users given keywords
                Notes mostFrequent = results.GroupBy(id => id).OrderByDescending(g => g.Count()).Take(1).Select(g => g.Key).First();
                mostFrequent.note = mostFrequent.note.Replace("my ", "your ");
                mostFrequent.note = mostFrequent.note.Replace("i ", "You ");
                if (mostFrequent != null || !String.IsNullOrEmpty(mostFrequent.note))
                {
                    if (!mostFrequent.note.StartsWith("You"))
                    {
                        mostFrequent.note = "You " + mostFrequent.note;
                    }
                    try
                    {
                        using (talk = new SpeechSynthesizer())
                        {
                            await talk.SpeakTextAsync(mostFrequent.note);
                        }
                        //MessageBox.Show(mostFrequent.note, "Here is what i found:", MessageBoxButton.OK);
                    }
                    catch (Exception)
                    {
                        //MessageBox.Show("Error when trying to use Text to speech", "Error", MessageBoxButton.OK);
                    }
                }
                else
                {
                    using (talk = new SpeechSynthesizer())
                    {
                        await talk.SpeakTextAsync("Sorry, i couldn't find any matching notes.");
                    }
                    //MessageBox.Show(mostFrequent.note, "Here is what found", MessageBoxButton.OK);
                }
            }
            else
            {
                //MessageBox.Show("I couldn't find any matching note, try again", "Oops", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// We found a note command, so we save it
        /// </summary>
        /// <param name="note"></param>
        private async void HandleNoteCommands(string text)
        {
            bool verify = StorageHelper.GetSetting("VERIFY_INPUT", true);
            var a = MessageBoxResult.Yes;
            
            if (verify)
            {
                a = await CortanaOverlay.ShowAsync(text, "I heard you say:", "Yes", "No");
            }

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
                //MessageBox.Show(vcdEx.Message);
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
                //if (!note.DestroyNote()) MessageBox.Show("Error while deleting your note");
                NotesList.ItemsSource = note.GetAllNotes();
            }
        }

        private async void UpdateLiveTile(string note)
        {
            ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault();
            StandardTileData standardData = new StandardTileData
            {
                BackContent = note,
                BackTitle = "Quick for Cortana",
                BackgroundImage = new Uri("/Assets/ApplicationIcon.png", UriKind.Relative),
                Title = "Quick for Cortana"
            };
           tile.Update(standardData);
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            if (StorageHelper.GetSetting("FIRST_USE", true))
            {
                StorageHelper.StoreSetting("FIRST_USE", false, true);
                NavigationService.Navigate(new Uri("/TutorialPage.xaml", UriKind.Relative));
            }
        }
    }
}