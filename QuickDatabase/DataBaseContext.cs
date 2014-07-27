using System.Data.Linq;

namespace QuickDatabase
{
    class DataBaseContext : DataContext
    {
        public const string ConnectionString = "Data Source = isostore:/notes.sdf";

        private Table<Notes> notes;
        public Table<Notes> Notes
        {
            get
            {
                if (notes == null) notes = GetTable<Notes>();

                return notes;
            }
        }

        public DataBaseContext(string connectionString)
            : base(connectionString)
        {
            if (!this.DatabaseExists())
            {
                this.CreateDatabase();
            }
        }
    }
}
