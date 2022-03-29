using GlobalUtilityLibrary;
using PitchreadyExcel.ViewModel;
using PitchreadyGlobal.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for UploadTemplateView.xaml
    /// </summary>
    public partial class UploadTemplateView : Window
    {
        public UploadTemplateView(ParentCategoryModel parentObject)
        {
            InitializeComponent();
            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight * 0.7);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth * 0.6);
            GlobalUtility.LogError("Inside Upload Template View");
            Header.HeaderColor = "#F3F2F0";

            UploadTemplateViewModel uploadTemplateViewModel = new UploadTemplateViewModel();
            uploadTemplateViewModel.parentid = Convert.ToInt32(parentObject.Id);
            this.DataContext = uploadTemplateViewModel;
            uploadTemplateViewModel.RequestReactivateForm += () =>
            {
                Activate();
                Focus();
            };

            uploadTemplateViewModel.RequestClose += () =>
            {
                Close();
                uploadTemplateViewModel.CloseExcel();
                uploadTemplateViewModel.FixedDocumentSequenceVar = null;
                if (Directory.Exists(System.IO.Path.GetTempPath() + "\\" + "TemplateRepositoryXPS"))
                {
                    try
                    {
                        Directory.Delete(System.IO.Path.GetTempPath() + "\\" + "TemplateRepositoryXPS", true);
                    }
                    catch
                    {

                    }
                    finally
                    {
                        Globals.ThisAddIn.Application.DisplayAlerts = true;
                        Globals.ThisAddIn.Application.Visible = true;
                        Globals.ThisAddIn.Application.ScreenUpdating = true;
                    }
                }
            };
        }

        private void Button_ClickNext(object sender, RoutedEventArgs e)
        {

        }

        private void Button_ClickPrevious(object sender, RoutedEventArgs e)
        {

        }
    }
}
