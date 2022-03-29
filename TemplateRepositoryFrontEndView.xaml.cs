using GlobalUtilityLibrary;
using GlobalUtilityLibrary.Helpers;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using PitchreadyExcel.Utility;
using PitchreadyExcel.ViewModel;
using PitchreadyGlobal.Enumrations;
using PitchreadyGlobal.Helpers;
using PitchreadyGlobal.UserControls;
using PitchreadyGlobal.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using Application = Microsoft.Office.Interop.Excel.Application;
using Constants = GlobalUtilityLibrary.Helpers.Constants;

namespace PitchreadyExcel.Forms
{
    /// <summary>
    /// Interaction logic for TemplateRepositoryFrontEndView.xaml
    /// </summary>
    public partial class TemplateRepositoryFrontEndView : System.Windows.Window
    {
        TemplateRepositoryFrontEndViewModel viewModel;
        List<ParentCategoryModel> TemplateCatagoryList = new List<ParentCategoryModel>();
        List<ParentCategoryModel> ParentCategoryList = new List<ParentCategoryModel>();
        List<ChildCategoryModel> TemplateDataList = new List<ChildCategoryModel>();
        List<ParentCategoryModel> ItemData = new List<ParentCategoryModel>();
        int rootCategoryID;

        public TemplateRepositoryFrontEndView(int RootCategoryID)
        {
            InitializeComponent();

            this.rootCategoryID = RootCategoryID;
            viewModel = new TemplateRepositoryFrontEndViewModel(rootCategoryID, treeViewCategory, selectAllPreviewCheckBox);
            List<string> importTypeItems = new List<string>();

            this.DataContext = viewModel;
            lstCartItems.ItemsSource = viewModel.CartList;
            Header.HeaderColor = "#F3F2F0";
            this.MinHeight = (System.Windows.SystemParameters.PrimaryScreenHeight * 0.80);
            this.MinWidth = (System.Windows.SystemParameters.PrimaryScreenWidth * 0.80);
            CenterWindowOnScreen();
            Utils.SetFormTitle(this);

            KeyDown += Window_KeyDown;
            Activated += Window_Activated;
            Deactivated += TemplateRepositoryFrontEndView_Deactivated;
            Loaded += Window_Loaded;
            Closing += Window_Closing;
            Closed += TemplateRepositoryFrontEndView_Closed;
            TemplateRepositoryFrontEndViewModel.NotifyViewEventToUncheck += TemplateRepositoryFrontEndViewModel_NotifyViewEvent;
            TemplateRepositoryFrontEndViewModel.NotifyViewEventToCheck += TemplateRepositoryFrontEndViewModel_NotifyViewEventToCheck;

            //Although name differs since the event was present already so defining as per my need
            viewModel.RequestClose += () =>
            {
                //Activate();
                //Focus();
                Close();
                Globals.ThisAddIn.Application.DisplayAlerts = true;
            };
        }

        private void TemplateRepositoryFrontEndView_Deactivated(object sender, EventArgs e)
        {
           // Globals.ThisAddIn.Application.Interactive = true;
        }

        private void TemplateRepositoryFrontEndViewModel_NotifyViewEventToCheck(object sender, EventArgs e)
        {
            selectAllPreviewCheckBox.Checked -= CheckBox_Checked;
            selectAllPreviewCheckBox.IsChecked = true;
            selectAllPreviewCheckBox.Checked += CheckBox_Checked;
        }

        private void TemplateRepositoryFrontEndViewModel_NotifyViewEvent(object sender, EventArgs e)
        {
            selectAllPreviewCheckBox.Unchecked -= CheckBox_Unchecked;
            selectAllPreviewCheckBox.IsChecked = false;
            selectAllPreviewCheckBox.Unchecked += CheckBox_Unchecked;
        }

