using Label_Printing_with_Bartender.Entities;
using Label_Printing_with_Bartender.Repository;
using Microsoft.Win32;
using Seagull.BarTender.Print;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Windows;

namespace Label_Printing_with_Bartender.Services
{
    public sealed class Services : IServices, IDisposable
    {
        public bool checkBartenderEngineStarts()
        {
            try
            {
                runBartenderEngine();
                return true;
            }
            catch (Exception err)
            {
                showMessageBox(err.Message);
                return false;
            }
        }

        public Engine runBartenderEngine()
        {
            return new Engine(true);
        }

        public void fileExistsCheck(IRepository repository)
        {
            //Get all formats and include SubStrings and Dtabases
            List<BartenderFormat> formatlist = repository.Get<BartenderFormat>().
                Include(i => i.BartenderDatabases).
                ToList();
            //For each format we check the format exists...
            foreach (BartenderFormat format in formatlist)
            {
                if (File.Exists(Path.GetFullPath(format.FormatPath)))
                {
                    foreach (BartenderDatabase database in format.BartenderDatabases.ToList())
                    {
                        if (!File.Exists(Path.GetFullPath(database.DatabaseConnectionString)))
                        {
                            showMessageBox("Database doesn't exists and was deleted from the application database: " + database.DatabaseConnectionString);
                            deleteDB(repository, database.Id);
                        }
                    }
                }
                else
                {
                    showMessageBox("Format doesn't exists and was deleted from the application database: " + format.FormatName);
                    deleteFMT(repository, format.Id);
                    repository.Save();
                }
            }
        }

        public BartenderFormat addFMT(IRepository repository)
        {
            BartenderFormat newformat = new BartenderFormat();
            Dictionary<string, string> results = openFileBox("Bartender Files|*.btw", true);
            foreach (KeyValuePair<string, string> fn in results)
            {
                newformat.FormatName = fn.Key;
                newformat.FormatPath = fn.Value;
                newformat.SubStrings = new List<BartenderSubString>();
                Engine engine = runBartenderEngine();
                LabelFormat openformat = engine.Documents.Open(newformat.FormatPath);
                if (openformat != null)
                {
                    foreach (var ss in openformat.SubStrings)
                    {
                        newformat.SubStrings.Add(new BartenderSubString()
                        {
                            SubStringName = ss.Name,
                            SubStringValue = ss.Value,
                            SubStringSerializeBy = ss.SerializeBy,
                            SubStringSerializeEvery = ss.SerializeEvery,
                            SubStringRollover = ss.Rollover,
                            SubStringRolloverLimit = ss.RolloverLimit,
                            SubStringRolloverResetValue = ss.RolloverResetValue,
                            SubStringType = ss.Type,
                        });
                    }
                }
                repository.Add(newformat);
                repository.Save();
            }
            return newformat;
        }

        public LabelFormatDocument openBartenderFormat(string path, Engine engine)
        {
            return engine.Documents.Open(path);
        }

        public void deleteFMT(IRepository repository, int id)
        {
            BartenderFormat deletingformat = repository.Get<BartenderFormat>().Where(w => w.Id == id).Include(i => i.SubStrings).Include(i => i.BartenderDatabases).First();
            List<Links> listoflinks = repository.Get<Links>().Where(w => w.BartenderFormat.Id == id).ToList();
            foreach (BartenderDatabase btdb in deletingformat.BartenderDatabases.ToList())
            {
                deleteDB(repository, btdb.Id);
            }
            repository.Delete(deletingformat);
            repository.Save();
        }

        public void deleteDB(IRepository repository, int id)
        {
            List<DatabaseTable> tables = repository.Get<DatabaseTable>().Where(w => w.BartenderDatabaseId == id).Include(i => i.DatabaseColumns).ToList();
            foreach (DatabaseTable table in tables)
            {
                List<DatabaseColumn> colunms = repository.Get<DatabaseColumn>().Where(w => w.DatabaseTableId == table.Id).ToList();
                foreach (DatabaseColumn colunm in colunms)
                {
                    repository.Delete(colunm);
                }
                repository.Delete(table);
            }
            List<Links> links = repository.Get<Links>().Where(w => w.BartenderDatabase.Id == id).ToList();
            foreach (Links link in links)
            {
                repository.Delete(link);
            }
            int databasecount = repository.Get<BartenderDatabase>().Where(w => w.Id == id).Count();
            if (databasecount > 0)
            {
                repository.Delete(repository.Get<BartenderDatabase>().Where(w => w.Id == id).First());
                repository.Save();
            }
        }

