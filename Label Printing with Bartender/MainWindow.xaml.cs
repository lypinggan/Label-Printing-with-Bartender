using Label_Printing_with_Bartender.Entities;
using Label_Printing_with_Bartender.Service;
using Label_Printing_with_Bartender.Context;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Data.Entity;

namespace Label_Printing_with_Bartender
{
    public partial class MainWindow : System.Windows.Window
    {
        #region Fields
        //Common Strings
        private const string appName = "Label Printing with Bartender";
        Seagull.BarTender.Print.Engine engine = null; // The BarTender Print Engine
        Seagull.BarTender.Print.LabelFormatDocument openformat = null; // The currently open Format
        SplashScreen splashScreen = new SplashScreen("\\splashscreen.jpg");

        //selected objects
        BartenderFormat selectedformat;
        BartenderSubString selectedsubstring;
        BartenderDatabase selecteddatabase;
        DatabaseTable selectedtable;
        DatabaseColumn selectedcolumn;
        Links selectedlink;

        //DB
        DB db = new DB();

        //Service
        private IRepository repository;

        #endregion

        #region Constructor
        public MainWindow()
        {
            WindowState = WindowState.Minimized;
            splashScreen.Show(true);
            InitializeComponent();
            this.repository = new Repository(db);
        }
        #endregion

        #region Delegates
        delegate void DelegateShowMessageBox(string message);
        #endregion

