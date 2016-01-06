using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Label_Printing_with_Bartender.Entities
{
    public class BartenderDatabase
    {
        /// <summary>
        /// Database identity.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the database attached to the Bartender format.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// The connection string of the attached database.
        /// </summary>
        public string DatabaseConnectionString { get; set; }

        /// <summary>
        /// Collection of the tables class attached to the database
        /// </summary>
        public ICollection<DatabaseTable> DatabaseTables{ get; set; }

    }

    public class DatabaseTable
    {
        /// <summary>
        /// Database identity.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Table name.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Foreign Key for BartenderDatabase
        /// </summary>
        public int BartenderDatabaseId { get; set; }

        /// <summary>
        /// Collection of the columns class attached to the table
        /// </summary>
        public ICollection<DatabaseColumn> DatabaseColumns { get; set; }
    }
    
    public class DatabaseColumn
    {
        /// <summary>
        /// Database identity.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Colunm name.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Foreign Key for DatabaseTable
        /// </summary>
        public int DatabaseTableId { get; set; }
    }
}
