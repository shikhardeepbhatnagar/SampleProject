using PitchreadyExcel.ViewModel.LibraryTool;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PitchreadyExcel.Forms
{
    /// <summary>
    /// Interaction logic for ExcelAdminWindow.xaml
    /// </summary>
    public partial class ExcelAdminWindow : Window
    {
        ExcelAdminWindowVM objVM = null;
        public ExcelAdminWindow()
        {
            InitializeComponent();
            objVM = new ExcelAdminWindowVM();
            this.DataContext = objVM;
        }
    }
}