        #region Form Event Handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                engine = new Seagull.BarTender.Print.Engine(true);
            }
            catch (Exception exception)
            {
                // If the engine is unable to start, a PrintEngineException will be thrown.
                System.Windows.MessageBox.Show(this, exception.Message, appName);
                this.Close(); // Close this app. We cannot run without connection to an engine.
                return;
            }
            //After BT engine loaded, update the format grid from the entity database
            updateFormatGrid();
            WindowState = WindowState.Normal;
            splashScreen.Show(false);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Quit the BarTender Print Engine, but do not save changes to any open formats.
            if (engine != null)
                engine.Stop(Seagull.BarTender.Print.SaveOptions.DoNotSaveChanges);
        }

        private void bAddFMT_Click(object sender, RoutedEventArgs e)
        {
            gLabelsGrid.ItemsSource = null;
            Dictionary<string, string> results = openFileBox("Bartender Files|*.btw", true);
            foreach (KeyValuePair<string, string> fn in results)
            {
                selectedformat = new BartenderFormat()
                {
                    FormatName = fn.Key,
                    FormatPath = fn.Value,
                    SubStrings = new List<BartenderSubString>()
                };
                openFormat();
                if (openformat != null)
                {
                    foreach (var ss in openformat.SubStrings)
                    {
                        selectedformat.SubStrings.Add(new BartenderSubString()
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
                this.repository.Add(selectedformat);
                this.repository.Save();
            }
            updateFormatGrid();
        }

        private void bRemoveFMT_Click(object sender, RoutedEventArgs e)
        {
            if (selectedformat != null)
            {
                int id = selectedformat.Id;
                //Disconnect Bartdner from the format before deleting.
                openformat = null;
                selectedformat = repository.Get<BartenderFormat>().Where(w => w.Id == id).Include(i => i.BartenderDatabases).First();
                //delete formats attached to the selectedformat
                foreach (BartenderDatabase btdb in selectedformat.BartenderDatabases.ToList())
                {
                    deleteDB(btdb.Id);
                }
                //Remove the format
                repository.Delete(selectedformat);
                repository.Save();
                selectedformat = null;
                //update the grids (cascade)
                updateFormatGrid();
                updateDatabaseGrid();
            }
            else
            {
                ShowMessageBox("Sorry you didn't select a format to remove.");
            }
        }

        private void bAddDB_Click(object sender, RoutedEventArgs e)
        {
            if (selectedformat != null)
            {
                gDatabases.ItemsSource = null;

                addBartenderDatabase();

                gDatabases.ItemsSource = repository.Get<BartenderFormat>().Where(w => w.Id == selectedformat.Id).SelectMany(s => s.BartenderDatabases).ToList();
            }
            else
            {
                ShowMessageBox("Sorry you haven't selected a format to link the database to.");
            }
        }

        private void bRemoveDB_Click(object sender, RoutedEventArgs e)
        {
            if (selecteddatabase != null)
            {
                deleteDB(selecteddatabase.Id);
                selecteddatabase = null;
            }
            else
            {
                ShowMessageBox("Sorry you didn't select a database to remove.");
            }
        }

        private void bAddLink_Click(object sender, RoutedEventArgs e)
        {
            if (selectedsubstring != null)
            {
                if (selectedcolumn != null)
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
                        updateLinks();
                    }
                    else
                    {
                        
                        selectedlink = repository.Get<Links>().Where(w => w.BartenderSubString.Id == selectedsubstring.Id).First();
                        
                        ShowMessageBox("Sorry this link is already assigned");
                    }
                }
                else
                {
                    ShowMessageBox("No Column Selected");
                }
            }
            else
            {
                ShowMessageBox("No SubString Selected");
            }
        }

        private void bRemoveLink_Click(object sender, RoutedEventArgs e)
        {
            if (selectedlink != null)
            {
                repository.Delete(repository.Get<Links>().Where(w => w.Id == selectedlink.Id).First());
                repository.Save();
                selectedlink = null;
                updateLinks();
                gLinks.SelectedIndex = 0;
            }
            else
            {
                ShowMessageBox("No Link selected.");
            }
        }

        #endregion

        #region DataGrid Event Handler

        private void gLabelsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedformat = (BartenderFormat)gLabelsGrid.SelectedItem;
            if (selectedformat != null)
            {
                int id = selectedformat.Id;
                selectedformat = repository.Get<BartenderFormat>().Where(w => w.Id == id).Include(i => i.BartenderDatabases).First();
                try
                {
                    openFormat();
                    updateSubStringGrid();
                    updateDatabaseGrid();
                    updateLinks();
                }
                catch (Exception err)
                {
                    ExceptionHandling(err);
                }
            }
        }

        private void gNamedSubstrings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedsubstring = (BartenderSubString)gNamedSubstrings.SelectedItem;
        }

        private void gDatabases_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selecteddatabase = (BartenderDatabase)gDatabases.SelectedItem;
            if (selecteddatabase != null)
            {
                updateTableGrid();
            }
        }

        private void gTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedtable = (DatabaseTable)gTables.SelectedItem;
            updateColumnGrid();
        }

        private void gColumns_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedcolumn = (DatabaseColumn)gColumns.SelectedItem;
        }

        private void gLinks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedlink = (Links)gLinks.SelectedItem;
        }

        #endregion

        #region Methods

        private void openFormat()
        {
            //Due to Bartender slowness I change the cursor to wait and back again.
            Mouse.OverrideCursor = Cursors.Wait;
            openformat = engine.Documents.Open(selectedformat.FormatPath);
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void addBartenderDatabase()
        {
            //open file dialog to get users database
            Dictionary<string, string> results = openFileBox("Access Database Files|*.mdb", false);
            foreach (KeyValuePair<string, string> fn in results)
            {
                //for each database user selectes make a new database in EF
                selecteddatabase = new BartenderDatabase()
                {
                    DatabaseName = fn.Key,
                    DatabaseConnectionString = fn.Value,
                    DatabaseTables = new List<DatabaseTable>()
                };
                //Add new tables to new database
                selecteddatabase.DatabaseTables = addTable();

                //for the selected format 
                selectedformat.BartenderDatabases.Add(selecteddatabase);
                repository.Save();
            }
        }

        private List<DatabaseTable> addTable()
        {
            List<DatabaseTable> listoftables = new List<DatabaseTable>();
            string connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + selecteddatabase.DatabaseConnectionString;
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
                        selectedtable = new DatabaseTable() { TableName = row["TABLE_NAME"].ToString(), DatabaseColumns = new List<DatabaseColumn>() };
                        selectedtable.DatabaseColumns = addColumn();
                        listoftables.Add(selectedtable);
                        selectedtable = null;
                    }
                }
            }
            return listoftables;
        }

        private List<DatabaseColumn> addColumn()
        {
            List<DatabaseColumn> listofcolumns = new List<DatabaseColumn>();
            string connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + selecteddatabase.DatabaseConnectionString;
            using (OleDbConnection conn = new OleDbConnection(connStr))
            {
                conn.Open();
                OleDbCommand Command = new OleDbCommand("select * from [" + selectedtable.TableName + "]", conn);
                using (var reader = Command.ExecuteReader(CommandBehavior.SchemaOnly))
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

        private void deleteDB(int id)
        {
            repository.Delete(repository.Get<BartenderDatabase>().Where(w => w.Id == id).Include(i => i.DatabaseTables.Select(s => s.DatabaseColumns)).First());
            repository.Save();
            updateDatabaseGrid();
        }

        private void updateFormatGrid()
        {
            gLabelsGrid.ItemsSource = repository.Get<BartenderFormat>().ToList();
            updateSubStringGrid();
        }

        private void updateSubStringGrid()
        {
            if (selectedformat != null)
            {
                gNamedSubstrings.ItemsSource = repository.Get<BartenderFormat>().Where(w => w.Id == selectedformat.Id).SelectMany(s => s.SubStrings).ToList();
            }
            else
            {
                gNamedSubstrings.ItemsSource = null;
            }
        }

        private void updateDatabaseGrid()
        {
            if (selectedformat != null)
            {
                gDatabases.ItemsSource = repository.Get<BartenderFormat>().Where(w => w.Id == selectedformat.Id).SelectMany(s => s.BartenderDatabases).ToList();
            }
            else
            {
                gDatabases.ItemsSource = null;
            }
            updateTableGrid();
        }

        private void updateTableGrid()
        {
            if (selecteddatabase != null)
            {
                gTables.ItemsSource = repository.Get<BartenderDatabase>().Where(w => w.Id == selecteddatabase.Id).SelectMany(s => s.DatabaseTables).ToList();
                updateColumnGrid();
            }
            else
            {
                gTables.ItemsSource = null;
            }
        }

        private void updateColumnGrid()
        {
            if (selectedtable != null)
            {
                gColumns.ItemsSource = repository.Get<DatabaseTable>().Where(w => w.Id == selectedtable.Id).SelectMany(s => s.DatabaseColumns).ToList();
            }
            else
            {
                gColumns.ItemsSource = null;
            }
        }

        private void updateLinks()
        {
            if (selectedformat != null)
            {
                gLinks.ItemsSource = repository.Get<Links>().
                    Where(w => w.BartenderFormat.Id == selectedformat.Id).
                    Include(i => i.BartenderFormat).
                    Include(i => i.BartenderSubString).
                    Include(i => i.BartenderDatabase).
                    Include(i => i.DatabaseTable).
                    Include(i => i.DatabaseColumn).
                    ToList();
            }
            else
            {
                gLinks.ItemsSource = null;
            }
        }

        private Dictionary<string, string> openFileBox(string filter, bool multiselect)
        {
            OpenFileDialog ofdl = new OpenFileDialog();

            ofdl.Filter = filter;
            ofdl.Multiselect = multiselect;
            ofdl.InitialDirectory = "C:\\";

            Dictionary<string, string> results = new Dictionary<string, string>();

            Nullable<bool> ofdlresult = ofdl.ShowDialog();

            if (ofdlresult == true)
            {
                foreach (string fn in ofdl.FileNames)
                {
                    results.Add(System.IO.Path.GetFileName(fn), System.IO.Path.GetFullPath(fn));
                }
            }
            return results;
        }

        void ShowMessageBox(string message)
        {
            System.Windows.MessageBox.Show(this, message, appName);
        }

        void ExceptionHandling(Exception err)
        {
            ShowMessageBox(err.Message);
            if (engine != null)
                engine.Stop(Seagull.BarTender.Print.SaveOptions.DoNotSaveChanges);
        }

        #endregion
    }
}
