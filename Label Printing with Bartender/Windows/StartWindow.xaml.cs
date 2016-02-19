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
        LabelFormatDocument openformat = null; // The currently open Format

        //selected objects
        BartenderFormat selectedformat;

        //DB
        DB db = new DB();

        //Services
        private IRepository repository;
        private IServices services;

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
            Printers printers = new Printers();
            foreach (Printer printer in printers)
            {
                cboPrinters.Items.Add(printer.PrinterName);
            }

            if (printers.Count > 0)
            {
                cboPrinters.SelectedItem = printers.Default.PrinterName;
            }
            updateFormatGrid();
        }

        private void bFormatSetup_Click(object sender, RoutedEventArgs e)
        {
            FormatSetupWindow dssw = new FormatSetupWindow(repository);
            WindowState = WindowState.Minimized;
            bool? result = dssw.ShowDialog();
            if (result == false)
            {
                WindowState = WindowState.Normal;
                updateFormatGrid();
            }
        }

        private void gLabelsGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            selectedformat = (BartenderFormat)gLabelsGrid.SelectedItem;
            selectedformat = repository.Get<BartenderFormat>().Where(w => w.Id == selectedformat.Id).First();
        }

        private void updateFormatGrid()
        {
            gLabelsGrid.ItemsSource = repository.Get<BartenderFormat>().ToList();
        }

        private void bPrint_Click(object sender, RoutedEventArgs e)
        {
            if (selectedformat != null)
            {
                openformat = services.openBartenderFormat(selectedformat.FormatPath, services.runBartenderEngine());
                if (cboPrinters.SelectedItem != null)
                    openformat.PrintSetup.PrinterName = cboPrinters.SelectedItem.ToString();
                Result result = openformat.Print();
                switch (result)
                {
                    case Result.Success:
                        services.showMessageBox("Print sucessfull");
                        break;
                    case Result.Timeout:
                        services.showMessageBox("Print timed out, please check Bartender runs manually and try again.");
                        break;
                    case Result.Failure:
                        services.showMessageBox("Print failed, this could be due to licence server issues.");
                        break;
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
