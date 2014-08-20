using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using QuickDatabase;
using Cortana_Quick.Resources;

namespace Cortana_Quick
{
    public partial class NoteDetailPage : PhoneApplicationPage
    {
        Notes note;
        public NoteDetailPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            //Todo Update button's icons
            ApplicationBarIconButton appBarButtonSave = new ApplicationBarIconButton(new Uri("/Assets/check.png", UriKind.Relative));
            appBarButtonSave.Text = AppResources.AppBarButtonTextSave;
            ApplicationBar.Buttons.Add(appBarButtonSave);
            appBarButtonSave.Click +=AppBarButtonSave_Click;

            ApplicationBarIconButton appBarButtonDelete = new ApplicationBarIconButton(new Uri("/Assets/close.png", UriKind.Relative));
            appBarButtonDelete.Text = AppResources.AppBarButtonTextDelete;
            ApplicationBar.Buttons.Add(appBarButtonDelete);
            appBarButtonDelete.Click += AppBarButtonDelete_Click;
        }

        private void AppBarButtonDelete_Click(object sender, EventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void AppBarButtonSave_Click(object sender, EventArgs e)
        {
            note.UpdateNote(note.id, BoxNoteDetail.Text);
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        //ToDo implement note detail page
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            note = new Notes();
            base.OnNavigatedTo(e);
            int id = Int32.Parse(NavigationContext.QueryString["parameter"]);
            note = note.GetNote(id);
            BoxNoteDetail.Text = note.note;
            BlockNoteDate.Text = note.date.ToString();
        }
    }
}