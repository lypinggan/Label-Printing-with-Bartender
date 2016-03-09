using Label_Printing_with_Bartender.Context;
using Label_Printing_with_Bartender.Entities;
using Label_Printing_with_Bartender.Repository;
using Label_Printing_with_Bartender.Services;
using Seagull.BarTender.Print;
using System;
using System.Data;
using System.Linq;
using System.Windows;

namespace Label_Printing_with_Bartender.Windows
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : System.Windows.Window, IDisposable
    {
        private const string appName = "Label Printing with Bartender";

        //DB
        DB db = new DB("LPWB_Database");

        //Services
        public IRepository repository;
        public IServices services;

        public StartWindow()
        {
            InitializeComponent();
            try
            {
                services = new Services.Services();
                if (services.checkBartenderEngineStarts())
                {
                    repository = new Repository.Repository(db);
                    services.fileExistsCheck(repository);
                }
            }
            catch (Exception err)
            {
                services.showMessageBox(err.Message);
                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            updateFormatGrid();
            Printers printers = new Printers();
            foreach (Printer printer in printers)
            {
                cboPrinters.Items.Add(printer.PrinterName);
            }

            if (printers.Count > 0)
            {
                cboPrinters.SelectedItem = printers.Default.PrinterName;
            }
        }

        private void bFormatSetup_Click(object sender, RoutedEventArgs e)
        {
            FormatSetupWindow dssw = new FormatSetupWindow(repository, services);
            WindowState = WindowState.Minimized;
            bool? result = dssw.ShowDialog();
            if (result == false)
            {
                WindowState = WindowState.Normal;
            }
            updateFormatGrid();
        }

        private void updateFormatGrid()
        {
            gLabelsGrid.ItemsSource = repository.Get<BartenderFormat>().ToList();
        }

        private void bPrint_Click(object sender, RoutedEventArgs e)
        {
            if (gLabelsGrid.SelectedIndex >= 0)
            {
                PrintWindow pw = new PrintWindow(repository, services, (BartenderFormat)gLabelsGrid.SelectedItem, cboPrinters.SelectedItem.ToString());
                WindowState = WindowState.Minimized;
                bool? result = pw.ShowDialog();
                if (result == false)
                {
                    WindowState = WindowState.Normal;
                }
            }
            else
            {
                services.showMessageBox("Sorry you have not selected a format from the list.");
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    services.Dispose();
                    db.Dispose();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion


    }
}