        public void addBartenderDatabase(IRepository repository, BartenderFormat selectedformat)
        {
            //open file dialog to get users database
            Dictionary<string, string> results = openFileBox("Access Database Files|*.mdb", false);
            foreach (KeyValuePair<string, string> fn in results)
            {
                if (Path.GetExtension(fn.Value) == ".mdb")
                {
                    //for each database user selects make a new database in EF
                    BartenderDatabase currentdatabase = new BartenderDatabase()
                    {
                        DatabaseName = fn.Key,
                        DatabaseConnectionString = fn.Value,
                        DatabaseTables = new List<DatabaseTable>()
                    };
                    //Add new tables to new database
                    currentdatabase.DatabaseTables = addTable(currentdatabase.DatabaseConnectionString);

                    //for the selected format
                    selectedformat.BartenderDatabases.Add(currentdatabase);
                    repository.Save();
                }
                else
                {
                    showMessageBox("Wrong database extention. only '.mdb' allowed.");
                }
            }
        }

        public List<DatabaseTable> addTable(string databaseconnectionstring)
        {
            List<DatabaseTable> listoftables = new List<DatabaseTable>();
            string connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + databaseconnectionstring;
            using (OleDbConnection conn = new OleDbConnection(connStr))
            {
                conn.Open();
                DataTable schemaTable = conn.GetSchema("Tables");
                foreach (DataRow row in schemaTable.Rows)
                {
                    if (row["TABLE_TYPE"].ToString() != "TABLE")
                    {
                        row.Delete();
                    }
                    if (row.RowState != DataRowState.Deleted)
                    {
                        DatabaseTable currenttable = new DatabaseTable() { TableName = row["TABLE_NAME"].ToString(), DatabaseColumns = new List<DatabaseColumn>() };
                        currenttable.DatabaseColumns = addColumn(databaseconnectionstring, currenttable);
                        listoftables.Add(currenttable);
                        currenttable = null;
                    }
                }
            }
            return listoftables;
        }

        public List<DatabaseColumn> addColumn(string databaseconnectionstring, DatabaseTable selectedtable)
        {
            List<DatabaseColumn> listofcolumns = new List<DatabaseColumn>();
            string connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + databaseconnectionstring;
            using (OleDbConnection conn = new OleDbConnection(connStr))
            {
                conn.Open();
                OleDbCommand command = new OleDbCommand("SELECT * FROM [" + selectedtable.TableName + "]", conn);
                using (var reader = command.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var table = reader.GetSchemaTable();
                    var nameCol = table.Columns["ColumnName"];
                    foreach (DataRow row in table.Rows)
                    {
                        if (row[nameCol].ToString() != "ID")
                        {
                            listofcolumns.Add(new DatabaseColumn() { ColumnName = row[nameCol].ToString() });
                        }
                    }
                }
            }
            return listofcolumns;
        }

        public void addLink(IRepository repository, BartenderFormat selectedformat, BartenderSubString selectedsubstring, BartenderDatabase selecteddatabase, DatabaseTable selectedtable, DatabaseColumn selectedcolumn)
        {
            int ssresult = repository.Get<Links>().Where(w => w.BartenderSubString.Id == selectedsubstring.Id).Count();
            int cresult = repository.Get<Links>().Where(w => w.DatabaseColumn.Id == selectedcolumn.Id).Count();

            if (ssresult == 0 && cresult == 0)
            {
                Links newlink = new Links()
                {
                    BartenderFormat = selectedformat,
                    BartenderSubString = selectedsubstring,
                    BartenderDatabase = selecteddatabase,
                    DatabaseTable = selectedtable,
                    DatabaseColumn = selectedcolumn
                };
                repository.Add(newlink);
                repository.Save();
            }
            else
            {
                showMessageBox("Sorry this link is already assigned");
            }
        }

        public void deleteLink(IRepository repository, int id)
        {
            repository.Delete(repository.Get<Links>().Where(w => w.Id == id).First());
            repository.Save();
        }

        public Dictionary<string, string> openFileBox(string filter, bool multiselect)
        {
            OpenFileDialog ofdl = new OpenFileDialog();
            ofdl.Filter = filter;
            ofdl.Multiselect = multiselect;
            ofdl.InitialDirectory = "C:\\";
            Dictionary<string, string> results = new Dictionary<string, string>();
            bool? ofdlresult = ofdl.ShowDialog();
            if (ofdlresult == true)
            {
                foreach (string fn in ofdl.FileNames)
                {
                    results.Add(Path.GetFileName(fn), Path.GetFullPath(fn));
                }
            }
            return results;
        }

        public void showMessageBox(string message)
        {
            MessageBox.Show(message, "Label Printing with Bartender");
        }

        public void exceptionHandling(string message, Exception err)
        {
            MessageBox.Show(message + ":" + err.Message, "ERROR - Label Printing with Bartender");
        }

        public void Dispose()
        {
        }
    }
}
