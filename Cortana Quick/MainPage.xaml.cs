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
        public static List<String> phrases = new List<String> {"are", "my", "is", "on", "where", "what", "who", "i", "left", "does", "did", "put", "kept"};
        bool noting = false;
        Notes note;

        public MainPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem("settings");
            appBarMenuItem.Click += AppBarMenuItem_Click;
            ApplicationBar.MenuItems.Add(appBarMenuItem);

            ApplicationBarMenuItem appBarMenuItemTutorial = new ApplicationBarMenuItem("tutorial");
            appBarMenuItemTutorial.Click += appBarMenuItemTutorial_Click;
            ApplicationBar.MenuItems.Add(appBarMenuItemTutorial);

            ApplicationBarMenuItem appBarMenuItemReview = new ApplicationBarMenuItem("review and rate me!");
            appBarMenuItemReview.Click += appBarMenuItemReview_Click;
            ApplicationBar.MenuItems.Add(appBarMenuItemReview);

#if DEBUG
            ApplicationBarIconButton testNoting = new ApplicationBarIconButton(new Uri("/Assets/Tiles/add.png", UriKind.Relative));
            testNoting.Text = "Test Noting";
            testNoting.Click += testNoting_Click;
            ApplicationBar.Buttons.Add(testNoting);

            ApplicationBarIconButton testQuestion = new ApplicationBarIconButton(new Uri("/Assets/Tiles/add.png", UriKind.Relative));
            testQuestion.Text = "Test Question";
            testQuestion.Click += testQuestion_Click;
            ApplicationBar.Buttons.Add(testQuestion);
#endif
        }

        void testQuestion_Click(object sender, EventArgs e)
        {
            HandleAskCommands("When am i going to work?");
        }

        void testNoting_Click(object sender, EventArgs e)
        {
            HandleNoteCommands("Parked my car here");
        }

        void appBarButtonAdd_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/NoteDetailPage.xaml?parameter=" + 0, UriKind.RelativeOrAbsolute));
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

        private void HandleAskCommands(string question)
        {
            noting = false;
            question = question.Replace("?", String.Empty);
            string[] words = question.Split(' ');
            List<Notes> results = new List<Notes>();
            Notes note = new Notes();

            for (int i = 0; i < words.Length; i++)
            {
                if (!phrases.Contains(words[i]))
                {
                    results.AddRange(note.GetSimilarNotes(" " + words[i] + " "));
                }
            }

            if (results.Count > 0)
            {
                //Here is where is the magic, selecting the most frequent note found using the users given keywords
                Notes mostFrequent = results.GroupBy(id => id).OrderByDescending(g => g.Count()).Take(1).Select(g => g.Key).First();
                mostFrequent.note = mostFrequent.note.Replace("my ", "your ");
                mostFrequent.note = mostFrequent.note.Replace("i ", "You ");
                mostFrequent.note = mostFrequent.note.Replace("I am ", "You are");
                if (mostFrequent != null || !String.IsNullOrEmpty(mostFrequent.note))
                {
                    if (!mostFrequent.note.StartsWith("You"))
                    {
                        mostFrequent.note = "You " + mostFrequent.note;
                    }
                    try
                    {
                        CortanaOverlay("Here is what i found:", mostFrequent.note, String.Empty);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error when trying to use Text to speech", "Error", MessageBoxButton.OK);
                    }
                }
                else
                {
                    CortanaOverlay("Sorry, i couldn't find anything", String.Empty, String.Empty);
                }
            }
            else
            {
                CortanaOverlay("Sorry, i couldn't find anything", String.Empty, String.Empty);
            }
        }

        /// <summary>
        /// We found a note command, so we save it
        /// </summary>
        /// <param name="note"></param>
        private void HandleNoteCommands(string text)
        {
            noting = true;
            text = "I " + text;
            bool verify = StorageHelper.GetSetting("VERIFY_INPUT", true);
            CortanaOverlay("I heard you say:", text, "Should i note it?");
            if (verify)
            {
                note = new Notes();
                note.date = DateTime.Now;
                note.note = text;
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

        private void UpdateLiveTile(string note)
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

        private void CortanaOverlay(String title, String message, String confirmation)
        {
            string left, right;
            CortanaOverlayData data = new CortanaOverlayData();
            data.Title = title;
            data.Message = message;
            data.Confirmation = confirmation;
            if (!noting)
            {
                left = "Ok";
                right = "Cancel";
            }
            else
            {
                left = "Yes";
                right = "No";
            }

            CustomMessageBox CortanaOverlay = new CustomMessageBox()
            {
                ContentTemplate = (DataTemplate)this.Resources["CortanaOverlay"],
                LeftButtonContent = left,
                RightButtonContent = right,
                IsFullScreen = false,
                Content = data
            };

            CortanaOverlay.Dismissing += CortanaOverlay_Dismissing;
            Speech(title + "..." + message + "..." + confirmation);
            CortanaOverlay.Show();
        }

        void CortanaOverlay_Dismissing(object sender, DismissingEventArgs e)
        {
            if (noting)
            {
                switch (e.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        note.Save();
                        UpdateLiveTile(note.note);
                        break;
                    case CustomMessageBoxResult.None:
                        break;
                    case CustomMessageBoxResult.RightButton:
                        break;
                    default:
                        break;
                }  
                NotesList.ItemsSource = note.GetAllNotes();
            }
            noting = false;
        }

        private async void Speech(string text)
        {
            talk = new SpeechSynthesizer();
            try
            {
                await talk.SpeakTextAsync(text);
                talk.Dispose();
            }
            catch (Exception)
            {
                MessageBox.Show("Error when trying to use Text to speech", "Error", MessageBoxButton.OK);
                talk.Dispose();
            }
        }
    }
}