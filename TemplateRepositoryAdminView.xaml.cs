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
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace PitchreadyGlobal.Views
{
    /// <summary>
    /// Interaction logic for TemplateRepositoryAdmin.xaml
    /// </summary>
    public partial class TemplateRepositoryAdminView : Window
    {
        TemplateRepositoryAdminViewModel editTemplateRepository_VM = new TemplateRepositoryAdminViewModel();
        public TemplateRepositoryAdminView()
        {
            InitializeComponent();
            Header.HeaderColor = "#F3F2F0";

            this.DataContext = editTemplateRepository_VM;
            editTemplateRepository_VM.ExpandAllNodes += EditTemplateRepository_VM_ExpandAllNodes;
            //ApplyTheme();
            //InitializeComponent();
            this.MinHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 0.80;
            this.MinWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 0.80;
            //CenterWindowOnScreen();
            //GlobalUtility.SetFormTitle(this);
            editTemplateRepository_VM.RequestClose += () => { Close(); };
            //string logoPath = GlobalUtility.GetClientLogoPath(GlobalUtilityLibrary.GlobalUtility.ClientNameForEvalueServerDatapopulation);
            //if (!string.IsNullOrEmpty(logoPath))
            //    imgClient.Source = new BitmapImage(new System.Uri(logoPath));
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EditTemplateRepository_VM_ExpandAllNodes(object sender, Microsoft.Exchange.WebServices.Data.NotificationEventArgs e)
        {
            CategoryDataTree.ExpandAllHierarchyItems();
            //CategoryDataTree.ExpandAllGroups();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RadToggleSwitchButton_Checked(object sender, RoutedEventArgs e)
        {
            var obj = sender as RadToggleSwitchButton;

            var dataContext = obj.DataContext as ParentCategoryModel;

            editTemplateRepository_VM.UpdateCategoryTRAdmin(dataContext.Id.Value, obj.IsChecked.Value, dataContext.CategoryName, dataContext.CategoryIconName);
        }

        private void RadToggleSwitchButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var obj = sender as RadToggleSwitchButton;

            var dataContext = obj.DataContext as ParentCategoryModel;

            editTemplateRepository_VM.UpdateCategoryTRAdmin(dataContext.Id.Value, obj.IsChecked.Value, dataContext.CategoryName, dataContext.CategoryIconName);

        }
    }
}
