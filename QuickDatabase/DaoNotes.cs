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
    }
}
