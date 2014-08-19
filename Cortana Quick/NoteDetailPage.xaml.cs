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
            //Todo Update button's icons
            ApplicationBarIconButton appBarButtonSave = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
            appBarButtonSave.Text = AppResources.AppBarButtonTextSave;
            ApplicationBar.Buttons.Add(appBarButtonSave);
            appBarButtonSave.Click +=AppBarButtonSave_Click;

            ApplicationBarIconButton appBarButtonDelete = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
            appBarButtonDelete.Text = AppResources.AppBarButtonTextDelete;
            ApplicationBar.Buttons.Add(appBarButtonDelete);
            appBarButtonDelete.Click += AppBarButtonDelete_Click;
        }

        private void AppBarButtonDelete_Click(object sender, EventArgs e)
        {
            //ToDo Programmatically close page
        }

        private void AppBarButtonSave_Click(object sender, EventArgs e)
        {
            note.UpdateNote(note.id, BoxNoteDetail.Text);
        }

        //ToDo implement note detail page
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            int value = Int32.Parse(NavigationContext.QueryString["parameter"]);
            note = note.GetNote(value);
            BoxNoteDetail.Text = note.note;
            BlockNoteDate.Text = note.date.ToString();
        }
    }
}