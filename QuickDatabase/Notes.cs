using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq.Mapping;

namespace QuickDatabase
{
    /// <summary>
    /// Data structure for notes storage
    /// </summary>
    [Table(Name = "Notes")]
    public class Notes
    {
        private int _id;
        [Column(Name = "id", IsPrimaryKey = true, IsDbGenerated = true, CanBeNull = false)]
        public int id { get { return _id; } set { _id = value; } }

        private string _note;
        [Column(Name = "note", CanBeNull = false)]
        public string note { get { return _note; } set { _note = value; } }

        private DateTime _date;
        [Column(Name = "date", CanBeNull = false)]
        public DateTime date { get { return _date; } set { _date = value; } }

        public IEnumerable GetAllNotes()
        {
            DaoNotes daoNotes = new DaoNotes();
            return daoNotes.GetAllNotes();
        }

        public IEnumerable<string> GetSimilarNotes(string keyword)
        {
            DaoNotes daoNotes = new DaoNotes();
            return daoNotes.GetSimilarNotes(keyword);
        }

        public bool Save()
        {
            DaoNotes daoNotes = new DaoNotes();
            return daoNotes.Save(this);
        }

        public bool DestroyOldNotes(int days, bool destroy)
        {
            if (destroy)
            {
                DaoNotes daoNotes = new DaoNotes();
                return daoNotes.DestroyOldNotes(days);
            }
            else
            {
                return false;
            }
        }
    }
}
