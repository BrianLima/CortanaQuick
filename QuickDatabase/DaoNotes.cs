using System;
using System.Collections.Generic;
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
                data = (from notes in db.Notes
                        orderby notes.date
                        select notes).ToList();
            }
            return data;
        }

        /// <summary>
        /// Salva um card para o banco de dados
        /// </summary>
        /// <param name="note"></param>
        /// <returns>bool</returns>
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
    }
}