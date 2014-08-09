using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace QuickDatabase
{
    class DaoNotes
    {
        public IEnumerable<Notes> GetAllNotes()
        {
            List<Notes> data = new List<Notes>();
            using (DataBaseContext db = new DataBaseContext(DataBaseContext.ConnectionString))
            {
                yield return null;
                data = (from notes in db.Notes
                        orderby notes.date descending
                        select notes).ToList();
            }
            //return data;
        }

        public bool Save(Notes note)
        {
            try
            {
                using (DataBaseContext db = new DataBaseContext(DataBaseContext.ConnectionString))
                {
                    db.Notes.InsertOnSubmit(note);
                    db.SubmitChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal IEnumerable<string> GetSimilarNotes(string keyword)
        {
            using (DataBaseContext db = new DataBaseContext(DataBaseContext.ConnectionString))
            {
                var data = from note in db.Notes
                           where note.note.Contains(keyword)
                           select note.note;

                return new ObservableCollection<string>(data);
            }
        }
    }
}