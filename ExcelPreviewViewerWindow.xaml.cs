using PitchreadyExcel.ViewModel;
using PitchreadyGlobal.ViewModels;
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
    /// Interaction logic for ExcelPreviewViewerWindow.xaml
    /// </summary>
    public partial class ExcelPreviewViewerWindow : Window
    {
        public ExcelPreviewViewerWindow(ChildCategoryModel parentObject)
        {
            InitializeComponent();

            Header.HeaderColor = "#F3F2F0";
            this.MinHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            this.MinWidth = System.Windows.SystemParameters.PrimaryScreenWidth;

            EditTemplateViewModel uploadTemplateViewModel = new EditTemplateViewModel(true);
            uploadTemplateViewModel.WorksheetCount = parentObject.WorksheetCount.Value;

            uploadTemplateViewModel.previousDescription = uploadTemplateViewModel.Description;
            uploadTemplateViewModel.previousFileName = uploadTemplateViewModel.FileName;
            uploadTemplateViewModel.previousSaveAsName = uploadTemplateViewModel.SaveAsName;
            uploadTemplateViewModel.previousTemplatePath = uploadTemplateViewModel.TemplatePath;

            uploadTemplateViewModel.parentid = Convert.ToInt32(parentObject.CategoryID);
            uploadTemplateViewModel.templateWorkbookId = Convert.ToInt32(parentObject.TemplateWorkbookId);
            PreviewViewer.DataContext = uploadTemplateViewModel;
        }
    }
}
