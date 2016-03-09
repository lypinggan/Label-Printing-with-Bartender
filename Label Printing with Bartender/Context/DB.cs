using Label_Printing_with_Bartender.Entities;
using System.Data.Entity;

namespace Label_Printing_with_Bartender.Context
{
    public class DB : DbContext
    {
        public DB(string connectionStringName)
            : base (connectionStringName)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<DB>());
            Configuration.ProxyCreationEnabled = false;
        }

        public IDbSet<BartenderFormat> BartenderFormats
        {
            get
            {
                return Set<BartenderFormat>();
            }
        }
        public IDbSet<BartenderSubString> BartenderSubStrings
        {
            get
            {
                return Set<BartenderSubString>();
            }
        }

        public IDbSet<BartenderDatabase> BartenderDatabases
        {
            get
            {
                return Set<BartenderDatabase>();
            }
        }
        public IDbSet<DatabaseTable> DatabaseTables
        {
            get
            {
                return Set<DatabaseTable>();
            }
        }
        public IDbSet<DatabaseColumn> DatabaseColumns
        {
            get
            {
                return Set<DatabaseColumn>();
            }
        }

        public IDbSet<Links> Links
        {
            get
            {
                return Set<Links>();
            }
        }
    }
}
