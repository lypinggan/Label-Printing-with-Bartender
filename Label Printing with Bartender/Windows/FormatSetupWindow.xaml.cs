using Label_Printing_with_Bartender.Entities;
using Label_Printing_with_Bartender.Repository;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Data.Entity;
using Label_Printing_with_Bartender.Services;

namespace Label_Printing_with_Bartender
{
    public partial class FormatSetupWindow : Window
    {
        #region Fields
        //selected objects
        private BartenderFormat selectedformat { get; set; }
        private BartenderSubString selectedsubstring { get; set; }
        private BartenderDatabase selecteddatabase { get; set; }
        private DatabaseTable selectedtable { get; set; }
        private DatabaseColumn selectedcolumn { get; set; }
        private Links selectedlink { get; set; }

        //Interfaces.
        /// <summary>
        /// Used for error handling and common methods
        /// </summary>
        private IServices services { get; set; }
        /// <summary>
        /// Link to EF Database.
        /// </summary>
        private IRepository repository { get; set; }


        #endregion

        #region Constructor
        public FormatSetupWindow(IRepository repositoryin)
        {
            InitializeComponent();
            repository = repositoryin;
            services = new Services.Services();
        }
        #endregion

        #region Form Event Handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            updateFormatGrid();
        }

        private void bAddFMT_Click(object sender, RoutedEventArgs e)
        {
            services.addFMT(repository);
            updateFormatGrid();
        }

        private void bRemoveFMT_Click(object sender, RoutedEventArgs e)
        {
            if (selectedformat != null)
            {
                int id = selectedformat.Id;
                services.deleteFMT(repository, id);
                selectedformat = null;
                updateFormatGrid();
                updateDatabaseGrid();
            }
            else
            {
                services.showMessageBox("Sorry you didn't select a format to remove.");
            }
        }

        private void bAddDB_Click(object sender, RoutedEventArgs e)
        {
            if (selectedformat != null)
            {
                gDatabases.ItemsSource = null;
                services.addBartenderDatabase(repository, selectedformat);
                gDatabases.ItemsSource = repository.Get<BartenderFormat>().
                    Where(w => w.Id == selectedformat.Id).
                    SelectMany(s => s.BartenderDatabases).ToList();
            }
            else
            {
                services.showMessageBox("Sorry you haven't selected a format to link the database to.");
            }
        }

        private void bRemoveDB_Click(object sender, RoutedEventArgs e)
        {
            if (selecteddatabase != null)
            {
                services.deleteDB(repository, selecteddatabase.Id);
                selecteddatabase = null;
                updateDatabaseGrid();
            }
            else
            {
                services.showMessageBox("Sorry you didn't select a database to remove.");
            }
        }

        private void bAddLink_Click(object sender, RoutedEventArgs e)
        {
            if (selectedsubstring != null)
            {
                if (selectedcolumn != null)
                {
                    services.addLink(repository, selectedformat, selectedsubstring, selecteddatabase, selectedtable, selectedcolumn);
                    updateLinks();
                }
                else
                {
                    services.showMessageBox("No Column Selected");
                }
            }
            else
            {
                services.showMessageBox("No SubString Selected");
            }
        }

        private void bRemoveLink_Click(object sender, RoutedEventArgs e)
        {
            if (selectedlink != null)
            {
                services.deleteLink(repository, selectedlink.Id);
                selectedlink = null;
                updateLinks();
                gLinks.SelectedIndex = 0;
            }
            else
            {
                services.showMessageBox("No Link selected.");
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
                selectedformat = repository.Get<BartenderFormat>().
                    Where(w => w.Id == id).
                    Include(i => i.SubStrings).
                    Include(i => i.BartenderDatabases).
                    First();
                try
                {
                    updateSubStringGrid();
                    updateDatabaseGrid();
                    updateLinks();
                }
                catch (Exception err)
                {
                    services.exceptionHandling("Format grid load failed", err);
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

        private void updateFormatGrid()
        {
            gLabelsGrid.ItemsSource = repository.Get<BartenderFormat>().ToList();
            updateSubStringGrid();
            updateLinks();
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
        #endregion

    }
}