        private void TemplateRepositoryFrontEndView_Closed(object sender, EventArgs e)
        {
            //ThreadHandler.threadExists = false;
            //ThreadHandler.form.Dispatcher.InvokeShutdown();
            Close();
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //viewModel.LoadingText = "Loading Category...";

            //await viewModel.OnWindowLoaded();
            treeViewCategory.ExpandAll();
        }


        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                var res = CustomMessageBoxExcel.Show("Do you want to close the form?", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.ConfirmationWithOkCancel, ExcelMessageBoxImage.Information);
                if (!res)
                {
                    return;
                }
                this.Close();
                return;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //var res = CustomMessageBoxExcel.Show("Do you want to close the form?", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.ConfirmationWithOkCancel, ExcelMessageBoxImage.Information);
            //if (!res)
            //{
            //    e.Cancel = true;
            //    return;
            //}
            viewModel.Dispose();
        }

        private void updateCartCount()
        {
            //btnSelectAllCart.IsEnabled = CartList.Count() > 0;
            //CartCount = CartList.Count().ToString("00");
        }

        private void updatePreviewCount()
        {
            //btnSelectallPreview.IsEnabled = templateDataSource.Count() > 0;
            //if (string.IsNullOrEmpty(searchCatagories.Text))
            //{
            //    btnclearAllTreeSelection.IsEnabled = templateDataSource.Count() > 0;
            //}
            //PreviewCount = templateDataSource.Count().ToString("00");
        }

        private void treeViewCategory_Expanded(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            RadTreeViewItem current = e.OriginalSource as RadTreeViewItem;
            if (current == null) return;
            var currentdata = current.DataContext as ParentCategoryModel;
            if (currentdata == null) return;
            currentdata.nodeImage = new BitmapImage(new Uri("/PitchreadyPowerPoint;component/Media/Icons/FolderImg.png", UriKind.Relative));
        }

        private void treeViewCategory_Collapsed(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            RadTreeViewItem current = e.OriginalSource as RadTreeViewItem;
            if (current == null) return;
            var currentdata = current.DataContext as ParentCategoryModel;
            if (currentdata == null) return;
            currentdata.nodeImage = new BitmapImage(new Uri("/PitchreadyPowerPoint;component/Media/Icons/FolderCollapse.png", UriKind.Relative));
        }

        private void treeViewCategory_Selected(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (treeViewCategory.SelectedItem != null)
            {
                RadTreeViewItem current = e.OriginalSource as RadTreeViewItem;
                current.IsSelected = false;
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            GlobalUtility.SetResourcesForTool(GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);

            //Globals.ThisAddIn.Application.Interactive = false;
        }

        private void WorkbookInfoGrid_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (WorkbookInfoGrid.CurrentCell != null && e.Key == Key.Space)
            {
                var row = WorkbookInfoGrid.CurrentCell.Content;
                if (row is System.Windows.Controls.CheckBox)
                {
                    (row as System.Windows.Controls.CheckBox).IsChecked = !(row as System.Windows.Controls.CheckBox).IsChecked;
                    e.Handled = true; //this is necessary because otherwise when the checkbox cell is selected, it will apply this keyup and also apply the default behavior for the checkbox
                }
            }
        }

        private void WorkbookInfoGrid_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            if (viewModel.clearCart)
            {
                if (e.AddedItems.Count > 0)
                {
                    if (e.AddedItems.Count == 1)
                        viewModel.SelectionChangedCommandHandler(e.AddedItems.FirstOrDefault() as TemplateRepoFrontEndModel, "AddToCart");
                    else
                        viewModel.SelectionChangedCommandHandlerMultipleChecks(e.AddedItems.ToList(), "AddToCart");
                }

                if (e.RemovedItems.Count > 0)
                {
                    if (e.RemovedItems.Count == 1)
                        viewModel.SelectionChangedCommandHandler(e.RemovedItems.FirstOrDefault() as TemplateRepoFrontEndModel, "RemoveFromCart");
                    else
                        viewModel.SelectionChangedCommandHandlerMultipleChecks(e.RemovedItems.ToList(), "RemoveFromCart");
                }
            }
            viewModel.clearCart = true;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            searchCatagories.Text = string.Empty;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            viewModel.CheckAllPreviewsForCart();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            viewModel.UncheckAllPreviews();
        }
    }
}
