using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Label_Printing_with_Bartender.Context;
using Label_Printing_with_Bartender.Entities;
using Label_Printing_with_Bartender.Repository;
using Label_Printing_with_Bartender.Services;
using Seagull.BarTender.Print;
using System.Data.Entity;

namespace Label_Printing_with_Bartender.Windows
{
    /// <summary>
    /// Interaction logic for PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : System.Windows.Window
    {
        //Interfaces.
        /// <summary>
        /// Used for error handling and common methods
        /// </summary>
        private IServices services { get; set; }
        /// <summary>
        /// Link to EF Database.
        /// </summary>
        private IRepository repository { get; set; }
        /// <summary>
        /// Bartender Format in EF.
        /// </summary>
        private BartenderFormat format { get; set; }

        string printername;

        public PrintWindow(IRepository repositoryin, IServices servicesin, BartenderFormat formatin, string printernamein)
        {
            InitializeComponent();
            repository = repositoryin;
            services = servicesin;
            format = repository.Get<BartenderFormat>().Where(w => w.Id == formatin.Id).FirstOrDefault();
            gLinks.ItemsSource = repository.Get<Links>().
                    Where(w => w.BartenderFormat.Id == format.Id).
                    Include(i => i.BartenderFormat).
                    Include(i => i.BartenderSubString).
                    Include(i => i.BartenderDatabase).
                    Include(i => i.DatabaseTable).
                    Include(i => i.DatabaseColumn).
                    ToList();
            printername = printernamein;
            lFormat.Content = format.FormatName;
            lPrinter.Content = printername;
        }

        private void bPrint_Click(object sender, RoutedEventArgs e)
        {
            LabelFormatDocument openformat = services.openBartenderFormat(format.FormatPath, services.runBartenderEngine());
            openformat.PrintSetup.PrinterName = printername;
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
    }
}
