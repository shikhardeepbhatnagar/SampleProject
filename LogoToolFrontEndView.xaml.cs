using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using PitchreadyGlobal.ApplicationConstants;
using PitchreadyGlobal.DependencyServices;
using PitchreadyGlobal.Enumrations;
using PitchreadyGlobal.Helpers;
using PitchreadyGlobal.Models;
using PitchreadyGlobal.UserControls;
using PitchreadyGlobal.ViewModels;
using Prism.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Xml;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls.MaterialControls;
using Telerik.Windows.Data;
using Telerik.Windows.DragDrop;


namespace PitchreadyGlobal.Views
{
    /// <summary>
    /// Interaction logic for LogoToolFrontEndView.xaml
    /// </summary>
    public partial class LogoToolFrontEndView : Window, INotifyPropertyChanged
    {
        string ProjectType;
        string ComboBoxSelection;
        private const string DropPositionFeedbackElementName = "DragBetweenItemsFeedback";
        private ContentPresenter dropPositionFeedbackPresenter;
        private Grid dropPositionFeedbackPresenterHost;
        bool isChecked = false;
        bool isAddWrapPanelCalled;
        private RadGridView associatedObject;
        string targetdatabasePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + System.IO.Path.DirectorySeparatorChar + "Evalueserve" + System.IO.Path.DirectorySeparatorChar + "Evalueserve" + System.IO.Path.DirectorySeparatorChar;
        string templateDir = Utils.GetToolTempPath(Tooltype.LogoTool);

        List<string> validPlaceHolders = new List<string>();
        List<int> ListOfImageAdded = new List<int>();
        #region Global Variables
        public static string Geo;
        public static string imgLogo;
        System.Data.DataTable dt_items = new System.Data.DataTable();
        System.Data.DataSet dsTarget;
        public string Search = "";
        System.Data.DataTable selected = new System.Data.DataTable();
        System.Data.DataTable selectedForCompany = new System.Data.DataTable();
        System.Data.DataTable companyDataTable = null;
        System.Data.DataTable brandDataTable = null;
        List<RadTreeViewItem> lstGeography = new List<RadTreeViewItem>();
        List<RadTreeViewItem> lstSectorIndustry = new List<RadTreeViewItem>();
        List<RadTreeViewItem> lstFreeFilter1 = new List<RadTreeViewItem>();
        List<RadTreeViewItem> lstFreeFilter2 = new List<RadTreeViewItem>();
        //CompanyDataLayer companyDataLayer = new CompanyDataLayer();
        //public static LogoPreview logoPreview = null;

        Dictionary<LogoToolCompanyDataFrontEnd, LogoTileUC> listTileViewMapping = new Dictionary<LogoToolCompanyDataFrontEnd, LogoTileUC>();// this is for mapping items of tileview and list view
        Dictionary<string, LogoToolCompanyDataFrontEnd> selectedItemsInCart = new Dictionary<string, LogoToolCompanyDataFrontEnd>();// this is for selected items of cart to maintain state between tile view and list view.
        #endregion
        DataSet dsCompany;
        AutoCompleteStringCollection autoComplteStrColl;
        System.Data.DataTable clonedCompany_tbl = new System.Data.DataTable();
        string sortingDir = "ASC";
        bool IsCollectionChanged = false;
        #region SerachCriteriaVariableForGeographyAndIndustry
        string GeographySearchFilterText = string.Empty;
        string SelectedContinents = string.Empty;
        string SelectedSubContinents = string.Empty;
        string SelectedRegions = string.Empty;
        string SelectedCountries = string.Empty;

        string IndustrySearchFilterText = string.Empty;
        string SelectedSectors = string.Empty;
        string SelectedIndustryGroups = string.Empty;
        string SelectedIndustries = string.Empty;
        string SelectedSubIndustries = string.Empty;

        string FreeFilter1SearchFilterText = string.Empty;
        string SelectedFilter3_1 = string.Empty;
        string SelectedFilter3_2 = string.Empty;
        string SelectedFilter3_3 = string.Empty;
        string SelectedFilter3_4 = string.Empty;

        string FreeFilter2SearchFilterText = string.Empty;
        string SelectedFilter4_1 = string.Empty;
        string SelectedFilter4_2 = string.Empty;
        string SelectedFilter4_3 = string.Empty;
        string SelectedFilter4_4 = string.Empty;

        Stopwatch ObjStopwatch;
        public bool IsProcessCancelled { get; set; }

        #endregion

        string TargetDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + System.IO.Path.DirectorySeparatorChar + "Evalueserve" + System.IO.Path.DirectorySeparatorChar + "Evalueserve" + System.IO.Path.DirectorySeparatorChar + "Logo Tool" + System.IO.Path.DirectorySeparatorChar;
        List<RadTreeView> availableTreeviews = new List<RadTreeView>();
        System.Data.DataTable companyTable = new System.Data.DataTable();
        bool frmInitialize = false;
        System.Data.DataTable Brandtbl = null;
        System.Data.DataTable originalTable = null;
        public static bool showPopupMsg = true;
        BackgroundWorker companyReaderThreadApplyButton;
        BackgroundWorker companyReaderThreadSearchTextBox;
        BackgroundWorker ExportToExcelButton;
        BackgroundWorker ExportToExcelButtonCart;
        ObservableCollection<LogoTileVM> cartSource = new ObservableCollection<LogoTileVM>();
        List<FilterDataModal> lstFilterDataModal = new List<FilterDataModal>();
        List<ManageHierarchyData> LstGeoFilterHierarchy = new List<ManageHierarchyData>();
        List<ManageHierarchyData> LstIndustryFilterHierarchy = new List<ManageHierarchyData>();
        List<ManageHierarchyData> LstFilter1FilterHierarchy = new List<ManageHierarchyData>();
        List<ManageHierarchyData> LstFilter2FilterHierarchy = new List<ManageHierarchyData>();
        ObservableCollection<DataLevelModel> ManageFilterMetaDataLevel1Collection;
        ObservableCollection<DataLevelModel> ManageFilterMetaDataLevel2Collection;
        ObservableCollection<DataLevelModel> ManageFilterMetaDataLevel3Collection;
        ObservableCollection<DataLevelModel> ManageFilterMetaDataLevel4Collection;
        ObservableCollection<ManageFieldGroupMaster> ManageFilterGroupCollection;
        public ObservableCollection<ManageLogoTemplateModel> ManageLogoTemplateCollection;

        ObservableCollection<LogoToolCompanyDataFrontEnd> ManageCompanyMasterDataCollection;
        ObservableCollection<LogoToolCompanyDataFrontEnd> manageCompanyMasterDataCollectionCart = new ObservableCollection<LogoToolCompanyDataFrontEnd>();
        List<LogoToolCompanyDataFrontEnd> masterDataClient = new List<LogoToolCompanyDataFrontEnd>();
        private bool isActiveGeoFilter;
        private bool isActiveIndustryFilter;
        private bool isActiveFilter1;
        private bool isActiveFilter2;
        private string geographyMasterDisplayName;
        private string industryMasterDisplayName;
        private string freeFilter1MasterDisplayName;
        private string freeFilter2MasterDisplayName;

        public bool IsActiveGeoFilter
        {
            get
            {
                return isActiveGeoFilter;
            }
            set
            {
                isActiveGeoFilter = value;
                OnPropertyChanged("IsActiveGeoFilter");
            }
        }
        public bool IsActiveIndustryFilter
        {
            get
            {
                return isActiveIndustryFilter;
            }
            set
            {
                isActiveIndustryFilter = value;
                OnPropertyChanged("IsActiveIndustryFilter");
            }
        }
        public bool IsActiveFilter1
        {
            get
            {
                return isActiveFilter1;
            }
            set
            {
                isActiveFilter1 = value;
                OnPropertyChanged("IsActiveFilter1");
            }
        }
        public bool IsActiveFilter2
        {
            get
            {
                return isActiveFilter2;
            }
            set
            {
                isActiveFilter2 = value;
                OnPropertyChanged("IsActiveFilter2");
            }
        }

        public string GeographyMasterDisplayName
        {
            get
            {
                return geographyMasterDisplayName;
            }

            set
            {
                geographyMasterDisplayName = value;
                OnPropertyChanged("GeographyMasterDisplayName");
            }
        }
        public string IndustryMasterDisplayName
        {
            get
            {
                return industryMasterDisplayName;
            }

            set
            {
                industryMasterDisplayName = value;
                OnPropertyChanged("IndustryMasterDisplayName");
            }
        }
        public string FreeFilter1MasterDisplayName
        {
            get
            {
                return freeFilter1MasterDisplayName;
            }

            set
            {
                freeFilter1MasterDisplayName = value;
                OnPropertyChanged("FreeFilter1MasterDisplayName");
            }
        }
        public string FreeFilter2MasterDisplayName
        {
            get
            {
                return freeFilter2MasterDisplayName;
            }

            set
            {
                freeFilter2MasterDisplayName = value;
                OnPropertyChanged("FreeFilter2MasterDisplayName");
            }
        }

        private BitmapImage companyWebsiteImageSource = null;
        public BitmapImage CompanyWebsiteImageSource
        {
            get
            {
                return companyWebsiteImageSource;
            }
            set
            {
                companyWebsiteImageSource = value;
                OnPropertyChanged("CompanyWebsiteImageSource");
            }
        }

        public ObservableCollection<LogoToolCompanyDataFrontEnd> ManageCompanyMasterDataCollectionCart
        {
            get
            {
                return manageCompanyMasterDataCollectionCart;
            }

            set
            {
                manageCompanyMasterDataCollectionCart = value;
                OnPropertyChanged("ManageCompanyMasterDataCollectionCart");
            }
        }

        public ObservableCollection<LogoTileVM> CartSource
        {
            get
            {
                return cartSource;
            }

            set
            {
                cartSource = value;
                OnPropertyChanged("CartSource");
            }
        }

        RadDesktopAlertManager DesktopAlertManagerObj = new RadDesktopAlertManager();

        public LogoToolFrontEndView(string projectType, string comboBoxSelection)
        {

            try
            {
                this.ProjectType = projectType;
                //FileBackupViewModel b = new FileBackupViewModel(projectType);
                this.ComboBoxSelection = comboBoxSelection;
                frmInitialize = true;

                InitializeComponent();

                if (projectType == "PitchreadyExcel")
                {
                    //Change Header Color As Per Project Type
                    this.Resources["SecondaryToolbarColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3F2F0"));
                    Header.HeaderColor = "#F3F2F0";

                    //Change Button Style As Per Project Type
                    clearAllBtn.Style = (Style)FindResource("ExcelButtonStyleSecondary");
                    ApplyBtn.Style = (Style)FindResource("ExcelButtonStylePrimary");
                    clearBtn.Style = (Style)FindResource("ExcelButtonStyleSecondary");
                    exportBtn.Style = (Style)FindResource("ExcelButtonStylePrimary");

                    ////Change Button Template As Per Project Type
                    //clearAllBtn.Template = FindResource("ExcelButtonStyleSecondaryTemplate") as ControlTemplate;
                    //ApplyBtn.Template = FindResource("ExcelButtonStylePrimaryTemplate") as ControlTemplate;
                    //clearBtn.Template = FindResource("ExcelButtonStyleSecondaryTemplate") as ControlTemplate;
                    //exportBtn.Template = FindResource("ExcelButtonStylePrimaryTemplate") as ControlTemplate;

                    //Change Icons
                    AtoZImage.ImageSource = (ImageSource)FindResource("Excel_atoz_logoDrawingImage");
                    NumSortImage.ImageSource = (ImageSource)FindResource("Excel_Sort_logoDrawingImage");
                    DeleteImage.ImageSource = (ImageSource)FindResource("DeleteDrawingImageLight");
                    btnExportCartImg.ImageSource = (ImageSource)FindResource("Excel_export_logoDrawingImage");
                    expanderImg.ImageSource = (ImageSource)FindResource("Excel_expand_logoDrawingImage");
                }
                else if (projectType == "PitchreadyPowerPoint")
                {

                }


                DesktopAlertManagerObj.HideAnimation = null;

                //added by evalueserve
                validPlaceHolders.Add(PpPlaceholderType.ppPlaceholderObject.ToString());
                validPlaceHolders.Add(PpPlaceholderType.ppPlaceholderMixed.ToString());
                validPlaceHolders.Add(PpPlaceholderType.ppPlaceholderMediaClip.ToString());
                validPlaceHolders.Add(PpPlaceholderType.ppPlaceholderBitmap.ToString());
                validPlaceHolders.Add(PpPlaceholderType.ppPlaceholderFooter.ToString());
                validPlaceHolders.Add(PpPlaceholderType.ppPlaceholderHeader.ToString());
                validPlaceHolders.Add(PpPlaceholderType.ppPlaceholderPicture.ToString());
                validPlaceHolders.Add(PpPlaceholderType.ppPlaceholderChart.ToString());
                validPlaceHolders.Add(PpPlaceholderType.ppPlaceholderTable.ToString());
                validPlaceHolders.Add(PpPlaceholderType.ppPlaceholderOrgChart.ToString());
                validPlaceHolders.Add(PpPlaceholderType.ppPlaceholderBody.ToString());

                radGeographyTreeView.FontSize = radIndustryTreeView.FontSize = 12;
                radFreeFilter1TreeView.FontSize = radFreeFilter2TreeView.FontSize = 12;
                CartSource.CollectionChanged += CartSource_CollectionChanged;
                //Utils.SetClientLogoWpf(clientLogoImage, Utils.ClientName);

                companyTable.Clear();
                companyTable.Columns.Add("ID");
                companyTable.Columns.Add("CompanyName_BrandName");
                companyTable.Columns.Add("LogoAddress");
                DataColumn colBoolean = new DataColumn("IsBrand");
                colBoolean.DataType = System.Type.GetType("System.Boolean");
                companyTable.Columns.Add(colBoolean);
                filter1Expander.Header = "Geography";
                filter2Expander.Header = "Industry";
                if (Utils.logoFilters != null && Utils.logoFilters.Count > 0)
                {
                    filter1Expander.Header = Utils.logoFilters[0];
                    filter2Expander.Header = Utils.logoFilters[1];
                }
                radGridViewGeoAndIndustry.Items.CollectionChanged += Items_CollectionChanged;
                radGridViewGeoAndIndustry.FilterDescriptors.CollectionChanging += FilterDescriptors_CollectionChanging;
                radGridViewGeoAndIndustry.FilterDescriptors.CollectionChanged += FilterDescriptors_CollectionChanged;

                tabItemTileView.IsSelected = true;

                this.MinHeight = (System.Windows.SystemParameters.PrimaryScreenHeight * 0.80);
                this.MinWidth = (System.Windows.SystemParameters.PrimaryScreenWidth * 0.90);
                CenterWindowOnScreen();
                Utils.SetFormTitle(this);
                this.Closed += LogoToolFrontEndView_Closed;
            }
            catch (Exception ex)
            {
                Utils.LogError("Error while loading Logo LogoForm : " + ex.Message + ", " + ex.InnerException);
            }
        }

        private void LogoToolFrontEndView_Closed(object sender, EventArgs e)
        {
            string s1 = "LogoToolFrontEndExcelWindowClose";
            Dictionary<object, object> combinedList = new Dictionary<object, object>();
            combinedList.Add(s1, string.Empty);
            Utils.InitiateWorkOnExcelEvent.GetEvent<CrossModuleCommunication>().Publish(combinedList);
        }

        private void CartSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            updateLayout();
        }

        private async void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            templateReviewCount.Content = 0;
            Collection<RadTreeViewItem> listGeoItems = GetContainers(radGeographyTreeView);
            Collection<RadTreeViewItem> listSectorItems = GetContainers(radIndustryTreeView);
            Collection<RadTreeViewItem> listFreeFilter1Items = GetContainers(radFreeFilter1TreeView);
            Collection<RadTreeViewItem> listFreeFilter2Items = GetContainers(radFreeFilter2TreeView);

            if ((listGeoItems == null || listGeoItems.Count == 0)
                && (listSectorItems == null || listSectorItems.Count == 0)
                && (listFreeFilter1Items == null || listFreeFilter1Items.Count == 0)
                && (listFreeFilter2Items == null || listFreeFilter2Items.Count == 0)
                )
            {
                CustomMessageBoxExcel.Show("Please select some filter(s)", "Logo Tool", ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Error);
                return;
            }

            btnExport.IsEnabled = true;
            busyIndicator.IsBusy = true;
            await Task.Run(() =>
            {
                Thread.Sleep(200);
            });
            busyIndicator.BusyContent = "Please Wait...";
            searchCompany.SearchText = string.Empty;
            searchTree.SearchText = string.Empty;
            searchradIndustryTree.SearchText = string.Empty;
            this.radGridViewGeoAndIndustry.FilterDescriptors.SuspendNotifications();
            foreach (Telerik.Windows.Controls.GridViewColumn column in this.radGridViewGeoAndIndustry.Columns)
            {
                column.ClearFilters();
            }
            this.radGridViewGeoAndIndustry.FilterDescriptors.ResumeNotifications();
            if (companyReaderThreadApplyButton == null)
            {
                companyReaderThreadApplyButton = new BackgroundWorker();
                companyReaderThreadApplyButton.DoWork += new DoWorkEventHandler(companyReaderThread_DoWork);
                companyReaderThreadApplyButton.RunWorkerCompleted += new
                    RunWorkerCompletedEventHandler(companyReaderThread_RunWorkerCompleted);
            }
            companyReaderThreadApplyButton.RunWorkerAsync();
        }
        private async void companyReaderThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (radGridViewGeoAndIndustry.Visibility == Visibility.Visible)
            {
                selectCheckBox.Checked -= selectCheckBox_Checked;
                selectCheckBox.Unchecked -= selectCheckBox_Unchecked;
                radGridViewGeoAndIndustry.SelectionChanged -= radGridViewGeoAndIndustry_SelectionChanged;
                SelectedContinents = string.Empty;
                SelectedSubContinents = string.Empty;
                SelectedRegions = string.Empty;
                SelectedCountries = string.Empty;

                SelectedSectors = string.Empty;
                SelectedIndustryGroups = string.Empty;
                SelectedIndustries = string.Empty;
                SelectedSubIndustries = string.Empty;

                SelectedFilter3_1 = string.Empty;
                SelectedFilter3_2 = string.Empty;
                SelectedFilter3_3 = string.Empty;
                SelectedFilter3_4 = string.Empty;

                SelectedFilter4_1 = string.Empty;
                SelectedFilter4_2 = string.Empty;
                SelectedFilter4_3 = string.Empty;
                SelectedFilter4_4 = string.Empty;

                LsvPageGlobVar.RecStart = 0;

                List<TreeNode> listGeo = new List<TreeNode>();
                List<TreeNode> listIndustry = new List<TreeNode>();

                Collection<RadTreeViewItem> listGeoItems = GetContainers(radGeographyTreeView);
                if (listGeoItems != null)
                    lstGeography = listGeoItems.ToList();

                Collection<RadTreeViewItem> listSectorItems = GetContainers(radIndustryTreeView);
                if (listSectorItems != null)
                    lstSectorIndustry = listSectorItems.ToList();

                Collection<RadTreeViewItem> listFreeFilter1Items = GetContainers(radFreeFilter1TreeView);
                if (listFreeFilter1Items != null)
                    lstFreeFilter1 = listFreeFilter1Items.ToList();

                Collection<RadTreeViewItem> listFreeFilter2Items = GetContainers(radFreeFilter2TreeView);
                if (listFreeFilter2Items != null)
                    lstFreeFilter2 = listFreeFilter2Items.ToList();

                await SearchCompany();

                //SearchCompanyBasedOnSectorIndustryAndGeography();               

                selectItemsBasedOnCart();
                shrinkGrid();
                editColumnChooser();

                if (radGridViewGeoAndIndustry.Items.Count != 0 && radGridViewGeoAndIndustry.SelectedItems.Count == radGridViewGeoAndIndustry.Items.Count)
                    selectCheckBox.IsChecked = true;
                else
                    selectCheckBox.IsChecked = false;
                busyIndicator.IsBusy = false;

                selectCheckBox.Checked += selectCheckBox_Checked;
                selectCheckBox.Unchecked += selectCheckBox_Unchecked;
                radGridViewGeoAndIndustry.SelectionChanged += radGridViewGeoAndIndustry_SelectionChanged;
            }
            else
            {

                radBrandGridView.ItemsSource = Brandtbl;
                busyIndicator.IsBusy = false;
                selectCheckBox.Checked += selectCheckBox_Checked;
                selectCheckBox.Unchecked += selectCheckBox_Unchecked;
                radGridViewGeoAndIndustry.SelectionChanged += radGridViewGeoAndIndustry_SelectionChanged;
            }
        }
        private void companyReaderThread_DoWork(object sender, DoWorkEventArgs e)
        {
            bool isBrand = false;
            bool isCompany = false;
            Dispatcher.Invoke(() =>
            {
                if (radGridViewGeoAndIndustry.Visibility == Visibility.Visible)
                    isCompany = true;
                else
                    isBrand = true;
            });

            if (isBrand)
            {
                //Brandtbl = companyDataLayer.GetBrandDetails();
            }
        }

        private void setImportTypeSource()
        {
            List<string> importTypeItems = new List<string>();

            if (ProjectType == "PitchreadyPowerPoint")
            {
                if (ManageLogoTemplateCollection.Count > 0)
                {
                    importTypeItems.Add("New Slide/Template");
                    importTypeItems.Add("Non-Printable Area");
                    importTypeItems.Add("Single object");
                    importTypeItems.Add("Multiple objects");
                    cmbImportType.ItemsSource = importTypeItems;
                    cmbImportType.SelectedIndex = 1;
                }
                else
                {
                    importTypeItems.Add("Non-Printable Area");
                    importTypeItems.Add("Single object");
                    importTypeItems.Add("Multiple onjects");
                    cmbImportType.ItemsSource = importTypeItems;
                    cmbImportType.SelectedIndex = 0;
                }

                //updateComboBox();
            }

            else if (ProjectType == "PitchreadyExcel")
            {
                if (ManageLogoTemplateCollection.Count > 0)
                {
                    importTypeItems.Add("Multiple cells - Horizontal");
                    importTypeItems.Add("Multiple cells - Vertical");
                    importTypeItems.Add("All in one cell");
                    importTypeItems.Add("Folder");
                    cmbImportType.ItemsSource = importTypeItems;
                    cmbImportType.SelectedItem = ComboBoxSelection;
                }
            }
            List<string> resolutions = new List<string>();
            resolutions.Add("High Resolution");
            resolutions.Add("Low Resolution");
            cmbResolution.ItemsSource = resolutions;
        }
        private void PopulateFilters()
        {
            //checkForExistingFilters();
        }

        //TODO: self commented
        //private void checkForExistingFilters()
        //{
        //    FiltersFile filters = new FiltersFile();
        //    FileInfo[] Files = filters.getFilterFiles("LogoFilterMetaData*.xml", TargetDirectory);

        //    foreach (FileInfo file in Files)
        //    {
        //        RadExpander expander = new RadExpander();

        //        RadTreeView tv = new RadTreeView();


        //        expander.Content = tv;
        //        tv.IsTriStateMode = true;
        //        tv.IsOptionElementsEnabled = true;

        //        RadTreeViewItem category = new RadTreeViewItem();
        //        string filterName = file.Name.Split('_')[1].Replace(".xml", "");
        //        category.Header = filterName;
        //        category.Tag = filterName;
        //        expander.Header = filterName;
        //        RadTreeViewItem item = createLevel("LogoFilterMetaData" + "_" + file.Name.Split('_')[1], "level1", category);
        //        item.IsTextSearchEnabled = true;
        //        tv.Items.Add(item);
        //        filterPanel.Children.Add(expander);
        //        tv.Name = filterName;
        //        availableTreeviews.Add(tv);
        //    }
        //}

        private RadTreeViewItem createLevel(string fileName, string tagName, RadTreeViewItem category)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(TargetDirectory + fileName);

            XmlNodeList elemList = doc.GetElementsByTagName(tagName);

            for (int i = 0; i < elemList.Count; i++)
            {
                RadTreeViewItem node = new RadTreeViewItem();
                node.Header = elemList[i].InnerXml;
                node.Tag = elemList[i].InnerXml;

                string newTagName = removeSpaces(elemList[i].InnerXml);

                XmlNodeList elemList1 = doc.GetElementsByTagName(newTagName + "__" + tagName);
                createLevel(fileName, newTagName + "__" + tagName, node);

                node.IsTextSearchEnabled = true;
                category.Items.Add(node);
            }

            return category;

        }

        private string removeSpaces(string str)
        {
            str = str.Trim().Replace(" ", string.Empty);
            str = str.Replace("/", "_");
            return str;
        }
        private async Task LoadLogoTemplateData()
        {
            try
            {
                var res = await ServiceManager.GetData<List<ManageLogoTemplateModel>>(PitchreadyConstants.GetAllLogoTemplateData, Tooltype.LogoTool);
                ManageLogoTemplateCollection = new ObservableCollection<ManageLogoTemplateModel>(res.Data);
            }
            catch (Exception ex)
            {
                Utils.LogError("GetFilterDataLevel1: " + ex.ToString());
            }
        }

        private async Task LoadFilterGroupData()
        {
            ManageFilterGroupCollection = new ObservableCollection<ManageFieldGroupMaster>();
            await Utils.GetFiledGroupMasterList(false);
            ManageFilterGroupCollection = new ObservableCollection<ManageFieldGroupMaster>(Utils.fieldGroupMasterList);
        }

        private async Task GetFilterDataLevel1()
        {
            try
            {
                Dictionary<string, string> Params = new Dictionary<string, string>();
                string filterGroupIds = ((int)(EnumGeoMasterFilter.Geography)).ToString() + "," + ((int)(EnumGeoMasterFilter.Industry)).ToString() + "," + ((int)(EnumGeoMasterFilter.Filter1)).ToString() + "," + ((int)(EnumGeoMasterFilter.Filter2)).ToString();
                Params.Add("FilterGroupId", filterGroupIds);
                var res = await ServiceManager.GetDataByParameter<List<DataLevelModel>>(PitchreadyConstants.GetFilterDataLevel1, Params, Tooltype.LogoTool);
                ManageFilterMetaDataLevel1Collection = new ObservableCollection<DataLevelModel>(res.Data);
            }
            catch (Exception ex)
            {
                Utils.LogError("GetFilterDataLevel1: " + ex.ToString());
            }
        }
        private async Task GetFilterDataLevel2()
        {
            try
            {
                Dictionary<string, string> Params = new Dictionary<string, string>();
                string filterGroupIds = ((int)(EnumGeoMasterFilter.Geography)).ToString() + "," + ((int)(EnumGeoMasterFilter.Industry)).ToString() + "," + ((int)(EnumGeoMasterFilter.Filter1)).ToString() + "," + ((int)(EnumGeoMasterFilter.Filter2)).ToString();
                Params.Add("FilterGroupId", filterGroupIds);
                var res = await ServiceManager.GetDataByParameter<List<DataLevelModel>>(PitchreadyConstants.GetFilterDataLevel2, Params, Tooltype.LogoTool);
                ManageFilterMetaDataLevel2Collection = new ObservableCollection<DataLevelModel>(res.Data);
            }
            catch (Exception ex)
            {
                Utils.LogError("GetFilterDataLevel1: " + ex.ToString());
            }
        }
        private async Task GetFilterDataLevel3()
        {
            Dictionary<string, string> Params = new Dictionary<string, string>();
            string filterGroupIds = ((int)(EnumGeoMasterFilter.Geography)).ToString() + "," + ((int)(EnumGeoMasterFilter.Industry)).ToString() + "," + ((int)(EnumGeoMasterFilter.Filter1)).ToString() + "," + ((int)(EnumGeoMasterFilter.Filter2)).ToString();
            Params.Add("FilterGroupId", filterGroupIds);
            var res = await ServiceManager.GetDataByParameter<List<DataLevelModel>>(PitchreadyConstants.GetFilterDataLevel3, Params, Tooltype.LogoTool);
            ManageFilterMetaDataLevel3Collection = new ObservableCollection<DataLevelModel>(res.Data);
        }
        private async Task GetFilterDataLevel4()
        {
            try
            {
                Dictionary<string, string> Params = new Dictionary<string, string>();
                string filterGroupIds = ((int)(EnumGeoMasterFilter.Geography)).ToString() + "," + ((int)(EnumGeoMasterFilter.Industry)).ToString() + "," + ((int)(EnumGeoMasterFilter.Filter1)).ToString() + "," + ((int)(EnumGeoMasterFilter.Filter2)).ToString();
                Params.Add("FilterGroupId", filterGroupIds);
                var res = await ServiceManager.GetDataByParameter<List<DataLevelModel>>(PitchreadyConstants.GetFilterDataLevel4, Params, Tooltype.LogoTool);
                ManageFilterMetaDataLevel4Collection = new ObservableCollection<DataLevelModel>(res.Data);
            }
            catch (Exception ex)
            {
                Utils.LogError("GetFilterDataLevel1: " + ex.ToString());
            }
        }
        public async Task GetAllCompanyMasterData()
        {
            try
            {
                Utils.LogError("GetAllCompanyMasterData Start Time: " + DateTime.Now);
                busyIndicator.BusyContent = "Loading data...";
                busyIndicator.IsBusy = true;
                await Task.Run(() =>
                {
                    Thread.Sleep(200);
                });
                string searchString = string.Empty;
                ManageCompanyMasterDataCollection = new ObservableCollection<LogoToolCompanyDataFrontEnd>();
                string apistrData = Newtonsoft.Json.JsonConvert.SerializeObject(lstFilterDataModal);
                Dictionary<string, string> newDict = new Dictionary<string, string>();
                newDict.Add("FilterData", apistrData);
                newDict.Add("OperationType", ((int)CRUDType.FilterSearch).ToString());
                newDict.Add("SearchString", searchString);
                var result = await ServiceManager.GetDataByParameter<List<LogoToolCompanyDataFrontEnd>>(PitchreadyConstants.GetAllClientCompanyMasterData, newDict, Tooltype.LogoTool);
                if (result != null)
                    ManageCompanyMasterDataCollection = new ObservableCollection<LogoToolCompanyDataFrontEnd>(result.Data);
                Utils.LogError("GetAllCompanyMasterData End Time: " + DateTime.Now);
            }
            catch
            {
                busyIndicator.IsBusy = false;
            }
        }
        public async Task GetAllCompanyMasterData(string SearchString)
        {
            try
            {
                Utils.LogError("GetAllCompanyMasterData(string SearchString) Start Time: " + DateTime.Now);
                busyIndicator.BusyContent = "Loading data...";
                busyIndicator.IsBusy = true;
                await Task.Run(() =>
                {
                    Thread.Sleep(200);
                });
                string searchString = SearchString;
                ManageCompanyMasterDataCollection = new ObservableCollection<LogoToolCompanyDataFrontEnd>();
                string apistrData = Newtonsoft.Json.JsonConvert.SerializeObject(lstFilterDataModal);
                Dictionary<string, string> newDict = new Dictionary<string, string>();
                newDict.Add("FilterData", apistrData);
                newDict.Add("OperationType", ((int)CRUDType.StringSearch).ToString());
                newDict.Add("SearchString", searchString);
                var result = await ServiceManager.GetDataByParameter<List<LogoToolCompanyDataFrontEnd>>(PitchreadyConstants.GetAllClientCompanyMasterData, newDict, Tooltype.LogoTool);
                if (result != null)
                    ManageCompanyMasterDataCollection = new ObservableCollection<LogoToolCompanyDataFrontEnd>(result.Data);
                Utils.LogError("GetAllCompanyMasterData(string SearchString) End Time: " + DateTime.Now);
            }
            catch
            {
                busyIndicator.IsBusy = false;
            }
        }
        private async Task LoadData()
        {
            try
            {
                var task = new List<Task>();
                //Task t1 = LoadFilterGroupData();
                //task.Add(t1);
                await GetFilterDataLevel1();
                //task.Add(t2);
                await GetFilterDataLevel2();
                //task.Add(t3);
                await GetFilterDataLevel3();
                //task.Add(t4);
                await GetFilterDataLevel4();
                //task.Add(t5);
                //Task t6 = GetAllCompanyMasterData();
                //task.Add(t6);
                await LoadLogoTemplateData();
                //task.Add(t7);

                //await Task.WhenAll(task);
            }
            catch (Exception ex)
            {
                Utils.LogError("logoForm In Load data " + ex.ToString());
            }
        }
        //public async Task DownloadLogoArtifacts(string SystemLogoName)
        //{
        //    try
        //    {
        //        busyIndicator.IsBusy = true;
        //        busyIndicator.BusyContent = "Downloading Logo Artefacts...";
        //        List<string> previewFile = new List<string>();
        //        previewFile.Add(SystemLogoName);
        //        await Task.Factory.StartNew(() =>
        //        {
        //            System.Threading.Thread.Sleep(200);
        //        });
        //        var isArtifactDownloaded = await ServiceCallHelper.DownloadFilesAndExtract(PitchreadyConstants.getLTCompressedfiles, previewFile, Tooltype.LogoTool);
        //        if (isArtifactDownloaded == null || !(bool)isArtifactDownloaded)
        //        {
        //            CustomMessageBoxExcel.Show("Unable to download logo(s), please check the connection and try again.", "Logo Tool", ExcelMessageBoxType.Ok, GlobalCustomControls.ExcelMessageBoxImage.Error);
        //            busyIndicator.IsBusy = false;
        //            return;
        //        }

        //        string tempDirectory = Utils.GetToolTempPath(Tooltype.LogoTool);
        //        busyIndicator.IsBusy = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        //Utils.LogError(DtkConstants.DisclaimerFeature + ex.StackTrace);
        //        busyIndicator.IsBusy = false;
        //    }

        //}
        private async Task DownloadAllLogosArtifacts(List<string> filelist)
        {
            string filesize = null;
            ObjStopwatch = new Stopwatch();
            ObjStopwatch.Start();
            Dictionary<string, string> DownloadFilecount = new Dictionary<string, string>();
            //_timer.Start();
            try
            {
                var templateDir = Utils.GetToolTempPath(Tooltype.LogoTool);
                var DistinctPresentationData = masterDataClient.GroupBy(x => x.ID).Select(g => g.First()).ToList();

                if (filelist.Count > 0)
                {
                    var cancellationToken = new CancellationTokenSource();
                    var result = await ServiceManager.DownloadFileAsync(PitchreadyConstants.getLTCompressedfiles, filelist, Tooltype.LogoTool, cancellationToken.Token, StkProgress, this);

                    if (IsProcessCancelled)
                        return;
                    if (result == null || result.InputData.Count == 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            CustomMessageBoxExcel.Show("Unable to download logo(s). Please try again.", "Logo Tool", ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Warning);
                            RadProgressBar1.Value = 0;
                            txtLoadingLabel.Text = "Downloading File...";
                            radBusyIndicator.BusyContent = "Loading...";
                            BrdrProgressIndicator.Visibility = Visibility.Collapsed;
                            TileViewBusyIndicator.Visibility = Visibility.Collapsed;
                        });
                        return;
                    }
                    filesize = result.FileSize;
                    DownloadFilecount = result.InputData;
                    Dispatcher.Invoke(() =>
                    {
                        RadProgressBar1.Value = 0;
                        txtLoadingLabel.Text = "Downloading File...";
                        BrdrProgressIndicator.Visibility = Visibility.Collapsed;
                        radBusyIndicator.BusyContent = "Extracting Files...";
                        TileViewBusyIndicator.Visibility = Visibility.Visible;
                    });
                }

                var TotalfilesList = masterDataClient.Select(o => o.SystemLogoName).ToList();
                TotalfilesList = TotalfilesList.Where(o => !string.IsNullOrEmpty(o)).ToList();
                int count = 0;
                foreach (var item in DistinctPresentationData)
                {
                    var ManageCompanyMasterData = masterDataClient.Where(o => o.ID == item.ID);
                    foreach (LogoToolCompanyDataFrontEnd Templatedetail in ManageCompanyMasterData)
                    {
                        try
                        {
                            if (IsProcessCancelled)
                                break;
                            await Task.Factory.StartNew(() =>
                            {
                                System.Threading.Thread.Sleep(50);

                            });
                            count++;
                            Dispatcher.Invoke(() =>
                            {
                                radBusyIndicator.BusyContent = "Loading Logo Artefacts (" + count + " / " + TotalfilesList.Count + ")";
                            });
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                string macID = Utils.GetMACAddress();
                string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

                ObjStopwatch.Stop();
                Dispatcher.Invoke(() =>
                {
                    //this.WindowState = WindowState.Minimized;
                    if (DownloadFilecount.Count() < filelist.Count())
                    {
                        string FilesListmessage = string.Empty;
                        List<LogoToolCompanyDataFrontEnd> list = DistinctPresentationData.Where(x => !DownloadFilecount.Any(y => y.Key == x.OriginalLogoName)).ToList();
                        for (int i = 0; i < list.Count(); i++)
                        {
                            FilesListmessage += Environment.NewLine + (i + 1) + ". " + list[i].OriginalLogoName;
                        }
                    }
                });
                long elapsedTime = ObjStopwatch.ElapsedMilliseconds / 1000;
                string elapsedTimestring = elapsedTime.ToString();
                if (elapsedTime > 0)
                    elapsedTimestring = elapsedTime.ToString() + " Seconds";
            }
            catch (Exception ex)
            {
                Utils.LogError("Exception in downloading Logos : " + ex.Message + ", " + ex.InnerException);
            }

            Dispatcher.Invoke(() =>
            {
                ObjStopwatch.Stop();
                RadProgressBar1.Value = 0;
                txtLoadingLabel.Text = "Downloading File...";
                radBusyIndicator.BusyContent = "Loading...";
                BrdrProgressIndicator.Visibility = Visibility.Collapsed;
                TileViewBusyIndicator.Visibility = Visibility.Collapsed;
            });

        }
        private void Geography()
        {
            try
            {
                if (ManageFilterGroupCollection.Count == 0)
                    ManageFilterGroupCollection = new ObservableCollection<ManageFieldGroupMaster>(Utils.fieldGroupMasterList);

                GeographyMasterDisplayName = Utils.fieldGroupMasterList.Where(x => x.FilterName == "Filter 1").Select(y => y.DisplayName).FirstOrDefault();
                IndustryMasterDisplayName = Utils.fieldGroupMasterList.Where(x => x.FilterName == "Filter 2").Select(y => y.DisplayName).FirstOrDefault();
                FreeFilter1MasterDisplayName = Utils.fieldGroupMasterList.Where(x => x.FilterName == "Filter 3").Select(y => y.DisplayName).FirstOrDefault();
                FreeFilter2MasterDisplayName = Utils.fieldGroupMasterList.Where(x => x.FilterName == "Filter 3").Select(y => y.DisplayName).FirstOrDefault();

                Dispatcher.Invoke(() =>
                {
                    Filter1_1Column.Header = filter1Expander.Header = GeographyMasterDisplayName;
                    Filter2_1Column.Header = filter2Expander.Header = IndustryMasterDisplayName;
                    Filter3_1Column.Header = filter3Expander.Header = FreeFilter1MasterDisplayName;
                    Filter4_1Column.Header = filter4Expander.Header = FreeFilter2MasterDisplayName;

                    IsActiveGeoFilter = ManageFilterGroupCollection.Where(x => x.FilterId == 1).Select(x => x.IsStatus).FirstOrDefault();
                    if (IsActiveGeoFilter)
                    {
                        filter1Expander.Visibility = Visibility.Visible;
                        List<DataLevelModel> lstGeoFilterMetaDataLvl1 = ManageFilterMetaDataLevel1Collection.Where(x => x.FilterGroupId == (int)(EnumGeoMasterFilter.Geography)).ToList();
                        List<DataLevelModel> lstGeoFilterMetaDataLvl2 = ManageFilterMetaDataLevel2Collection.Where(x => x.FilterGroupId == (int)(EnumGeoMasterFilter.Geography)).ToList();
                        List<DataLevelModel> lstGeoFilterMetaDataLvl3 = ManageFilterMetaDataLevel3Collection.Where(x => x.FilterGroupId == (int)(EnumGeoMasterFilter.Geography)).ToList();
                        List<DataLevelModel> lstGeoFilterMetaDataLvl4 = ManageFilterMetaDataLevel4Collection.Where(x => x.FilterGroupId == (int)(EnumGeoMasterFilter.Geography)).ToList();

                        PopulateGeographyTreeFromDB(radGeographyTreeView, lstGeoFilterMetaDataLvl1, lstGeoFilterMetaDataLvl2, lstGeoFilterMetaDataLvl3, lstGeoFilterMetaDataLvl4);
                    }
                    else
                    {
                        filter1Expander.Visibility = Visibility.Collapsed;
                        Filter1_1Column.IsVisible = false;
                    }
                });
            }
            catch (Exception ex)
            {
                Utils.LogError("Exception inside  Geography : " + ex.Message + ", " + ex.InnerException);
            }
        }
        private void PopulateGeographyTreeFromDB(Telerik.Windows.Controls.RadTreeView objTreeView, List<DataLevelModel> lstGeoFilterMetaDataLvl1,
            List<DataLevelModel> lstGeoFilterMetaDataLvl2, List<DataLevelModel> lstGeoFilterMetaDataLvl3, List<DataLevelModel> lstGeoFilterMetaDataLvl4)
        {
            try
            {
                string filterType = EnumGeoMasterFilter.Geography.ToString();
                RadTreeViewItem category = new RadTreeViewItem();
                category.Header = "All";
                category.Tag = "Geography";
                if (Utils.logoFilters != null && Utils.logoFilters.Count > 0)
                {
                    category.Header = Utils.logoFilters[0];
                    category.Tag = Utils.logoFilters[0];
                }
                category.IsExpanded = true;
                objTreeView.Items.Add(category);
                foreach (DataLevelModel dataRow in lstGeoFilterMetaDataLvl1)
                {
                    RadTreeViewItem treeRoot = new RadTreeViewItem();
                    int count = dataRow.FilterCount;//ManageCompanyMasterDataCollection.Where(x => x.Filter1_1.Contains(dataRow.Name.ToString())).ToList().Count;
                    treeRoot.Header = dataRow.Name.ToString() + " (" + count + ")";
                    treeRoot.Tag = dataRow.Id + "," + dataRow.FilterGroupId;
                    treeRoot.ToolTip = treeRoot.Header;

                    category.Items.Add(treeRoot);

                    foreach (RadTreeViewItem childnode in GetChildNodeForLvl2FromDB(Convert.ToInt64(dataRow.Id), lstGeoFilterMetaDataLvl2, lstGeoFilterMetaDataLvl3, lstGeoFilterMetaDataLvl4, filterType))
                    {
                        childnode.ToolTip = childnode.Header;
                        treeRoot.Items.Add(childnode);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.LogError("Exception inside  PopulateGeographyTreeFromDB : " + ex.Message + ", " + ex.InnerException);
            }
        }
        private List<RadTreeViewItem> GetChildNodeForLvl2FromDB(long parentid, List<DataLevelModel> lstFilterMetaDataLvl2, List<DataLevelModel> lstFilterMetaDataLvl3, List<DataLevelModel> lstFilterMetaDataLvl4, string filterType)
        {
            int count = 0;
            List<RadTreeViewItem> childtreenodes = new List<RadTreeViewItem>();
            try
            {
                foreach (DataLevelModel dataRow in lstFilterMetaDataLvl2)
                {
                    if (parentid == dataRow.Level1Id)
                    {
                        count = 0;
                        RadTreeViewItem childNode = new RadTreeViewItem();

                        childNode.Header = dataRow.Name.ToString() + " (" + dataRow.FilterCount + ")";
                        childNode.Tag = dataRow.Id + "," + 2;
                        childNode.ExpandAll();
                        foreach (RadTreeViewItem cnode in GetChildNodeForLvl3FromDB(Convert.ToInt64(dataRow.Id), lstFilterMetaDataLvl3, lstFilterMetaDataLvl4, filterType))
                        {
                            childNode.Items.Add(cnode);
                        }
                        childtreenodes.Add(childNode);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.LogError("Exception inside  GetChildNodeForLvl2FromDB : " + ex.Message + ", " + ex.InnerException);
            }
            return childtreenodes;
        }
        private List<RadTreeViewItem> GetChildNodeForLvl3FromDB(long parentid, List<DataLevelModel> lstFilterMetaDataLvl3, List<DataLevelModel> lstFilterMetaDataLvl4, string filterType)
        {
            int count = 0;
            List<RadTreeViewItem> childtreenodes = new List<RadTreeViewItem>();
            try
            {
                foreach (DataLevelModel dataRow in lstFilterMetaDataLvl3)
                {
                    if (parentid == dataRow.Level2Id)
                    {
                        count = 0;
                        RadTreeViewItem childNode = new RadTreeViewItem();

                        childNode.Header = dataRow.Name.ToString() + " (" + dataRow.FilterCount + ")";
                        childNode.Tag = dataRow.Id + "," + 3;
                        childNode.ExpandAll();
                        foreach (RadTreeViewItem cnode in GetChildNodeForLvl4FromDB(Convert.ToInt64(dataRow.Id), lstFilterMetaDataLvl4, filterType))
                        {
                            childNode.Items.Add(cnode);
                        }
                        childtreenodes.Add(childNode);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.LogError("Exception inside  GetChildNodeForLvl3FromDB : " + ex.Message + ", " + ex.InnerException);
            }
            return childtreenodes;
        }
        private List<RadTreeViewItem> GetChildNodeForLvl4FromDB(long parentid, List<DataLevelModel> lstFilterMetaDataLvl4, string filterType)
        {
            int count = 0;
            List<RadTreeViewItem> childtreenodes = new List<RadTreeViewItem>();
            try
            {
                foreach (DataLevelModel dataRow in lstFilterMetaDataLvl4)
                {
                    if (parentid == dataRow.Level3Id)
                    {
                        count = 0;
                        RadTreeViewItem childNode = new RadTreeViewItem();

                        childNode.Header = dataRow.Name.ToString() + " (" + dataRow.FilterCount + ")";
                        childNode.Tag = dataRow.Id + "," + 4;
                        childNode.ExpandAll();

                        childtreenodes.Add(childNode);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.LogError("Exception inside  GetChildNodeForLvl4FromDB : " + ex.Message + ", " + ex.InnerException);
            }
            return childtreenodes;
        }

        private List<RadTreeViewItem> GetGeographyChildNode(long parentid, System.Data.DataTable dataSource)
        {
            List<RadTreeViewItem> childtreenodes = new List<RadTreeViewItem>();
            try
            {
                DataView dataView1 = new DataView(dataSource);
                String strFilter = "" + "GeographyParentID" + "=" + parentid.ToString() + "";
                dataView1.RowFilter = strFilter;

                if (dataView1.Count > 0)
                {
                    foreach (DataRow dataRow in dataView1.ToTable().Rows)
                    {
                        RadTreeViewItem childNode = new RadTreeViewItem();
                        childNode.Header = dataRow["GeographyMapName"].ToString();
                        childNode.Tag = dataRow["GeographyMapID"] + "," + dataRow["GeographyMapType"];
                        childNode.ExpandAll();
                        foreach (RadTreeViewItem cnode in GetGeographyChildNode(Convert.ToInt64(dataRow["GeographyMapID"]), dataSource))
                        {
                            childNode.Items.Add(cnode);
                        }
                        childtreenodes.Add(childNode);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.LogError("Exception inside  GetGeographyChildNode : " + ex.Message + ", " + ex.InnerException);
            }
            return childtreenodes;
        }

        private void SectorIndustry()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    IsActiveIndustryFilter = ManageFilterGroupCollection.Where(x => x.FilterId == 2).Select(x => x.IsStatus).FirstOrDefault();
                    if (IsActiveIndustryFilter)
                    {
                        filter2Expander.Visibility = Visibility.Visible;
                        List<DataLevelModel> lstIndustryFilterMetaDataLvl1 = ManageFilterMetaDataLevel1Collection.Where(x => x.FilterGroupId == (int)(EnumGeoMasterFilter.Industry)).ToList();
                        List<DataLevelModel> lstIndustryFilterMetaDataLvl2 = ManageFilterMetaDataLevel2Collection.Where(x => x.FilterGroupId == (int)(EnumGeoMasterFilter.Industry)).ToList();
                        List<DataLevelModel> lstIndustryFilterMetaDataLvl3 = ManageFilterMetaDataLevel3Collection.Where(x => x.FilterGroupId == (int)(EnumGeoMasterFilter.Industry)).ToList();
                        List<DataLevelModel> lstIndustryFilterMetaDataLvl4 = ManageFilterMetaDataLevel4Collection.Where(x => x.FilterGroupId == (int)(EnumGeoMasterFilter.Industry)).ToList();

                        PopulateSectorIndustryTreeFromDB(radIndustryTreeView, lstIndustryFilterMetaDataLvl1, lstIndustryFilterMetaDataLvl2, lstIndustryFilterMetaDataLvl3, lstIndustryFilterMetaDataLvl4);
                    }
                    else
                    {
                        filter2Expander.Visibility = Visibility.Collapsed;
                        Filter2_1Column.IsVisible = false;
                    }
                });
            }
            catch (Exception ex)
            {
                Utils.LogError("Exception inside  SectorIndustry : " + ex.Message + ", " + ex.InnerException);
            }
        }
        private void PopulateSectorIndustryTreeFromDB(Telerik.Windows.Controls.RadTreeView objTreeView, List<DataLevelModel> lstIndustryFilterMetaDataLvl1,
            List<DataLevelModel> lstIndustryFilterMetaDataLvl2, List<DataLevelModel> lstIndustryFilterMetaDataLvl3, List<DataLevelModel> lstIndustryFilterMetaDataLvl4)
        {
            try
            {
                string filterType = EnumGeoMasterFilter.Industry.ToString();
                RadTreeViewItem category = new RadTreeViewItem();
                category.Header = "All";
                category.Tag = "Industry";
                if (Utils.logoFilters != null && Utils.logoFilters.Count > 0)
                {
                    category.Header = Utils.logoFilters[1];
                    category.Tag = Utils.logoFilters[1];
                }
                category.IsExpanded = true;
                objTreeView.Items.Add(category);
                foreach (DataLevelModel dataRow in lstIndustryFilterMetaDataLvl1)
                {
                    RadTreeViewItem treeRoot = new RadTreeViewItem();
                    int count = dataRow.FilterCount;//ManageCompanyMasterDataCollection.Where(x => x.Filter2_1.Contains(dataRow.Name.ToString())).ToList().Count;
                    treeRoot.Header = dataRow.Name.ToString() + " (" + count + ")";
                    treeRoot.Tag = dataRow.Id + "," + 1;
                    treeRoot.ToolTip = treeRoot.Header;

                    category.Items.Add(treeRoot);

                    foreach (RadTreeViewItem childnode in GetChildNodeForLvl2FromDB(Convert.ToInt64(dataRow.Id), lstIndustryFilterMetaDataLvl2, lstIndustryFilterMetaDataLvl3, lstIndustryFilterMetaDataLvl4, filterType))
                    {
                        childnode.ToolTip = childnode.Header;
                        treeRoot.Items.Add(childnode);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.LogError("Exception inside  PopulateSectorIndustryTreeFromDB : " + ex.Message + ", " + ex.InnerException);
            }
        }

        private void FreeFilter1()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    IsActiveFilter1 = ManageFilterGroupCollection.Where(x => x.FilterId == 3).Select(x => x.IsStatus).FirstOrDefault();
                    if (IsActiveFilter1)
                    {
                        filter3Expander.Visibility = Visibility.Visible;
                        List<DataLevelModel> lstFilter1FilterMetaDataLvl1 = ManageFilterMetaDataLevel1Collection.Where(x => x.FilterGroupId == (int)(EnumGeoMasterFilter.Filter1)).ToList();
                        List<DataLevelModel> lstFilter1FilterMetaDataLvl2 = ManageFilterMetaDataLevel2Collection.Where(x => x.FilterGroupId == (int)(EnumGeoMasterFilter.Filter1)).ToList();
                        List<DataLevelModel> lstFilter1FilterMetaDataLvl3 = ManageFilterMetaDataLevel3Collection.Where(x => x.FilterGroupId == (int)(EnumGeoMasterFilter.Filter1)).ToList();
                        List<DataLevelModel> lstFilter1FilterMetaDataLvl4 = ManageFilterMetaDataLevel4Collection.Where(x => x.FilterGroupId == (int)(EnumGeoMasterFilter.Filter1)).ToList();

                        PopulateFilter1TreeFromDB(radFreeFilter1TreeView, lstFilter1FilterMetaDataLvl1, lstFilter1FilterMetaDataLvl2, lstFilter1FilterMetaDataLvl3, lstFilter1FilterMetaDataLvl4);
                    }
                    else
                    {
                        filter3Expander.Visibility = Visibility.Collapsed;
                        Filter3_1Column.IsVisible = false;
                    }
                });
            }
            catch (Exception ex)
            {
                Utils.LogError("Exception inside  FreeFilter1 : " + ex.Message + ", " + ex.InnerException);
            }
        }
        private void PopulateFilter1TreeFromDB(Telerik.Windows.Controls.RadTreeView objTreeView, List<DataLevelModel> lstFilter1FilterMetaDataLvl1,
            List<DataLevelModel> lstFilter1FilterMetaDataLvl2, List<DataLevelModel> lstFilter1FilterMetaDataLvl3, List<DataLevelModel> lstFilter1FilterMetaDataLvl4)
        {
            try
            {
                string filterType = EnumGeoMasterFilter.Filter1.ToString();
                RadTreeViewItem category = new RadTreeViewItem();
                category.Header = "All";
                category.Tag = "FreeFilter1";
                if (Utils.logoFilters != null && Utils.logoFilters.Count > 0)
                {
                    category.Header = Utils.logoFilters[2];
                    category.Tag = Utils.logoFilters[2];
                }
                category.IsExpanded = true;
                objTreeView.Items.Add(category);
                foreach (DataLevelModel dataRow in lstFilter1FilterMetaDataLvl1)
                {
                    RadTreeViewItem treeRoot = new RadTreeViewItem();
                    int count = dataRow.FilterCount;//ManageCompanyMasterDataCollection.Where(x => x.Filter3_1.Contains(dataRow.Name.ToString())).ToList().Count;
                    treeRoot.Header = dataRow.Name.ToString() + " (" + count + ")";
                    treeRoot.Tag = dataRow.Id + "," + 1;
                    treeRoot.ToolTip = treeRoot.Header;

                    category.Items.Add(treeRoot);

                    foreach (RadTreeViewItem childnode in GetChildNodeForLvl2FromDB(Convert.ToInt64(dataRow.Id), lstFilter1FilterMetaDataLvl2, lstFilter1FilterMetaDataLvl3, lstFilter1FilterMetaDataLvl4, filterType))
                    {
                        childnode.ToolTip = childnode.Header;
                        treeRoot.Items.Add(childnode);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.LogError("Exception inside  PopulateFilter1TreeFromDB : " + ex.Message + ", " + ex.InnerException);
            }
        }

        private void FreeFilter2()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    IsActiveFilter2 = ManageFilterGroupCollection.Where(x => x.FilterId == 4).Select(x => x.IsStatus).FirstOrDefault();
                    if (IsActiveFilter2)
                    {
                        filter4Expander.Visibility = Visibility.Visible;
                        List<DataLevelModel> lstFilter2FilterMetaDataLvl1 = ManageFilterMetaDataLevel1Collection.Where(x => x.FilterGroupId == (int)(EnumGeoMasterFilter.Filter2)).ToList();
                        List<DataLevelModel> lstFilter2FilterMetaDataLvl2 = ManageFilterMetaDataLevel2Collection.Where(x => x.FilterGroupId == (int)(EnumGeoMasterFilter.Filter2)).ToList();
                        List<DataLevelModel> lstFilter2FilterMetaDataLvl3 = ManageFilterMetaDataLevel3Collection.Where(x => x.FilterGroupId == (int)(EnumGeoMasterFilter.Filter2)).ToList();
                        List<DataLevelModel> lstFilter2FilterMetaDataLvl4 = ManageFilterMetaDataLevel4Collection.Where(x => x.FilterGroupId == (int)(EnumGeoMasterFilter.Filter2)).ToList();

                        PopulateFilter2TreeFromDB(radFreeFilter2TreeView, lstFilter2FilterMetaDataLvl1, lstFilter2FilterMetaDataLvl2, lstFilter2FilterMetaDataLvl3, lstFilter2FilterMetaDataLvl4);
                    }
                    else
                    {
                        filter4Expander.Visibility = Visibility.Collapsed;
                        Filter4_1Column.IsVisible = false;
                    }
                });
            }
            catch (Exception ex)
            {
                Utils.LogError("Exception inside  FreeFilter1 : " + ex.Message + ", " + ex.InnerException);
            }
        }
        private void PopulateFilter2TreeFromDB(Telerik.Windows.Controls.RadTreeView objTreeView, List<DataLevelModel> lstFilter2FilterMetaDataLvl1,
            List<DataLevelModel> lstFilter2FilterMetaDataLvl2, List<DataLevelModel> lstFilter2FilterMetaDataLvl3, List<DataLevelModel> lstFilter2FilterMetaDataLvl4)
        {
            try
            {
                string filterType = EnumGeoMasterFilter.Filter2.ToString();
                RadTreeViewItem category = new RadTreeViewItem();
                category.Header = "All";
                category.Tag = "FreeFilter2";
                if (Utils.logoFilters != null && Utils.logoFilters.Count > 0)
                {
                    category.Header = Utils.logoFilters[3];
                    category.Tag = Utils.logoFilters[3];
                }
                category.IsExpanded = true;
                objTreeView.Items.Add(category);
                foreach (DataLevelModel dataRow in lstFilter2FilterMetaDataLvl1)
                {
                    RadTreeViewItem treeRoot = new RadTreeViewItem();
                    int count = dataRow.FilterCount;//ManageCompanyMasterDataCollection.Where(x => x.Filter4_1.Contains(dataRow.Name.ToString())).ToList().Count;
                    treeRoot.Header = dataRow.Name.ToString() + " (" + count + ")";
                    treeRoot.Tag = dataRow.Id + "," + 1;
                    treeRoot.ToolTip = treeRoot.Header;

                    category.Items.Add(treeRoot);

                    foreach (RadTreeViewItem childnode in GetChildNodeForLvl2FromDB(Convert.ToInt64(dataRow.Id), lstFilter2FilterMetaDataLvl2, lstFilter2FilterMetaDataLvl3, lstFilter2FilterMetaDataLvl4, filterType))
                    {
                        childnode.ToolTip = childnode.Header;
                        treeRoot.Items.Add(childnode);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.LogError("Exception inside  PopulateFilter2TreeFromDB : " + ex.Message + ", " + ex.InnerException);
            }
        }

        private string generateQuery(List<RadTreeViewItem> baseLevelItems, string level, string expression)
        {
            foreach (RadTreeViewItem item in baseLevelItems)
            {
                expression = expression + level + " = " + "'" + item.Header + "'" + " OR ";
            }

            if (expression.EndsWith(" OR "))
                expression = expression.Remove(expression.Length - 4);

            expression = expression + ")" + " AND " + "(";

            return expression;
        }

        private void editColumnChooser()
        {
            try
            {
                var IsFilterOneActive = ManageFilterGroupCollection.Where(x => x.FilterId == 1).Select(x => x.IsStatus).FirstOrDefault();
                var IsFilterTwoActive = ManageFilterGroupCollection.Where(x => x.FilterId == 2).Select(x => x.IsStatus).FirstOrDefault();
                var IsFilterThreeActive = ManageFilterGroupCollection.Where(x => x.FilterId == 3).Select(x => x.IsStatus).FirstOrDefault();
                var IsFilterFourActive = ManageFilterGroupCollection.Where(x => x.FilterId == 4).Select(x => x.IsStatus).FirstOrDefault();

                var ClmCartList = lbColumnChooser.ItemsSource.Cast<object>().ToList();

                Telerik.Windows.Controls.GridViewSelectColumn col1 = ClmCartList[0] as Telerik.Windows.Controls.GridViewSelectColumn;

                if (col1 != null && col1.Name == "selectCol")
                    ClmCartList.RemoveAt(0);

                List<dynamic> colToRemove = new List<dynamic>();
                foreach (Telerik.Windows.Controls.GridViewDataColumn col in ClmCartList)
                {
                    if (col != null &&
                       (col.Header.ToString().Contains("Geo1") || col.Header.ToString().Contains("Geo2") || col.Header.ToString().Contains("Geo3") || col.Header.ToString().Contains("Geo4")
                        || col.Header.ToString().Contains("Ind1") || col.Header.ToString().Contains("Ind2") || col.Header.ToString().Contains("Ind3") || col.Header.ToString().Contains("Ind4")))
                    {
                        colToRemove.Add(col);
                    }
                }

                foreach (Telerik.Windows.Controls.GridViewDataColumn col in colToRemove)
                {
                    ClmCartList.Remove(col);
                }

                List<dynamic> newColToRemove = new List<dynamic>();
                foreach (Telerik.Windows.Controls.GridViewDataColumn col in ClmCartList)
                {
                    if (!IsFilterOneActive && col.Header.ToString().Equals(GeographyMasterDisplayName))
                        newColToRemove.Add(col);

                    if (!IsFilterTwoActive && col.Header.ToString().Equals(IndustryMasterDisplayName))
                        newColToRemove.Add(col);

                    if (!IsFilterThreeActive && col.Header.ToString().Equals(FreeFilter1MasterDisplayName))
                        newColToRemove.Add(col);

                    if (!IsFilterFourActive && col.Header.ToString().Equals(FreeFilter2MasterDisplayName))
                        newColToRemove.Add(col);
                }

                foreach (Telerik.Windows.Controls.GridViewDataColumn col in newColToRemove)
                {
                    ClmCartList.Remove(col);
                }

                lbColumnChooser.ItemsSource = ClmCartList;
            }
            catch (Exception ex)
            {
                Utils.LogError(ex.ToString());
            }
        }

        private void shrinkGrid()
        {

            int columnSpan = 1;
            int setCoulmn = 2;
            Visibility visible = Visibility.Visible;
            int column = Grid.GetColumn(previewColumn);


            setCoulmn = 2;
            visible = Visibility.Visible;
            columnSpan = 1;
            if (ProjectType == "PitchreadyExcel")
                expanderImg.ImageSource = expanderImg.ImageSource = (ImageSource)FindResource("Excel_expand_logoDrawingImage");
            logoExpander.ToolTip = "Expand";


            gridColumn.Visibility = visible;
            Grid.SetColumn(previewColumn, setCoulmn);
            Grid.SetColumnSpan(previewColumn, columnSpan);
        }

        private void selectItemsBasedOnCart()
        {
            radGridViewGeoAndIndustry.SelectionChanged -= radGridViewGeoAndIndustry_SelectionChanged;

            foreach (LogoTileVM row in CartSource)
            {
                IEnumerable<LogoToolCompanyDataFrontEnd> rows = radGridViewGeoAndIndustry.Items.Cast<LogoToolCompanyDataFrontEnd>()
                 .Where(row1 => row1.ID == row.LogoData.ID).ToList();

                if (rows.Count() != 0)
                    radGridViewGeoAndIndustry.SelectedItems.Add(rows.First());
            }
            radGridViewGeoAndIndustry.SelectionChanged += radGridViewGeoAndIndustry_SelectionChanged;
        }
        private Collection<RadTreeViewItem> GetContainers(Telerik.Windows.Controls.RadTreeView radTreeView)
        {
            // gets all nodes from the TreeView   
            Collection<RadTreeViewItem> allTreeContainers = GetAllItemContainers(radTreeView);
            // gets all nodes (recursively) for the first node   
            RadTreeViewItem firstNode = radTreeView.ItemContainerGenerator.ContainerFromIndex(0) as RadTreeViewItem;
            if (firstNode != null)
            {
                Collection<RadTreeViewItem> firstNodeContainers = GetAllItemContainers(firstNode);
                return firstNodeContainers;
            }
            return null;

        }

        private Collection<RadTreeViewItem> GetAllItemContainers(System.Windows.Controls.ItemsControl itemsControl)
        {
            Collection<RadTreeViewItem> allItems = new Collection<RadTreeViewItem>();
            for (int i = 0; i < itemsControl.Items.Count; i++)
            {
                // try to get the item Container   
                RadTreeViewItem childItemContainer = itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as RadTreeViewItem;
                // the item container maybe null if it is still not generated from the runtime   
                if (childItemContainer != null)
                {
                    if (childItemContainer.CheckState == System.Windows.Automation.ToggleState.On)
                        allItems.Add(childItemContainer);
                    Collection<RadTreeViewItem> childItems = GetAllItemContainers(childItemContainer);
                    foreach (RadTreeViewItem childItem in childItems)
                    {
                        if (childItem.CheckState == System.Windows.Automation.ToggleState.On)
                            allItems.Add(childItem);
                    }
                }
            }
            return allItems;
        }


        private void SearchCompanyBasedOnSectorIndustryAndGeography()
        {
            try
            {
                IndustrySearchFilterText = JoinedQueryString(SelectedSectors, SelectedIndustryGroups, SelectedIndustries, SelectedSubIndustries);
                GeographySearchFilterText = JoinedQueryString(SelectedContinents, SelectedSubContinents, SelectedRegions, SelectedCountries);
                FreeFilter1SearchFilterText = JoinedQueryString(SelectedFilter3_1, SelectedFilter3_2, SelectedFilter3_3, SelectedFilter3_4);
                FreeFilter2SearchFilterText = JoinedQueryString(SelectedFilter4_1, SelectedFilter4_2, SelectedFilter4_3, SelectedFilter4_4);

                selected.Rows.Clear();
                if (ManageCompanyMasterDataCollection.Count > 0)
                {
                    string filterExpression = string.Empty;
                    if (!string.IsNullOrEmpty(IndustrySearchFilterText))
                        filterExpression = IndustrySearchFilterText.Trim();

                    if (string.IsNullOrEmpty(filterExpression))
                        filterExpression = GeographySearchFilterText.Trim();
                    else if (!string.IsNullOrEmpty(GeographySearchFilterText))
                        filterExpression = filterExpression + "|" + GeographySearchFilterText.Trim();

                    if (string.IsNullOrEmpty(filterExpression))
                        filterExpression = FreeFilter1SearchFilterText.Trim();
                    else if (!string.IsNullOrEmpty(FreeFilter1SearchFilterText))
                        filterExpression = filterExpression + "|" + FreeFilter1SearchFilterText.Trim();

                    if (string.IsNullOrEmpty(filterExpression))
                        filterExpression = FreeFilter2SearchFilterText.Trim();
                    else if (!string.IsNullOrEmpty(FreeFilter2SearchFilterText))
                        filterExpression = filterExpression + "|" + FreeFilter2SearchFilterText.Trim();

                    if (!string.IsNullOrEmpty(filterExpression))
                    {
                        string value = string.Empty;
                        List<LogoToolCompanyDataFrontEnd> selectedRows = new List<LogoToolCompanyDataFrontEnd>();
                        if (filterExpression.Contains('|'))
                        {
                            string[] filterList = filterExpression.Split('|');
                            foreach (var item in filterList)
                            {
                                value = item.Split('=')[1].ToString().Trim();
                                if (item.Contains("Filter1_1"))
                                {
                                    foreach (var data in ManageCompanyMasterDataCollection.Where(x => x.Filter1_1.Contains(value)))
                                    {
                                        if (!selectedRows.Contains(data))
                                        {
                                            if (data.Filter1_1.Contains('|'))
                                            {
                                                string[] filtersValue = data.Filter1_1.Split('|');
                                                foreach (string val in filtersValue)
                                                {
                                                    if (value.Equals(val.Trim()))
                                                        selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                                }
                                            }
                                            else
                                            {
                                                if (data.Filter1_1.Equals(value))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                    }
                                }
                                else if (item.Contains("Filter1_2"))
                                {
                                    foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter1_2.Contains(value)))
                                    {
                                        var result = selectedRows.Where(x => x.ID == data.ID);
                                        if (result.Count() == 0)
                                        {
                                            if (data.Filter1_2.Contains('|'))
                                            {
                                                string[] filtersValue = data.Filter1_2.Split('|');
                                                foreach (string val in filtersValue)
                                                {
                                                    if (value.Equals(val.Trim()))
                                                        selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                                }
                                            }
                                            else
                                            {
                                                if (data.Filter1_2.Equals(value))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                    }
                                }
                                else if (item.Contains("Filter1_3"))
                                {
                                    foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter1_3.Contains(value)))
                                    {
                                        var result = selectedRows.Where(x => x.ID == data.ID);
                                        if (result.Count() == 0)
                                        {
                                            if (data.Filter1_3.Contains('|'))
                                            {
                                                string[] filtersValue = data.Filter1_3.Split('|');
                                                foreach (string val in filtersValue)
                                                {
                                                    if (value.Equals(val.Trim()))
                                                        selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                                }
                                            }
                                            else
                                            {
                                                if (data.Filter1_3.Equals(value))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                    }
                                }
                                else if (item.Contains("Filter1_4"))
                                {
                                    foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter1_4.Contains(value)))
                                    {
                                        var result = selectedRows.Where(x => x.ID == data.ID);
                                        if (result.Count() == 0)
                                        {
                                            if (data.Filter1_4.Contains('|'))
                                            {
                                                string[] filtersValue = data.Filter1_4.Split('|');
                                                foreach (string val in filtersValue)
                                                {
                                                    if (value.Equals(val.Trim()))
                                                        selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                                }
                                            }
                                            else
                                            {
                                                if (data.Filter1_4.Equals(value))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                    }
                                }

                                if (item.Contains("Filter2_1"))
                                {
                                    foreach (var data in ManageCompanyMasterDataCollection.Where(x => x.Filter2_1.Contains(value)))
                                    {
                                        if (!selectedRows.Contains(data))
                                        {
                                            if (data.Filter2_1.Contains('|'))
                                            {
                                                string[] filtersValue = data.Filter2_1.Split('|');
                                                foreach (string val in filtersValue)
                                                {
                                                    if (value.Equals(val.Trim()))
                                                        selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                                }
                                            }
                                            else
                                            {
                                                if (data.Filter2_1.Equals(value))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                    }
                                }
                                else if (item.Contains("Filter2_2"))
                                {
                                    foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter2_2.Contains(value)))
                                    {
                                        var result = selectedRows.Where(x => x.ID == data.ID);
                                        if (result.Count() == 0)
                                        {
                                            if (data.Filter2_2.Contains('|'))
                                            {
                                                string[] filtersValue = data.Filter2_2.Split('|');
                                                foreach (string val in filtersValue)
                                                {
                                                    if (value.Equals(val.Trim()))
                                                        selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                                }
                                            }
                                            else
                                            {
                                                if (data.Filter2_2.Equals(value))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                    }
                                }
                                else if (item.Contains("Filter2_3"))
                                {
                                    foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter2_3.Contains(value)))
                                    {
                                        var result = selectedRows.Where(x => x.ID == data.ID);
                                        if (result.Count() == 0)
                                        {
                                            if (data.Filter2_3.Contains('|'))
                                            {
                                                string[] filtersValue = data.Filter2_3.Split('|');
                                                foreach (string val in filtersValue)
                                                {
                                                    if (value.Equals(val.Trim()))
                                                        selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                                }
                                            }
                                            else
                                            {
                                                if (data.Filter2_3.Equals(value))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                    }
                                }
                                else if (item.Contains("Filter2_4"))
                                {
                                    foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter2_4.Contains(value)))
                                    {
                                        var result = selectedRows.Where(x => x.ID == data.ID);
                                        if (result.Count() == 0)
                                        {
                                            if (data.Filter2_4.Contains('|'))
                                            {
                                                string[] filtersValue = data.Filter2_4.Split('|');
                                                foreach (string val in filtersValue)
                                                {
                                                    if (value.Equals(val.Trim()))
                                                        selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                                }
                                            }
                                            else
                                            {
                                                if (data.Filter2_4.Equals(value))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                    }
                                }

                                if (item.Contains("Filter3_1"))
                                {
                                    foreach (var data in ManageCompanyMasterDataCollection.Where(x => x.Filter3_1.Contains(value)))
                                    {
                                        if (!selectedRows.Contains(data))
                                        {
                                            if (data.Filter3_1.Contains('|'))
                                            {
                                                string[] filtersValue = data.Filter3_1.Split('|');
                                                foreach (string val in filtersValue)
                                                {
                                                    if (value.Equals(val.Trim()))
                                                        selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                                }
                                            }
                                            else
                                            {
                                                if (data.Filter3_1.Equals(value))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                    }
                                }
                                else if (item.Contains("Filter3_2"))
                                {
                                    foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter3_2.Contains(value)))
                                    {
                                        var result = selectedRows.Where(x => x.ID == data.ID);
                                        if (result.Count() == 0)
                                        {
                                            if (data.Filter3_2.Contains('|'))
                                            {
                                                string[] filtersValue = data.Filter3_2.Split('|');
                                                foreach (string val in filtersValue)
                                                {
                                                    if (value.Equals(val.Trim()))
                                                        selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                                }
                                            }
                                            else
                                            {
                                                if (data.Filter3_2.Equals(value))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                    }
                                }
                                else if (item.Contains("Filter3_3"))
                                {
                                    foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter3_3.Contains(value)))
                                    {
                                        var result = selectedRows.Where(x => x.ID == data.ID);
                                        if (result.Count() == 0)
                                        {
                                            if (data.Filter3_3.Contains('|'))
                                            {
                                                string[] filtersValue = data.Filter3_3.Split('|');
                                                foreach (string val in filtersValue)
                                                {
                                                    if (value.Equals(val.Trim()))
                                                        selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                                }
                                            }
                                            else
                                            {
                                                if (data.Filter3_3.Equals(value))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                    }
                                }
                                else if (item.Contains("Filter3_4"))
                                {
                                    foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter3_4.Contains(value)))
                                    {
                                        var result = selectedRows.Where(x => x.ID == data.ID);
                                        if (result.Count() == 0)
                                        {
                                            if (data.Filter3_4.Contains('|'))
                                            {
                                                string[] filtersValue = data.Filter3_4.Split('|');
                                                foreach (string val in filtersValue)
                                                {
                                                    if (value.Equals(val.Trim()))
                                                        selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                                }
                                            }
                                            else
                                            {
                                                if (data.Filter3_4.Equals(value))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                    }
                                }

                                if (item.Contains("Filter4_1"))
                                {
                                    foreach (var data in ManageCompanyMasterDataCollection.Where(x => x.Filter4_1.Contains(value)))
                                    {
                                        if (!selectedRows.Contains(data))
                                        {
                                            if (data.Filter4_1.Contains('|'))
                                            {
                                                string[] filtersValue = data.Filter4_1.Split('|');
                                                foreach (string val in filtersValue)
                                                {
                                                    if (value.Equals(val.Trim()))
                                                        selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                                }
                                            }
                                            else
                                            {
                                                if (data.Filter4_1.Equals(value))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                    }
                                }
                                else if (item.Contains("Filter4_2"))
                                {
                                    foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter4_2.Contains(value)))
                                    {
                                        var result = selectedRows.Where(x => x.ID == data.ID);
                                        if (result.Count() == 0)
                                        {
                                            if (data.Filter4_2.Contains('|'))
                                            {
                                                string[] filtersValue = data.Filter4_2.Split('|');
                                                foreach (string val in filtersValue)
                                                {
                                                    if (value.Equals(val.Trim()))
                                                        selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                                }
                                            }
                                            else
                                            {
                                                if (data.Filter4_2.Equals(value))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                    }
                                }
                                else if (item.Contains("Filter4_3"))
                                {
                                    foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter4_3.Contains(value)))
                                    {
                                        var result = selectedRows.Where(x => x.ID == data.ID);
                                        if (result.Count() == 0)
                                        {
                                            if (data.Filter4_3.Contains('|'))
                                            {
                                                string[] filtersValue = data.Filter4_3.Split('|');
                                                foreach (string val in filtersValue)
                                                {
                                                    if (value.Equals(val.Trim()))
                                                        selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                                }
                                            }
                                            else
                                            {
                                                if (data.Filter4_3.Equals(value))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                    }
                                }
                                else if (item.Contains("Filter4_4"))
                                {
                                    foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter4_4.Contains(value)))
                                    {
                                        var result = selectedRows.Where(x => x.ID == data.ID);
                                        if (result.Count() == 0)
                                        {
                                            if (data.Filter4_4.Contains('|'))
                                            {
                                                string[] filtersValue = data.Filter4_4.Split('|');
                                                foreach (string val in filtersValue)
                                                {
                                                    if (value.Equals(val.Trim()))
                                                        selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                                }
                                            }
                                            else
                                            {
                                                if (data.Filter4_4.Equals(value))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            value = filterExpression.Split('=')[1].ToString().Trim();
                            if (filterExpression.Contains("Filter1_1"))
                            {
                                foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter1_1.Contains(value)))
                                {
                                    var result = selectedRows.Where(x => x.ID == data.ID);
                                    if (result.Count() == 0)
                                    {
                                        if (data.Filter1_1.Contains('|'))
                                        {
                                            string[] filtersValue = data.Filter1_1.Split('|');
                                            foreach (string val in filtersValue)
                                            {
                                                if (value.Equals(val.Trim()))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                        else
                                        {
                                            if (data.Filter1_1.Equals(value))
                                                selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                        }
                                    }
                                }
                            }
                            if (filterExpression.Contains("Filter1_2"))
                            {
                                foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter1_2.Contains(value)))
                                {
                                    var result = selectedRows.Where(x => x.ID == data.ID);
                                    if (result.Count() == 0)
                                    {
                                        if (data.Filter1_2.Contains('|'))
                                        {
                                            string[] filtersValue = data.Filter1_2.Split('|');
                                            foreach (string val in filtersValue)
                                            {
                                                if (value.Equals(val.Trim()))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                        else
                                        {
                                            if (data.Filter1_2.Equals(value))
                                                selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                        }
                                    }
                                }
                            }
                            if (filterExpression.Contains("Filter1_3"))
                            {
                                foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter1_3.Contains(value)))
                                {
                                    var result = selectedRows.Where(x => x.ID == data.ID);
                                    if (result.Count() == 0)
                                    {
                                        if (data.Filter1_3.Contains('|'))
                                        {
                                            string[] filtersValue = data.Filter1_3.Split('|');
                                            foreach (string val in filtersValue)
                                            {
                                                if (value.Equals(val.Trim()))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                        else
                                        {
                                            if (data.Filter1_3.Equals(value))
                                                selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                        }
                                    }
                                }
                            }
                            if (filterExpression.Contains("Filter1_4"))
                            {
                                foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter1_4.Contains(value)))
                                {
                                    var result = selectedRows.Where(x => x.ID == data.ID);
                                    if (result.Count() == 0)
                                    {
                                        if (data.Filter1_4.Contains('|'))
                                        {
                                            string[] filtersValue = data.Filter1_4.Split('|');
                                            foreach (string val in filtersValue)
                                            {
                                                if (value.Equals(val.Trim()))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                        else
                                        {
                                            if (data.Filter1_4.Equals(value))
                                                selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                        }
                                    }
                                }
                            }
                            if (filterExpression.Contains("Filter2_1"))
                            {
                                foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter2_1.Contains(value)))
                                {
                                    var result = selectedRows.Where(x => x.ID == data.ID);
                                    if (result.Count() == 0)
                                    {
                                        if (data.Filter2_1.Contains('|'))
                                        {
                                            string[] filtersValue = data.Filter2_1.Split('|');
                                            foreach (string val in filtersValue)
                                            {
                                                if (value.Equals(val.Trim()))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                        else
                                        {
                                            if (data.Filter2_1.Equals(value))
                                                selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                        }
                                    }
                                }
                            }
                            if (filterExpression.Contains("Filter2_2"))
                            {
                                foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter2_2.Contains(value)))
                                {
                                    var result = selectedRows.Where(x => x.ID == data.ID);
                                    if (result.Count() == 0)
                                    {
                                        if (data.Filter2_2.Contains('|'))
                                        {
                                            string[] filtersValue = data.Filter2_2.Split('|');
                                            foreach (string val in filtersValue)
                                            {
                                                if (value.Equals(val.Trim()))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                        else
                                        {
                                            if (data.Filter2_2.Equals(value))
                                                selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                        }
                                    }
                                }
                            }
                            if (filterExpression.Contains("Filter2_3"))
                            {
                                foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter2_3.Contains(value)))
                                {
                                    var result = selectedRows.Where(x => x.ID == data.ID);
                                    if (result.Count() == 0)
                                    {
                                        if (data.Filter2_3.Contains('|'))
                                        {
                                            string[] filtersValue = data.Filter2_3.Split('|');
                                            foreach (string val in filtersValue)
                                            {
                                                if (value.Equals(val.Trim()))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                        else
                                        {
                                            if (data.Filter2_3.Equals(value))
                                                selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                        }
                                    }
                                }
                            }
                            if (filterExpression.Contains("Filter2_4"))
                            {
                                foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter2_4.Contains(value)))
                                {
                                    var result = selectedRows.Where(x => x.ID == data.ID);
                                    if (result.Count() == 0)
                                    {
                                        if (data.Filter2_4.Contains('|'))
                                        {
                                            string[] filtersValue = data.Filter2_4.Split('|');
                                            foreach (string val in filtersValue)
                                            {
                                                if (value.Equals(val.Trim()))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                        else
                                        {
                                            if (data.Filter2_4.Equals(value))
                                                selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                        }
                                    }
                                }
                            }
                            if (filterExpression.Contains("Filter3_1"))
                            {
                                foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter3_1.Contains(value)))
                                {
                                    var result = selectedRows.Where(x => x.ID == data.ID);
                                    if (result.Count() == 0)
                                    {
                                        if (data.Filter3_1.Contains('|'))
                                        {
                                            string[] filtersValue = data.Filter3_1.Split('|');
                                            foreach (string val in filtersValue)
                                            {
                                                if (value.Equals(val.Trim()))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                        else
                                        {
                                            if (data.Filter3_1.Equals(value))
                                                selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                        }
                                    }
                                }
                            }
                            if (filterExpression.Contains("Filter3_2"))
                            {
                                foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter3_2.Contains(value)))
                                {
                                    var result = selectedRows.Where(x => x.ID == data.ID);
                                    if (result.Count() == 0)
                                    {
                                        if (data.Filter3_2.Contains('|'))
                                        {
                                            string[] filtersValue = data.Filter3_2.Split('|');
                                            foreach (string val in filtersValue)
                                            {
                                                if (value.Equals(val.Trim()))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                        else
                                        {
                                            if (data.Filter3_2.Equals(value))
                                                selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                        }
                                    }
                                }
                            }
                            if (filterExpression.Contains("Filter3_3"))
                            {
                                foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter3_3.Contains(value)))
                                {
                                    var result = selectedRows.Where(x => x.ID == data.ID);
                                    if (result.Count() == 0)
                                    {
                                        if (data.Filter3_3.Contains('|'))
                                        {
                                            string[] filtersValue = data.Filter3_3.Split('|');
                                            foreach (string val in filtersValue)
                                            {
                                                if (value.Equals(val.Trim()))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                        else
                                        {
                                            if (data.Filter3_3.Equals(value))
                                                selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                        }
                                    }
                                }
                            }
                            if (filterExpression.Contains("Filter3_4"))
                            {
                                foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter3_4.Contains(value)))
                                {
                                    var result = selectedRows.Where(x => x.ID == data.ID);
                                    if (result.Count() == 0)
                                    {
                                        if (data.Filter3_4.Contains('|'))
                                        {
                                            string[] filtersValue = data.Filter3_4.Split('|');
                                            foreach (string val in filtersValue)
                                            {
                                                if (value.Equals(val.Trim()))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                        else
                                        {
                                            if (data.Filter3_4.Equals(value))
                                                selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                        }
                                    }
                                }
                            }
                            if (filterExpression.Contains("Filter4_1"))
                            {
                                foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter4_1.Contains(value)))
                                {
                                    var result = selectedRows.Where(x => x.ID == data.ID);
                                    if (result.Count() == 0)
                                    {
                                        if (data.Filter4_1.Contains('|'))
                                        {
                                            string[] filtersValue = data.Filter4_1.Split('|');
                                            foreach (string val in filtersValue)
                                            {
                                                if (value.Equals(val.Trim()))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                        else
                                        {
                                            if (data.Filter4_1.Equals(value))
                                                selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                        }
                                    }
                                }
                            }
                            if (filterExpression.Contains("Filter4_2"))
                            {
                                foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter4_2.Contains(value)))
                                {
                                    var result = selectedRows.Where(x => x.ID == data.ID);
                                    if (result.Count() == 0)
                                    {
                                        if (data.Filter4_2.Contains('|'))
                                        {
                                            string[] filtersValue = data.Filter4_2.Split('|');
                                            foreach (string val in filtersValue)
                                            {
                                                if (value.Equals(val.Trim()))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                        else
                                        {
                                            if (data.Filter4_2.Equals(value))
                                                selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                        }
                                    }
                                }
                            }
                            if (filterExpression.Contains("Filter4_3"))
                            {
                                foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter4_3.Contains(value)))
                                {
                                    var result = selectedRows.Where(x => x.ID == data.ID);
                                    if (result.Count() == 0)
                                    {
                                        if (data.Filter4_3.Contains('|'))
                                        {
                                            string[] filtersValue = data.Filter4_3.Split('|');
                                            foreach (string val in filtersValue)
                                            {
                                                if (value.Equals(val.Trim()))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                        else
                                        {
                                            if (data.Filter4_3.Equals(value))
                                                selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                        }
                                    }
                                }
                            }
                            if (filterExpression.Contains("Filter4_4"))
                            {
                                foreach (LogoToolCompanyDataFrontEnd data in ManageCompanyMasterDataCollection.Where(x => x.Filter4_4.Contains(value)))
                                {
                                    var result = selectedRows.Where(x => x.ID == data.ID);
                                    if (result.Count() == 0)
                                    {
                                        if (data.Filter4_4.Contains('|'))
                                        {
                                            string[] filtersValue = data.Filter4_4.Split('|');
                                            foreach (string val in filtersValue)
                                            {
                                                if (value.Equals(val.Trim()))
                                                    selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                            }
                                        }
                                        else
                                        {
                                            if (data.Filter4_4.Equals(value))
                                                selectedRows.Add((LogoToolCompanyDataFrontEnd)data);
                                        }
                                    }
                                }
                            }
                        }

                        //if (selectedRows.Count() > 0)
                        {
                            LsvPageGlobVar.TotalRec = selectedRows.Count;
                            PopulateSearchResultCompanyListViewBasedOnSectorAndIndustry(new ObservableCollection<LogoToolCompanyDataFrontEnd>(selectedRows));

                        }
                    }
                    else
                    {
                        LsvPageGlobVar.TotalRec = ManageCompanyMasterDataCollection.Count;
                        PopulateSearchResultCompanyListViewBasedOnSectorAndIndustry(ManageCompanyMasterDataCollection);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.LogError("Exception inside  Geography : " + ex.Message + ", " + ex.InnerException);
            }
        }

        string JoinedQueryString(string param1, string param2, string param3, string param4)
        {
            string resultsting = string.Empty;
            resultsting = string.IsNullOrEmpty(param1) ? string.Empty : param1;
            resultsting = string.IsNullOrEmpty(resultsting) ? param2 : string.IsNullOrEmpty(param2) ? resultsting : resultsting + " | " + param2;
            resultsting = string.IsNullOrEmpty(resultsting) ? param3 : string.IsNullOrEmpty(param3) ? resultsting : resultsting + " | " + param3;
            resultsting = string.IsNullOrEmpty(resultsting) ? param4 : string.IsNullOrEmpty(param4) ? resultsting : resultsting + " | " + param4;


            return resultsting;
        }
        public static System.Data.DataTable ToDataTable<T>(List<T> items)
        {
            System.Data.DataTable dataTable = new System.Data.DataTable();
            //Get all the properties by using reflection   
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names  
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {

                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        void PopulateSearchResultCompanyListViewBasedOnSectorAndIndustry(ObservableCollection<LogoToolCompanyDataFrontEnd> selectedRows)
        {
            try
            {
                if (companyTab.IsSelected)
                {
                    radGridViewGeoAndIndustry.ItemsSource = null;
                    radGridViewGeoAndIndustry.ItemsSource = selectedRows;
                    if (selectedRows != null)
                    {
                        if (selectedRows.Count > 0)
                        {
                            LsvPageGlobVar.TotalRec = selectedRows.Count;
                            if (LsvPageGlobVar.RecStart + 100 > LsvPageGlobVar.TotalRec)
                            {
                                LsvPageGlobVar.RecEnd = LsvPageGlobVar.TotalRec;
                            }
                            else
                            {
                                LsvPageGlobVar.RecEnd = LsvPageGlobVar.RecStart + 99;
                            }

                            int showEndresult = LsvPageGlobVar.RecEnd + 1;
                            if (LsvPageGlobVar.RecEnd == LsvPageGlobVar.TotalRec)
                                showEndresult = LsvPageGlobVar.TotalRec;
                        }
                        else
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.LogError("Exception inside  PopulateSectorIndustryTree : " + ex.Message + ", " + ex.InnerException);
            }
        }

        private async Task SearchCompany()
        {
            Search = "";
            FilterDataModal filterDM = null;
            try
            {
                lstFilterDataModal.Clear();
                int counter = 1;
                if (lstGeography.Count > 0)
                {
                    foreach (RadTreeViewItem geography in lstGeography)
                    {
                        int ID = Convert.ToInt32(geography.Tag.ToString().Split(',')[0]);
                        int levelID = Convert.ToInt32(geography.Tag.ToString().Split(',')[1]);

                        if (levelID == 1)
                        {
                            filterDM = new FilterDataModal { Id = counter, FilterGroupId = 1, Id_Level1 = ID, Id_Level2 = null, Id_Level3 = null, Id_Level4 = null };
                        }
                        else if (levelID == 2)
                        {
                            filterDM = new FilterDataModal { Id = counter, FilterGroupId = 1, Id_Level1 = null, Id_Level2 = ID, Id_Level3 = null, Id_Level4 = null };
                        }
                        else if (levelID == 3)
                        {
                            filterDM = new FilterDataModal { Id = counter, FilterGroupId = 1, Id_Level1 = null, Id_Level2 = null, Id_Level3 = ID, Id_Level4 = null };
                        }
                        else if (levelID == 4)
                        {
                            filterDM = new FilterDataModal { Id = counter, FilterGroupId = 1, Id_Level1 = null, Id_Level2 = null, Id_Level3 = null, Id_Level4 = ID };
                        }
                        lstFilterDataModal.Add(filterDM);
                        counter++;
                        //GetDataForSearchGeographyOnly(geography.Tag.ToString().Split(',')[1], geography.Header.ToString().Split('(')[0].Trim());
                    }
                }
                if (lstSectorIndustry.Count > 0)
                {
                    foreach (RadTreeViewItem industry in lstSectorIndustry)
                    {
                        int ID = Convert.ToInt32(industry.Tag.ToString().Split(',')[0]);
                        int levelID = Convert.ToInt32(industry.Tag.ToString().Split(',')[1]);

                        if (levelID == 1)
                        {
                            filterDM = new FilterDataModal { Id = counter, FilterGroupId = 2, Id_Level1 = ID, Id_Level2 = null, Id_Level3 = null, Id_Level4 = null };
                        }
                        else if (levelID == 2)
                        {
                            filterDM = new FilterDataModal { Id = counter, FilterGroupId = 2, Id_Level1 = null, Id_Level2 = ID, Id_Level3 = null, Id_Level4 = null };
                        }
                        else if (levelID == 3)
                        {
                            filterDM = new FilterDataModal { Id = counter, FilterGroupId = 2, Id_Level1 = null, Id_Level2 = null, Id_Level3 = ID, Id_Level4 = null };
                        }
                        else if (levelID == 4)
                        {
                            filterDM = new FilterDataModal { Id = counter, FilterGroupId = 2, Id_Level1 = null, Id_Level2 = null, Id_Level3 = null, Id_Level4 = ID };
                        }
                        lstFilterDataModal.Add(filterDM);
                        counter++;
                        //GetDataForSearchForIndustryOnly(industry, industry.Tag.ToString().Split(',')[1], industry.Header.ToString().Split('(')[0].Trim());
                    }
                }
                if (lstFreeFilter1.Count > 0)
                {
                    foreach (RadTreeViewItem freeFilter1 in lstFreeFilter1)
                    {
                        int ID = Convert.ToInt32(freeFilter1.Tag.ToString().Split(',')[0]);
                        int levelID = Convert.ToInt32(freeFilter1.Tag.ToString().Split(',')[1]);

                        if (levelID == 1)
                        {
                            filterDM = new FilterDataModal { Id = counter, FilterGroupId = 3, Id_Level1 = ID, Id_Level2 = null, Id_Level3 = null, Id_Level4 = null };
                        }
                        else if (levelID == 2)
                        {
                            filterDM = new FilterDataModal { Id = counter, FilterGroupId = 3, Id_Level1 = null, Id_Level2 = ID, Id_Level3 = null, Id_Level4 = null };
                        }
                        else if (levelID == 3)
                        {
                            filterDM = new FilterDataModal { Id = counter, FilterGroupId = 3, Id_Level1 = null, Id_Level2 = null, Id_Level3 = ID, Id_Level4 = null };
                        }
                        else if (levelID == 4)
                        {
                            filterDM = new FilterDataModal { Id = counter, FilterGroupId = 3, Id_Level1 = null, Id_Level2 = null, Id_Level3 = null, Id_Level4 = ID };
                        }
                        lstFilterDataModal.Add(filterDM);
                        counter++;
                        //GetDataForSearchFreeFilter1Only(freeFilter1.Tag.ToString().Split(',')[1], freeFilter1.Header.ToString().Split('(')[0].Trim());
                    }
                }
                if (lstFreeFilter2.Count > 0)
                {
                    foreach (RadTreeViewItem freeFilter2 in lstFreeFilter2)
                    {
                        int ID = Convert.ToInt32(freeFilter2.Tag.ToString().Split(',')[0]);
                        int levelID = Convert.ToInt32(freeFilter2.Tag.ToString().Split(',')[1]);

                        if (levelID == 1)
                        {
                            filterDM = new FilterDataModal { Id = counter, FilterGroupId = 4, Id_Level1 = ID, Id_Level2 = null, Id_Level3 = null, Id_Level4 = null };
                        }
                        else if (levelID == 2)
                        {
                            filterDM = new FilterDataModal { Id = counter, FilterGroupId = 4, Id_Level1 = null, Id_Level2 = ID, Id_Level3 = null, Id_Level4 = null };
                        }
                        else if (levelID == 3)
                        {
                            filterDM = new FilterDataModal { Id = counter, FilterGroupId = 4, Id_Level1 = null, Id_Level2 = null, Id_Level3 = ID, Id_Level4 = null };
                        }
                        else if (levelID == 4)
                        {
                            filterDM = new FilterDataModal { Id = counter, FilterGroupId = 4, Id_Level1 = null, Id_Level2 = null, Id_Level3 = null, Id_Level4 = ID };
                        }
                        lstFilterDataModal.Add(filterDM);
                        counter++;
                        //GetDataForSearchFreeFilter2Only(freeFilter2.Tag.ToString().Split(',')[1], freeFilter2.Header.ToString().Split('(')[0].Trim());
                    }
                }
                if (lstFilterDataModal.Count <= 0)
                {
                    lstFilterDataModal.Add(new FilterDataModal { Id = 1, FilterGroupId = 1, Id_Level1 = null, Id_Level2 = null, Id_Level3 = null, Id_Level4 = null });
                    lstFilterDataModal.Add(new FilterDataModal { Id = 2, FilterGroupId = 2, Id_Level1 = null, Id_Level2 = null, Id_Level3 = null, Id_Level4 = null });
                    lstFilterDataModal.Add(new FilterDataModal { Id = 3, FilterGroupId = 3, Id_Level1 = null, Id_Level2 = null, Id_Level3 = null, Id_Level4 = null });
                    lstFilterDataModal.Add(new FilterDataModal { Id = 4, FilterGroupId = 4, Id_Level1 = null, Id_Level2 = null, Id_Level3 = null, Id_Level4 = null });
                }

                await GetAllCompanyMasterData();

                LsvPageGlobVar.TotalRec = ManageCompanyMasterDataCollection.Count;
                PopulateSearchResultCompanyListViewBasedOnSectorAndIndustry(ManageCompanyMasterDataCollection);
                radGridViewGeoAndIndustry.Rebind();

                templateReviewCount.Content = radGridViewGeoAndIndustry.Items.TotalItemCount.ToString();

                if (radGridViewGeoAndIndustry.Items.Count.ToString() == "0")
                {
                    var res = CustomMessageBoxExcel.Show("No records found ", "Logo search", ExcelMessageBoxType.ConfirmationWithOkCancel, ExcelMessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Utils.LogError("Exception inside  SearchCompany : " + ex.Message + ", " + ex.InnerException);
            }
        }

        private void GetDataForSearchGeographyOnly(string type, string id)
        {
            try
            {
                switch (type)
                {
                    case "1":
                        Search = string.IsNullOrEmpty(Search) ? Search + "Filter1_1=" + id + " " : Search + " | Filter1_1=" + id + " ";
                        SelectedContinents = string.IsNullOrEmpty(SelectedContinents) ? SelectedContinents + "Filter1_1=" + id + " " : SelectedContinents + " | Filter1_1=" + id + " ";
                        break;
                    case "2":
                        Search = string.IsNullOrEmpty(Search) ? Search + "Filter1_2=" + id + " " : Search + " |  Filter1_2=" + id + " ";
                        SelectedSubContinents = string.IsNullOrEmpty(SelectedSubContinents) ? SelectedSubContinents + "Filter1_2=" + id + " " : SelectedSubContinents + " |  Filter1_2=" + id + " ";
                        break;
                    case "3":
                        Search = string.IsNullOrEmpty(Search) ? Search + "Filter1_3=" + id + " " : Search + " | Filter1_3=" + id + " ";
                        SelectedRegions = string.IsNullOrEmpty(SelectedRegions) ? SelectedRegions + "Filter1_3=" + id + " " : SelectedRegions + " | Filter1_3=" + id + " ";
                        break;
                    case "4":
                        Search = string.IsNullOrEmpty(Search) ? Search + "Filter1_4=" + id + " " : Search + " | Filter1_4=" + id + " ";
                        SelectedCountries = string.IsNullOrEmpty(SelectedCountries) ? SelectedCountries + "Filter1_4=" + id + " " : SelectedCountries + " | Filter1_4=" + id + " ";
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Utils.LogError("Exception inside  GetDataForSearchGeographyOnly : " + ex.Message + ", " + ex.InnerException);
            }
        }

        private void GetDataForSearchForIndustryOnly(RadTreeViewItem tvn, string type, string id)
        {
            try
            {
                switch (type)
                {
                    case "1":
                        Search = string.IsNullOrEmpty(Search) ? Search + "Filter2_1=" + id + " " : Search + " | Filter2_1=" + id + " ";
                        SelectedSectors = string.IsNullOrEmpty(SelectedSectors) ? SelectedSectors + "Filter2_1=" + id + " " : SelectedSectors + " | Filter2_1=" + id + " ";
                        break;
                    case "2":
                        Search = string.IsNullOrEmpty(Search) ? Search + "Filter2_2=" + id + " " : Search + " |  Filter2_2=" + id + " ";
                        SelectedIndustryGroups = string.IsNullOrEmpty(SelectedIndustryGroups) ? SelectedIndustryGroups + "Filter2_2=" + id + " " : SelectedIndustryGroups + " |  Filter2_2=" + id + " ";
                        break;
                    case "3":
                        Search = string.IsNullOrEmpty(Search) ? Search + "Filter2_3=" + id + " " : Search + " | Filter2_3=" + id + " ";
                        SelectedIndustries = string.IsNullOrEmpty(SelectedIndustries) ? SelectedIndustries + "Filter2_3=" + id + " " : SelectedIndustries + " | Filter2_3=" + id + " ";
                        break;
                    case "4":
                        Search = string.IsNullOrEmpty(Search) ? Search + "Filter2_4=" + id + " " : Search + " | Filter2_4=" + id + " ";
                        SelectedSubIndustries = string.IsNullOrEmpty(SelectedSubIndustries) ? SelectedSubIndustries + "Filter2_4=" + id + " " : SelectedSubIndustries + " | Filter2_4=" + id + " ";
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Utils.LogError("Exception inside  GetDataForSearchForIndustryOnly : " + ex.Message + ", " + ex.InnerException);
            }
        }

        private void GetDataForSearchFreeFilter1Only(string type, string id)
        {
            try
            {
                switch (type)
                {
                    case "1":
                        Search = string.IsNullOrEmpty(Search) ? Search + "Filter3_1=" + id + " " : Search + " | Filter3_1=" + id + " ";
                        SelectedFilter3_1 = string.IsNullOrEmpty(SelectedFilter3_1) ? SelectedFilter3_1 + "Filter3_1=" + id + " " : SelectedFilter3_1 + " | Filter3_1=" + id + " ";
                        break;
                    case "2":
                        Search = string.IsNullOrEmpty(Search) ? Search + "Filter3_2=" + id + " " : Search + " |  Filter3_2=" + id + " ";
                        SelectedFilter3_2 = string.IsNullOrEmpty(SelectedFilter3_2) ? SelectedFilter3_2 + "Filter3_2=" + id + " " : SelectedFilter3_2 + " |  Filter3_2=" + id + " ";
                        break;
                    case "3":
                        Search = string.IsNullOrEmpty(Search) ? Search + "Filter3_3=" + id + " " : Search + " | Filter3_3=" + id + " ";
                        SelectedFilter3_3 = string.IsNullOrEmpty(SelectedFilter3_3) ? SelectedFilter3_3 + "Filter3_3=" + id + " " : SelectedFilter3_3 + " | Filter3_3=" + id + " ";
                        break;
                    case "4":
                        Search = string.IsNullOrEmpty(Search) ? Search + "Filter3_4=" + id + " " : Search + " | Filter3_4=" + id + " ";
                        SelectedFilter3_4 = string.IsNullOrEmpty(SelectedFilter3_4) ? SelectedFilter3_4 + "Filter3_4=" + id + " " : SelectedFilter3_4 + " | Filter3_4=" + id + " ";
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Utils.LogError("Exception inside  GetDataForSearchFreeFilter1Only : " + ex.Message + ", " + ex.InnerException);
            }
        }

        private void GetDataForSearchFreeFilter2Only(string type, string id)
        {
            try
            {
                switch (type)
                {
                    case "1":
                        Search = string.IsNullOrEmpty(Search) ? Search + "Filter4_1=" + id + " " : Search + " | Filter4_1=" + id + " ";
                        SelectedFilter4_1 = string.IsNullOrEmpty(SelectedFilter4_1) ? SelectedFilter4_1 + "Filter4_1=" + id + " " : SelectedFilter4_1 + " | Filter4_1=" + id + " ";
                        break;
                    case "2":
                        Search = string.IsNullOrEmpty(Search) ? Search + "Filter4_2=" + id + " " : Search + " |  Filter4_2=" + id + " ";
                        SelectedFilter4_2 = string.IsNullOrEmpty(SelectedFilter4_2) ? SelectedFilter4_2 + "Filter4_2=" + id + " " : SelectedFilter4_2 + " |  Filter4_2=" + id + " ";
                        break;
                    case "3":
                        Search = string.IsNullOrEmpty(Search) ? Search + "Filter4_3=" + id + " " : Search + " | Filter4_3=" + id + " ";
                        SelectedFilter4_3 = string.IsNullOrEmpty(SelectedFilter4_3) ? SelectedFilter4_3 + "Filter4_3=" + id + " " : SelectedFilter4_3 + " | Filter4_3=" + id + " ";
                        break;
                    case "4":
                        Search = string.IsNullOrEmpty(Search) ? Search + "Filter4_4=" + id + " " : Search + " | Filter4_4=" + id + " ";
                        SelectedFilter4_4 = string.IsNullOrEmpty(SelectedFilter4_4) ? SelectedFilter4_4 + "Filter4_4=" + id + " " : SelectedFilter4_4 + " | Filter4_4=" + id + " ";
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Utils.LogError("Exception inside  GetDataForSearchFreeFilter2Only : " + ex.Message + ", " + ex.InnerException);
            }
        }

        private void logoExpander_Click(object sender, RoutedEventArgs e)
        {
            int columnSpan = 1;
            int setCoulmn = 0;
            Visibility visible = Visibility.Visible;
            int column = Grid.GetColumn(previewColumn);

            if (column == 2)
            {
                setCoulmn = 0;
                visible = Visibility.Collapsed;
                columnSpan = 2;
                if (ProjectType == "PitchreadyExcel")
                    expanderImg.ImageSource = (ImageSource)FindResource("Excel_shrink_logoDrawingImage");
                logoExpander.ToolTip = "Shrink";
            }
            else
            {
                setCoulmn = 2;
                visible = Visibility.Visible;
                columnSpan = 1;
                if (ProjectType == "PitchreadyExcel")
                    expanderImg.ImageSource = (ImageSource)FindResource("Excel_expand_logoDrawingImage");
                logoExpander.ToolTip = "Expand";
            }

            gridColumn.Visibility = visible;
            Grid.SetColumn(previewColumn, setCoulmn);
            Grid.SetColumnSpan(previewColumn, columnSpan);
        }


        private void selectAllState(SelectionChangeEventArgs e)
        {
            //selectCheckBox.Unchecked -= selectCheckBox_Unchecked;

            List<LogoToolCompanyDataFrontEnd> companyCount = new List<LogoToolCompanyDataFrontEnd>();

            foreach (LogoToolCompanyDataFrontEnd row in ManageCompanyMasterDataCollectionCart)
            {
                IEnumerable<LogoToolCompanyDataFrontEnd> rows = radGridViewGeoAndIndustry.Items.Cast<LogoToolCompanyDataFrontEnd>()
                 .Where(row1 => row1.ID.ToString() == row.ID.ToString()).ToList();

                if (rows != null && rows.Count() > 0)
                    companyCount.Add(rows.First());
            }
            var FilteredRows = radGridViewGeoAndIndustry.Items.Cast<LogoToolCompanyDataFrontEnd>().ToList().Except(companyCount).ToList();

            if (radGridViewGeoAndIndustry.Items.Count != 0 && FilteredRows.Count == 0)
            {
                selectCheckBox.IsChecked = true;
            }
            else
            {
                //selectCheckBox.Unchecked += selectCheckBox_Unchecked;
                selectCheckBox.IsChecked = false;
            }
        }
        private void FilterDescriptors_CollectionChanging(object sender, CollectionChangingEventArgs e)
        {
            if (e.Action != CollectionChangeAction.Remove)
                IsCollectionChanged = true;
        }

        private void FilterDescriptors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(1000);
                IsCollectionChanged = false;
                Dispatcher.Invoke(() =>
                {
                    selectItemsBasedOnCart();
                });
            });
        }
        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (IsCollectionChanged) return;
            if (radGridViewGeoAndIndustry.Items.Count > 0)
            {
                lblNodataResult.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblNodataResult.Visibility = Visibility.Visible;
            }

            selectItemsBasedOnCart();
        }



        List<string> notFoundLogos = new List<string>();
        List<LogoToolCompanyDataFrontEnd> selectedGridItems = new List<LogoToolCompanyDataFrontEnd>();

        bool IsSearchTextBox = false;
        private void radGridViewGeoAndIndustry_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            if (IsCollectionChanged)
                return;
            try
            {
                if (e.AddedItems.Count > 0)
                {
                    foreach (LogoToolCompanyDataFrontEnd addedItem in e.AddedItems)
                    {
                        if (CartSource.Any(o => o.LogoData.ID.Equals(addedItem.ID))) continue;
                        int actualAddedSrNo = 0;
                        if (CartSource.Count > 0)
                            actualAddedSrNo = cartSource.Max(o => o.SrNoText);
                        string actualcountID = Guid.NewGuid().ToString();
                        LogoTileVM LooTileModal = new LogoTileVM()
                        {
                            LogoData = addedItem,
                            Tooltip = addedItem.CompanyName,
                            Strindex = actualcountID,
                            SrNoText = actualAddedSrNo + 1
                        };
                        LooTileModal.LogoData.IsActive = true;
                        LooTileModal.DeleteFromCart += DeleteDataFromCart;
                        LooTileModal.CreateCopy += CreateCartDataCopy;
                        CartSource.Add(LooTileModal);
                        LooTileModal.loadData();
                        wrapPanelScroll.ScrollToBottom();
                    }
                }

                List<LogoTileVM> DataTodelete = new List<LogoTileVM>();
                if (e.RemovedItems.Count > 0)
                {
                    foreach (LogoToolCompanyDataFrontEnd removedItem in e.RemovedItems)
                    {
                        DataTodelete.AddRange((CartSource.Where(o => o.LogoData.ID.Equals(removedItem.ID))).ToList());
                    }
                    foreach (var item in DataTodelete)
                    {
                        CartSource.Remove(item);
                    }
                }
                selectCheckBox.Checked -= selectCheckBox_Checked;
                selectCheckBox.Unchecked -= selectCheckBox_Unchecked;
                if (radGridViewGeoAndIndustry.Items.Count != 0 && radGridViewGeoAndIndustry.SelectedItems.Count == radGridViewGeoAndIndustry.Items.Count)
                    selectCheckBox.IsChecked = true;
                else
                    selectCheckBox.IsChecked = false;
                ResetCartStats();
                selectCheckBox.Checked += selectCheckBox_Checked;
                selectCheckBox.Unchecked += selectCheckBox_Unchecked;
            }
            catch (Exception ex)
            {
                Utils.LogError("Error on Removing radGridViewGeoAndIndustry_SelectionChanged: " + ex.Message + ", " + ex.InnerException);
            }
            finally
            {

            }
        }

        private void ResetCartStats()
        {
            int count = 1;
            foreach (var item in CartSource)
            {
                item.LogoData.RowCounter = count;
                count++;
            }
            countLabel.Content = CartSource.Count();
            updateLayout();
        }

        private void CreateCartDataCopy(object sender, Microsoft.Exchange.WebServices.Data.NotificationEventArgs e)
        {
            var data = sender as LogoTileVM;
            string actualcountID = Guid.NewGuid().ToString();
            int actualAddedSrNo = 0;
            if (CartSource.Count > 0)
                actualAddedSrNo = cartSource.Max(o => o.SrNoText);
            LogoTileVM LooTileModal = new LogoTileVM()
            {
                LogoData = Utils.CreateCopy(data.LogoData),
                Tooltip = data.LogoData.CompanyName,
                Strindex = actualcountID,
                SrNoText = actualAddedSrNo + 1
            };
            LooTileModal.LogoData.IsActive = true;
            LooTileModal.DeleteFromCart += DeleteDataFromCart;
            LooTileModal.CreateCopy += CreateCartDataCopy;
            CartSource.Add(LooTileModal);
            LooTileModal.loadData();
            wrapPanelScroll.ScrollToBottom();
            ResetCartStats();
        }

        private async void DeleteDataFromCart(object sender, Microsoft.Exchange.WebServices.Data.NotificationEventArgs e)
        {
            var data = sender as LogoTileVM;
            data.LogoData.IsActive = false;

            var DesktopAlertparams = new DesktopAlertParameters
            {
                Header = "Logo Tool",
                Content = "1 logo deleted click here to restore the logo.",
                ShowDuration = 5000,
                CanMove = false,
                CanAutoClose = true,
                Icon = new Image
                {
                    Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                        System.Drawing.SystemIcons.Information.Handle,
                                        Int32Rect.Empty,
                                        BitmapSizeOptions.FromEmptyOptions())
                },
                IconColumnWidth = 48,
                IconMargin = new Thickness(10, 0, 20, 0),
                ShowMenuButton = false,
                ShowCloseButton = true,
                CommandParameter = data,
                Click = (s, a) =>
                {
                    data.LogoData.IsActive = true;
                },
            };

            DesktopAlertManagerObj.ShowAlert(DesktopAlertparams);

            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            await System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                if (!data.LogoData.IsActive)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (data.LogoData.IsActive || DesktopAlertManagerObj.GetAllAlerts().Count() == 0) break;
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            });
            DesktopAlertManagerObj.CloseAllAlerts();

            if (!data.LogoData.IsActive)
            {
                var dataObj = CartSource.FirstOrDefault(o => o.LogoData.RowCounter.Equals(data.LogoData.RowCounter));
                CartSource.Remove(dataObj);
                if (!CartSource.Any(o => o.LogoData.ID.Equals(data.LogoData.ID)))
                    radGridViewGeoAndIndustry.SelectedItems.Remove(radGridViewGeoAndIndustry.SelectedItems.Cast<LogoToolCompanyDataFrontEnd>().FirstOrDefault(o => o.ID.Equals(data.LogoData.ID)));

                if (selectCheckBox.IsChecked != null && (bool)selectCheckBox.IsChecked)
                {
                    selectCheckBox.Checked -= selectCheckBox_Checked;
                    selectCheckBox.IsChecked = true;
                    selectCheckBox.Checked += selectCheckBox_Checked;
                }
                ResetCartStats();
            }
        }

        private void showLogoBtn_Click(object sender, RoutedEventArgs e)
        {
            radGridViewlogoPreviewGrid.Visibility = Visibility.Collapsed;
            logoImg.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/PitchreadyPowerPoint;component/Media/Icons/LogoTool/logo_images_C.png"));
            logoList.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/PitchreadyPowerPoint;component/Media/Icons/LogoTool/Logo_list_G.png"));
            logoGridPanel.Visibility = Visibility.Visible;
            selectAllState(null);
        }

        private void showLogoList_Click(object sender, RoutedEventArgs e)
        {

            radGridViewlogoPreviewGrid.Visibility = Visibility.Visible;
            logoList.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/PitchreadyPowerPoint;component/Media/Icons/LogoTool/Logo_list_C.png"));
            logoImg.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/PitchreadyPowerPoint;component/Media/Icons/LogoTool/logo_images_G.png"));
            logoGridPanel.Visibility = Visibility.Collapsed;
        }



        private void Luc_Drop(object sender, System.Windows.DragEventArgs e)
        {

            LogoTileUC destinationLUC = sender as LogoTileUC;
            LogoTileVM destinationDataRow = destinationLUC.DataContext as LogoTileVM;

            Object sourceLUCTemp = e.Data.GetData(typeof(LogoTileUC));
            LogoTileUC sourceLUC = (LogoTileUC)sourceLUCTemp;
            LogoTileVM sourceDataRow = (LogoTileVM)sourceLUC.DataContext;

            LogoTileVM toInsert = sourceDataRow;
            int index = CartSource.IndexOf(destinationDataRow);

            CartSource.Remove(sourceDataRow);
            CartSource.Insert(index, toInsert);
            sourceLUC.Effect = null;
            sourceLUC.RenderTransform = null;
            if (this._dragdropWindow != null)
            {
                this._dragdropWindow.Close();
                this._dragdropWindow = null;
            }
            ResetCartStats();
        }
        private void Luc_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (IsDeleteAlertClicked)
                return;

            base.OnMouseMove(e);
            LogoTileUC luc = sender as LogoTileUC;
            var logoWrapPanel = luc.GetVisualParent<WrapPanel>() as WrapPanel;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (logoWrapPanel is WrapPanel)//Anand
                {
                    var draggedItem = logoWrapPanel as WrapPanel;

                    luc.Effect = new DropShadowEffect
                    {
                        Color = new Color { A = 50, R = 0, G = 0, B = 0 },
                        Direction = 320,
                        ShadowDepth = 0,
                        Opacity = .75,
                    };
                    luc.RenderTransform = new RotateTransform(0, 300, 200);

                    CreateDragDropWindow(luc);

                    DragDrop.DoDragDrop(logoWrapPanel, luc, System.Windows.DragDropEffects.Move);
                }
            }
        }

        private void radGridViewlogoPreview_Drop(object sender, System.Windows.DragEventArgs e)
        {
            try
            {

                bool sourceRowChecked = false;
                bool destinationRowChecked = false;

                var destinationRow = e.OriginalSource as GridViewRow ?? (e.OriginalSource as FrameworkElement).ParentOfType<GridViewRow>();
                DataRow destinationDataRow = destinationRow.DataContext as DataRow;
                DataRow sourceDataRow = e.Data.GetData(typeof(DataRow)) as DataRow;

                if (destinationDataRow == sourceDataRow)
                {
                    e.Handled = true;
                    return;
                }
                radCartGridView.SelectionChanged -= radGridViewlogoPreview_SelectionChanged;
                sourceRowChecked = radCartGridView.SelectedItems.Contains(sourceDataRow);
                destinationRowChecked = radCartGridView.SelectedItems.Contains(destinationDataRow);

                radCartGridView.SelectedItems.Remove(sourceDataRow);
                radCartGridView.SelectedItems.Remove(destinationDataRow);

                DataRow toInsert = companyTable.NewRow();

                toInsert.ItemArray = sourceDataRow.ItemArray;
                int index = companyTable.Rows.IndexOf(destinationDataRow);

                companyTable.Rows.Remove(sourceDataRow);
                companyTable.Rows.InsertAt(toInsert, index);

                radCartGridView.Rebind();

                if (sourceRowChecked)
                    radCartGridView.SelectedItems.Add(toInsert);
                if (destinationRowChecked)
                    radCartGridView.SelectedItems.Add(destinationDataRow);

                //showLogos();

                //remapping();
                radCartGridView.SelectionChanged += radGridViewlogoPreview_SelectionChanged;
                //reselectSelectedItemsOfCart();


            }
            catch (Exception ex)
            {
                HideDropPositionFeedbackPresenter();
            }

        }

        //private void remapping()
        //{
        //    try
        //    {
        //        listTileViewMapping.Clear();

        //        foreach (LogoToolCompanyDataFrontEnd item in ManageCompanyMasterDataCollectionCart)
        //        {
        //            if (selectedItemsInCart.Count != 0 && selectedItemsInCart.ContainsKey(item.ID.ToString()))
        //            {
        //                selectedItemsInCart[item.ID.ToString()] = item;
        //            }
        //            List<LogoTileUC> UC_List = logoWrapPanel.Children.Cast<LogoTileUC>().ToList();

        //            try
        //            {
        //                LogoTileUC UC = UC_List.FirstOrDefault(u => (u.Tag as LogoToolCompanyDataFrontEnd).ID.ToString() == item.ID.ToString());
        //                if (UC != null)
        //                    listTileViewMapping.Add(item, UC);
        //            }
        //            catch (Exception ex)
        //            {
        //                continue;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
        bool IsAlertClicked = false;
        bool IsDeleteAlertClicked = false;
        private void radGridViewlogoPreview_CellLoaded(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Header.ToString() == "Actions")
            {
                if (e.Cell is GridViewCell)
                {
                    e.Cell.Cursor = System.Windows.Input.Cursors.Hand;
                }
            }
            if (e.Cell.Column.Header.ToString() == "Sr. No.")
            {
                if (e.Cell is GridViewCell)
                {
                    e.Cell.Content = Convert.ToInt32(radCartGridView.Items.IndexOf(e.Cell.ParentRow.Item)) + 1;
                }
            }
        }

        private void exportBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectType == "PitchreadyExcel")
            {
                Dictionary<object, object> s = new Dictionary<object, object>();
                s.Add("LogoToolFrontEndExcel", "");
                s.Add(cmbImportType.SelectedItem as string, "");
                s.Add("Resize", txtLogoSize.Text);

                List<string> LocalPathLogos = new List<string>();
                foreach (LogoTileVM luc in CartTilePanel.Items)
                {
                    if (cmbImportType.SelectedItem.ToString() != "Folder")
                        LocalPathLogos.Add(luc.LogoData.LocalLogoPath);
                    else
                        LocalPathLogos.Add(luc.LogoData.LocalLogoPath + "," + luc.LogoData.CompanyName + "_" + luc.LogoData.CompanyTicker);
                }

                s.Add("LocalPathLogos", LocalPathLogos);
                Utils.InitiateWorkOnExcelEvent.GetEvent<CrossModuleCommunication>().Publish(s);
                this.WindowState = WindowState.Minimized;
            }

            else if (ProjectType == "PitchreadyPowerPoint")
            {

            }
        }

        private void LogoPreview_Closed(object sender, EventArgs e)
        {
            //logoPreview = null;
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            //    Utils.TryDeleteFolder(Utils.GetToolTempPath(GlobalUtilityLibrary.Enumrations.Tooltype.LogoTool));
            //});
        }

        private void numSort_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.radCartGridView.SortDescriptors.Clear();
                Utils.GlobalEventForExcel.GetEvent<CrossModuleCommunication>().Publish(null);
                System.Windows.Forms.Cursor.Current = Utils.CustomCursor;
                CartSource = new ObservableCollection<LogoTileVM>(CartSource.Reverse());
                CartTilePanel.ItemsSource = CartSource;
                radCartGridView.ItemsSource = CartSource;
                //radGridViewlogoPreview.Rebind();
                //showLogos();
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                int count = 1;
                foreach (var item in CartSource)
                {
                    item.LogoData.RowCounter = count;
                    count++;
                }
            }
            catch (Exception ex)
            {
                Utils.LogError("Error on Removing numSort_Click: " + ex.Message + ", " + ex.InnerException);
            }
        }

        public ObservableCollection<LogoToolCompanyDataFrontEnd> ReverseRowsInDataTable(ObservableCollection<LogoToolCompanyDataFrontEnd> inputTable)
        {
            ObservableCollection<LogoToolCompanyDataFrontEnd> outputTable = new ObservableCollection<LogoToolCompanyDataFrontEnd>();

            for (int i = inputTable.Count - 1; i >= 0; i--)
            {
                outputTable.Add(inputTable[i]);

                listTileViewMapping.Remove(inputTable[i]);
                listTileViewMapping.Add(outputTable[outputTable.Count - 1], null);

                if (selectedItemsInCart.ContainsKey(inputTable[i].ID.ToString()))
                {
                    selectedItemsInCart.Remove(inputTable[i].ID.ToString());

                    selectedItemsInCart.Add(outputTable[outputTable.Count - 1].ID.ToString(), null);
                }

                //remapping();
                //reselectSelectedItemsOfCart();
            }

            return outputTable;
        }

        SortDescriptor descriptor = new SortDescriptor();
        List<LogoTileVM> OriginalOrder = null;
        private void alphaSort_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Utils.GlobalEventForExcel.GetEvent<CrossModuleCommunication>().Publish(null);
                System.Windows.Forms.Cursor.Current = Utils.CustomCursor;

                if (sortingDir == "ORI")
                {
                    alphaSort.ToolTip = "A-Z";
                    CartSource = new ObservableCollection<LogoTileVM>(CartSource.OrderBy(o => o.SrNoText));
                    sortingDir = "ASC";
                }

                else if (sortingDir == "ASC")
                {
                    if (OriginalOrder == null)
                        OriginalOrder = CartSource.ToList();
                    alphaSort.ToolTip = "Z-A";
                    CartSource = new ObservableCollection<LogoTileVM>(CartSource.OrderBy(o => o.LogoData.CompanyName));
                    sortingDir = "desc";
                }
                else if (sortingDir == "desc")
                {
                    if (OriginalOrder == null)
                        OriginalOrder = CartSource.ToList();
                    alphaSort.ToolTip = "Original order";
                    CartSource = new ObservableCollection<LogoTileVM>(CartSource.OrderByDescending(o => o.LogoData.CompanyName));
                    sortingDir = "ORI";
                }
                else
                    sortingDir = "ASC";

                int count = 1;
                foreach (var item in CartSource)
                {
                    item.LogoData.RowCounter = count;
                    count++;
                }

                CartTilePanel.ItemsSource = CartSource;
                radCartGridView.ItemsSource = CartSource;
            }
            catch (Exception ex)
            {
                Utils.LogError("Error on Removing alphaSort_Click: " + ex.Message + ", " + ex.InnerException);
            }
            finally
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
        }

        private void clearAllBtn_Click(object sender, RoutedEventArgs e)
        {
            var res = CustomMessageBoxExcel.Show("Are you sure you want to clear all? ", "Confirmation", ExcelMessageBoxType.ConfirmationWithOkCancel, ExcelMessageBoxImage.Information);

            if (!res)
            {
                return;
            }


            this.radGridViewGeoAndIndustry.FilterDescriptors.SuspendNotifications();
            foreach (Telerik.Windows.Controls.GridViewColumn column in this.radGridViewGeoAndIndustry.Columns)
            {
                column.ClearFilters();
            }
            this.radGridViewGeoAndIndustry.FilterDescriptors.ResumeNotifications();
            selectedGridItems.Clear();
            radGridViewGeoAndIndustry.SelectedItems.Clear();
            CartSource.Clear();

            radGeographyTreeView.UncheckTreeView();
            radIndustryTreeView.UncheckTreeView();
            radFreeFilter1TreeView.UncheckTreeView();
            radFreeFilter2TreeView.UncheckTreeView();
            radGridViewGeoAndIndustry.ItemsSource = null;
            radGridViewGeoAndIndustry.SelectedItems.Clear();
            radGridViewGeoAndIndustry.Rebind();
            searchCompany.SearchText = string.Empty;
            IsClearBtnClick = false;
            templateReviewCount.Content = 0;
            listTileViewMapping.Clear();
            selectedItemsInCart.Clear();

            cmbImportType.SelectedIndex = 1;
            txtLogoSize.Text = "100";
            updateLayout();

            if (originalTable != null)
                originalTable.Clear();
            originalTable = null;
            shrinkGrid();
            searchTree.SearchText = string.Empty;
            searchradIndustryTree.SearchText = string.Empty;
            selectCheckBox.IsChecked = false;
            radCartGridView.Items.Refresh();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var res = CustomMessageBoxExcel.Show("Are you sure you want to delete all logos? ", "Confirmation", ExcelMessageBoxType.ConfirmationWithOkCancel, ExcelMessageBoxImage.Information);

                if (!res)
                {
                    return;
                }
                DesktopAlertManagerObj.CloseAllAlerts(false);
                radGridViewGeoAndIndustry.SelectedItems.Clear();
                CartSource.Clear();
            }
            catch (Exception ex)
            {
                Utils.LogError("Error on Removing btnDelete_Click: " + ex.Message + ", " + ex.InnerException);
            }
            finally
            {
                ResetCartStats();
            }
        }

        private void updateLayout()
        {
            //radGridViewlogoPreview.Rebind();
            if (CartSource.Count == 0)
            {
                exportBtn.IsEnabled = false;
                btnDelete.IsEnabled = false;
                btnExportCart.IsEnabled = false;
                numSort.IsEnabled = false;
                alphaSort.IsEnabled = false;
                showLogoBtn.IsEnabled = false;
                showLogoList.IsEnabled = false;
            }
            else
            {
                exportBtn.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnExportCart.IsEnabled = true;
                numSort.IsEnabled = true;
                alphaSort.IsEnabled = true;
                logoExpander.IsEnabled = true;
                showLogoBtn.IsEnabled = true;
                showLogoList.IsEnabled = true;
            }

            countLabel.Content = CartSource.Count;
        }

        private void cmbImportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProjectType == "PitchreadyPowerPoint")
            {
                if (cmbImportType.SelectedItem.ToString() != "New Slide/Template")
                    exportBtn.Content = "Export";
                else
                    exportBtn.Content = "Next";

                if (cmbImportType.SelectedItem.ToString() == "Non-Printable Area")
                {
                    logoResizePanel.IsEnabled = true;
                    txtLogoSize.Text = "100";
                }
                else
                    logoResizePanel.IsEnabled = false;
            }
            else if (ProjectType == "PitchreadyExcel")
            {
                if (cmbImportType.SelectedItem.ToString() != "All in one cell" ||
                    cmbImportType.SelectedItem.ToString() != "Folder")
                {
                    logoResizePanel.IsEnabled = true;
                    txtLogoSize.Text = "100";
                }
                else
                {
                    logoResizePanel.IsEnabled = false;
                }
                exportBtn.Content = "Export";
            }
        }

        private void searchCompany_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView dataRow = searchCompany.SelectedItem as DataRowView;

            if (dataRow == null)
                return;

            System.Data.DataTable dt = companyDataTable.Select("CompanyName = '" + dataRow["CompanyName"] + "'").CopyToDataTable();
            System.Data.DataTable DataList = (System.Data.DataTable)radGridViewGeoAndIndustry.ItemsSource;

            if (DataList != null)
            {
                DataList.Merge(dt);

                radGridViewGeoAndIndustry.ItemsSource = DataList;
            }
            else
                radGridViewGeoAndIndustry.ItemsSource = dt;

            radGridViewGeoAndIndustry.Rebind();

            updateLayout();
        }

        System.Data.DataTable searchedRecords = new System.Data.DataTable();
        private async void companyReaderThreadSearchTextBox_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                selectCheckBox.Checked -= selectCheckBox_Checked;
                selectCheckBox.Unchecked -= selectCheckBox_Unchecked;
                radGridViewGeoAndIndustry.SelectionChanged -= radGridViewGeoAndIndustry_SelectionChanged;
                selectCheckBox.Checked -= selectCheckBox_Checked;
                selectCheckBox.Unchecked -= selectCheckBox_Unchecked;
                radGridViewGeoAndIndustry.SelectionChanged -= radGridViewGeoAndIndustry_SelectionChanged;
                if (companyTab.IsSelected)
                {
                    IsSearchTextBox = true;
                    LsvPageGlobVar.RecStart = 0;
                    string company = searchCompany.SearchText;
                    if (company.Contains("*"))
                        company = company.Replace("*", "[*]");
                    if (company.Contains("%"))
                        company = company.Replace("%", "[%]");
                    if (company.Contains("'"))
                        company = company.Replace("'", "''");
                    LsvPageGlobVar.RecStart = 0;
                    if (string.IsNullOrEmpty(searchCompany.SearchText))
                    {
                        await GetAllCompanyMasterData(string.Empty);
                        PopulateSearchResultCompanyListViewBasedOnSectorAndIndustry(ManageCompanyMasterDataCollection);
                    }
                    else
                    {
                        await GetAllCompanyMasterData(company);

                        if (ManageCompanyMasterDataCollection.Count <= 0)
                        {
                            var res = CustomMessageBoxExcel.Show("No records found ", "Logo Search", ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                            radGridViewGeoAndIndustry.ItemsSource = new ObservableCollection<LogoToolCompanyDataFrontEnd>(ManageCompanyMasterDataCollection);
                            busyIndicator.IsBusy = false;
                            templateReviewCount.Content = radGridViewGeoAndIndustry.Items.TotalItemCount.ToString();
                            searchCompany.Focusable = true;
                            searchCompany.Focus();
                            return;
                        }
                        PopulateSearchResultCompanyListViewBasedOnSectorAndIndustry(ManageCompanyMasterDataCollection);
                    }
                    LsvPageGlobVar.TotalRec = ManageCompanyMasterDataCollection.Count;
                    Dispatcher.Invoke(() =>
                    {
                        exportBtn.IsEnabled = ManageCompanyMasterDataCollection.Count > 0;
                    });
                    updateLayout();
                    selectItemsBasedOnCart();
                    selectAllState(null);
                    templateReviewCount.Content = radGridViewGeoAndIndustry.Items.TotalItemCount.ToString();

                    IsSearchTextBox = false;
                    busyIndicator.IsBusy = false;
                }
                else
                {
                    IsSearchTextBox = true;
                    if (brandDataTable.Rows.Count > 0)
                    {
                        LsvPageGlobVar.TotalRec = brandDataTable.Rows.Count;
                        LsvPageGlobVar.RecStart = 0;
                        selectedForCompany = brandDataTable;
                        string company = searchCompanyBrand.SearchText;
                        if (company.Contains("*"))
                            company = company.Replace("*", "[*]");
                        if (company.Contains("%"))
                            company = company.Replace("%", "[%]");
                        if (company.Contains("'"))
                            company = company.Replace("'", "''");
                        LsvPageGlobVar.RecStart = 0;
                        if (string.IsNullOrEmpty(searchCompanyBrand.SearchText))
                        {
                            searchedRecords = brandDataTable;
                        }
                        else
                        {
                            DataView dv = new DataView(selectedForCompany);
                            dv.RowFilter = string.Format("BrandName LIKE '%{0}%' OR CompanyTicker LIKE '%{0}%'", company);
                            //dv.RowFilter = 
                            searchedRecords = dv.ToTable();
                            if (dv.ToTable() != null)
                            {
                                if (dv.ToTable().Rows.Count <= 0)
                                {
                                    var res = CustomMessageBoxExcel.Show("No records found ", "Logo Search", ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                                    radBrandGridView.ItemsSource = searchedRecords;
                                    busyIndicator.IsBusy = false;
                                    templateReviewCount.Content = radBrandGridView.Items.TotalItemCount.ToString();
                                    searchCompanyBrand.Focusable = true;
                                    searchCompanyBrand.Focus();
                                    return;
                                }
                            }
                            RelevencySearch(searchCompanyBrand.SearchText);
                            PopulateSearchResultCompanyListViewBasedOnSectorAndIndustry(ManageCompanyMasterDataCollection);
                        }

                        RelevencySearch(searchCompanyBrand.SearchText);
                        radBrandGridView.ItemsSource = searchedRecords;
                        updateLayout();
                    }
                    selectItemsBasedOnCart();
                    selectAllState(null);
                    templateReviewCount.Content = radGridViewGeoAndIndustry.Items.TotalItemCount.ToString();

                    IsSearchTextBox = false;
                    busyIndicator.IsBusy = false;

                }
                selectCheckBox.Checked += selectCheckBox_Checked;
                selectCheckBox.Unchecked += selectCheckBox_Unchecked;
                radGridViewGeoAndIndustry.SelectionChanged += radGridViewGeoAndIndustry_SelectionChanged;
            }
            catch (Exception ex)
            {
                selectCheckBox.Checked += selectCheckBox_Checked;
                selectCheckBox.Unchecked += selectCheckBox_Unchecked;
                radGridViewGeoAndIndustry.SelectionChanged += radGridViewGeoAndIndustry_SelectionChanged;
            }

        }


        void RelevencySearch(string searchText)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                EnumerableRowCollection<DataRow> query = from contact in searchedRecords.AsEnumerable()
                                                         where (contact.Field<string>("CompanyName").ToLower().StartsWith(searchText.ToLower())
                                                         || (contact.Field<string>("CompanyTicker") != null && contact.Field<string>("CompanyTicker").ToLower().StartsWith(searchText.ToLower())))
                                                         orderby contact.Field<string>("CompanyName") ascending
                                                         select contact;
                EnumerableRowCollection<DataRow> query2 = from contact in searchedRecords.AsEnumerable()
                                                          where (contact.Field<string>("CompanyName").ToLower().IndexOf(searchText.ToLower()) > 0
                                                          || (contact.Field<string>("CompanyTicker") != null && contact.Field<string>("CompanyTicker").ToLower().IndexOf(searchText.ToLower()) > 0))
                                                          orderby contact.Field<string>("CompanyName") ascending
                                                          select contact;

                if (query.Count() > 0 && query2.Count() > 0)
                {
                    System.Data.DataTable dt3 = query2.CopyToDataTable();
                    System.Data.DataTable dt = query.CopyToDataTable();
                    if (dt != null && dt3 != null)
                        dt.Merge(dt3);
                    searchedRecords = dt;
                }
                else if (query2.Count() > 0)
                {
                    System.Data.DataTable dt = query2.CopyToDataTable();
                    searchedRecords = dt;
                }
                else if (query.Count() > 0)
                {
                    System.Data.DataTable dt = query.CopyToDataTable();
                    searchedRecords = dt;
                }
            }
        }


        private void companyReaderThreadSearchTextBox_DoWork(object sender, DoWorkEventArgs e)
        {
            bool isBrand = false;
            bool isCompany = false;
            Dispatcher.Invoke(() =>
            {
                if (companyTab.IsSelected)
                    isCompany = true;
                else
                    isBrand = true;
            });
            if (isCompany)
            {
            }
            else if (isBrand && brandDataTable == null)
            {
                //brandDataTable = companyDataLayer.GetBrandDetails();
                exportBtn.IsEnabled = brandDataTable.Rows.Count > 0;
            }
        }
        private void searchFromDatabase()
        {
            try
            {

                this.radGridViewGeoAndIndustry.FilterDescriptors.SuspendNotifications();
                foreach (Telerik.Windows.Controls.GridViewColumn column in this.radGridViewGeoAndIndustry.Columns)
                {
                    column.ClearFilters();
                }
                this.radGridViewGeoAndIndustry.FilterDescriptors.ResumeNotifications();
                if (companyReaderThreadSearchTextBox == null)
                {
                    companyReaderThreadSearchTextBox = new BackgroundWorker();
                    companyReaderThreadSearchTextBox.DoWork += new DoWorkEventHandler(companyReaderThreadSearchTextBox_DoWork);
                    companyReaderThreadSearchTextBox.RunWorkerCompleted += new
                        RunWorkerCompletedEventHandler(companyReaderThreadSearchTextBox_RunWorkerCompleted);
                }
                busyIndicator.IsBusy = true;
                companyReaderThreadSearchTextBox.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                selectCheckBox.Checked += selectCheckBox_Checked;
                selectCheckBox.Unchecked += selectCheckBox_Unchecked;
                radGridViewGeoAndIndustry.SelectionChanged += radGridViewGeoAndIndustry_SelectionChanged;
                Utils.LogError("Error occured inside SearchCompanyForCompanyByTxt. Message: " + ex.Message + ". StackTrace: " + ex.StackTrace + ". Datetime: " + DateTime.Now);
            }
        }

        private void searchCompany_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty(searchCompany.SearchText) || searchCompany.SearchText.Length <= 1)
                {
                    SerchStats.Visibility = Visibility.Visible;
                    return;
                }
                else
                {
                    SerchStats.Visibility = Visibility.Collapsed; ;
                }
                shrinkGrid();
                searchFromDatabase();
            }

        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {

            // updateComboBox();
        }

        private void Window_GotMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //updateComboBox();
        }

        //TODO: self commented
        //private void updateComboBox()
        //{
        //    Microsoft.Office.Interop.PowerPoint.Slide slide = (Slide)Globals.ThisAddIn.Application.ActiveWindow.View.Slide;

        //    Selection selectionMultiple = Globals.ThisAddIn.Application.ActiveWindow.Selection;
        //    Microsoft.Office.Interop.PowerPoint.ShapeRange shapeRangeMultiple = null;
        //    try
        //    {
        //        shapeRangeMultiple = selectionMultiple.ShapeRange;
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
        //        return;
        //    }

        //    if (shapeRangeMultiple.HasTable == MsoTriState.msoTrue)
        //    {
        //        if (ShapeInsertionHelper.GetAllSelectedCells(shapeRangeMultiple[1]).Count > 1)
        //        {
        //            cmbImportType.SelectedIndex = 3;
        //        }
        //        else
        //        {
        //            cmbImportType.SelectedIndex = 2;
        //        }
        //    }

        //    if (shapeRangeMultiple.HasTable != MsoTriState.msoTrue)
        //    {
        //        if (shapeRangeMultiple.Count > 1)
        //        {
        //            cmbImportType.SelectedIndex = 3;
        //        }
        //        else
        //        {
        //            cmbImportType.SelectedIndex = 2;
        //        }
        //    }
        //}

        private void Window_Activated(object sender, EventArgs e)
        {
            Utils.SetResourcesForTool(Tooltype.LogoTool);
            showPopupMsg = true;
            try
            {
                //updateComboBox();
            }
            catch (Exception ex)
            {

            }

        }

        private void radGridViewlogoPreview_DragOver(object sender, System.Windows.DragEventArgs e)
        {
            var destinationRow = e.OriginalSource as GridViewRow ?? (e.OriginalSource as FrameworkElement).ParentOfType<GridViewRow>();
            this.ShowDropPositionFeedbackPresenter(radCartGridView, destinationRow);
        }

        public DependencyObject FindChild(DependencyObject o, Type childType)
        {
            DependencyObject foundChild = null;
            if (o != null)
            {
                int childrenCount = VisualTreeHelper.GetChildrenCount(o);
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(o, i);
                    if (child.GetType() != childType)
                    {
                        foundChild = FindChild(child, childType);
                    }
                    else
                    {
                        foundChild = child;
                        break;
                    }
                }
            }
            return foundChild;
        }

        private void RadTabControl_SelectionChanged(object sender, RadSelectionChangedEventArgs e)
        {
            DesktopAlertManagerObj.CloseAllAlerts();
            if (frmInitialize)
            {
                frmInitialize = false;
                return;
            }

            RadTabControl tab = sender as RadTabControl;
            if (tab.SelectedIndex == 0)
            {
                radGridViewGeoAndIndustry.Visibility = Visibility.Visible;
                radBrandGridView.Visibility = Visibility.Collapsed;
            }
            else
            {
                radGridViewGeoAndIndustry.Visibility = Visibility.Collapsed;
                radBrandGridView.Visibility = Visibility.Visible;

            }
        }

        private void radGridViewGeoAndIndustry_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {

                Telerik.Windows.Controls.GridView.GridViewRow row = (Telerik.Windows.Controls.GridView.GridViewRow)radGridViewGeoAndIndustry.CurrentCell.ParentRow;
                row.IsSelected = !row.IsSelected;
            }
            catch (Exception ex)
            {
                Utils.LogError("error on checking the row: " + ex.Message + ", " + ex.InnerException);
            }
        }

        bool IsClearBtnClick = false;
        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            IsClearBtnClick = true;
            //btnExport.IsEnabled = false;//
            var res = CustomMessageBoxExcel.Show("Are you sure you want to clear all applied filters? ", "Confirmation", ExcelMessageBoxType.ConfirmationWithOkCancel, ExcelMessageBoxImage.Information);
            if (!res)
            {
                return;
            }
            this.radGridViewGeoAndIndustry.FilterDescriptors.SuspendNotifications();
            foreach (Telerik.Windows.Controls.GridViewColumn column in this.radGridViewGeoAndIndustry.Columns)
            {
                column.ClearFilters();
            }
            this.radGridViewGeoAndIndustry.FilterDescriptors.ResumeNotifications();
            radGeographyTreeView.UncheckTreeView();
            radIndustryTreeView.UncheckTreeView();
            radFreeFilter1TreeView.UncheckTreeView();
            radFreeFilter2TreeView.UncheckTreeView();

            radGridViewGeoAndIndustry.ItemsSource = null;
            radGridViewGeoAndIndustry.SelectedItems.Clear();
            radGridViewGeoAndIndustry.Rebind();
            searchCompany.SearchText = string.Empty;
            updateLayout();
            selectCheckBox.IsChecked = false;
            searchTree.SearchText = string.Empty;
            searchradIndustryTree.SearchText = string.Empty;
            IsClearBtnClick = false;
            templateReviewCount.Content = 0;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string message = string.Empty;
            bool logoInCart = false;
            if (showPopupMsg == true && companyTable.Rows.Count > 0 || showPopupMsg == true && CartSource.Count > 0)
            {
                message = "logo(s) in the cart";
                logoInCart = true;
            }
            if (showPopupMsg == true && radGridViewGeoAndIndustry.Items.Count > 0 && ManageCompanyMasterDataCollection.Count != radGridViewGeoAndIndustry.Items.Count)
            {
                if (logoInCart)
                    message += " & ";

                message += "some data in filter";
            }
            if (!string.IsNullOrEmpty(message))
            {
                var res = new CustomMessageBoxExcel(string.Format("You have {0}. Do you still want to exit?", message), "Logo Tool", ExcelMessageBoxType.ConfirmationWithOkCancel, ExcelMessageBoxImage.Information);
                res.ShowDialog();

                if (res.messageBoxResult == ExcelCustomMessageResult.Cancel || res.messageBoxResult == ExcelCustomMessageResult.None)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        RadExpander expandedExpander = null;
        private void filter1Expander_Expanded(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (expandedExpander != null)
                expandedExpander.IsExpanded = false;

            expandedExpander = sender as RadExpander;
        }

        private void filter1Expander_Collapsed(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (expandedExpander == (sender as RadExpander))
                expandedExpander = null;
        }



        private void radGridViewlogoPreview_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            try
            {
                foreach (LogoToolCompanyDataFrontEnd row in e.AddedItems)
                {
                    if (!selectedItemsInCart.ContainsKey(row.ID.ToString()))
                    {
                        selectedItemsInCart.Add(row.ID.ToString(), row);
                        if (listTileViewMapping.ContainsKey(row) && listTileViewMapping[row] != null)
                        {
                            listTileViewMapping[row].logoCheckBox.IsChecked = true;
                        }
                    }
                }

                foreach (LogoToolCompanyDataFrontEnd row in e.RemovedItems)
                {

                    if (selectedGridItems.Contains(row))
                        selectedGridItems.Remove(row);

                    if (selectedItemsInCart.ContainsKey(row.ID.ToString()))
                    {
                        selectedItemsInCart.Remove(row.ID.ToString());

                        if (listTileViewMapping.ContainsKey(row) && listTileViewMapping[row] != null)
                        {
                            listTileViewMapping[row].logoCheckBox.IsChecked = false;
                        }
                    }
                }
            }
            catch
            { }
        }

        bool unCheckCheckBox = false;
        private void radGridViewGeoAndIndustry_SelectionChanging(object sender, SelectionChangingEventArgs e)
        {

            try
            {
                if ((e.AddedItems.Count + companyTable.Rows.Count) > 300)
                {
                    radGridViewGeoAndIndustry.SelectionChanged -= radGridViewGeoAndIndustry_SelectionChanged;
                    var res = CustomMessageBoxExcel.Show("You can add only upto 300 logos to Cart ", "Logo search", ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                    unCheckCheckBox = true;
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void txtLogoSize_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex _regex = new Regex("[^0-9]+");
            e.Handled = _regex.IsMatch(e.Text);
        }

        private void ApplyBtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(searchCompany.SearchText) || searchCompany.SearchText.Length <= 1)
            {
                SerchStats.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                SerchStats.Visibility = Visibility.Collapsed; ;
            }
            btnExport.IsEnabled = true;
            shrinkGrid();
            Utils.GlobalEventForExcel.GetEvent<CrossModuleCommunication>().Publish(null);
            System.Windows.Forms.Cursor.Current = Utils.CustomCursor;
            searchFromDatabase();
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
        }

        private void searchCompany_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
        }
        private void radGeographyTreeView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) { RoutedEvent = MouseWheelEvent, Source = sender };
                var parent = ((System.Windows.Controls.Control)sender).Parent as UIElement;
                if (parent != null)
                {
                    parent.RaiseEvent(eventArg);
                }
            }
        }

        private void selectCol_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void searchTree_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string searchText = string.Empty;
            if (e.Key == Key.Enter)
            {
                searchGeographyTreeView();
            }
        }

        bool isGeoitemSelected = false;
        private void searchGeographyTreeView()
        {
            radGeographyTreeView.ExpandAll();
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(200);
                Dispatcher.Invoke(() =>
                {
                    if (!string.IsNullOrEmpty(searchTree.SearchText))
                    {
                        Collection<RadTreeViewItem> allTreeContainers = GetAllNodes(radGeographyTreeView);
                        foreach (RadTreeViewItem item in allTreeContainers)
                        {
                            if (item.Header.ToString().ToLower().ToString().Contains(searchTree.SearchText.ToLower()))
                            {
                                item.Focus();
                                isGeoitemSelected = true;
                                item.IsSelected = true;
                                return;
                            }

                        }
                        isGeoitemSelected = false;
                        CustomMessageBoxExcel.Show("No records found ", "Confirmation", ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                    }
                });
            });
        }
        private Collection<RadTreeViewItem> GetAllNodes(System.Windows.Controls.ItemsControl itemsControl)
        {
            Collection<RadTreeViewItem> allItems = new Collection<RadTreeViewItem>();
            for (int i = 0; i < itemsControl.Items.Count; i++)
            {
                // try to get the item Container   
                //RadTreeViewItem childItemContainer = itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as RadTreeViewItem;
                RadTreeViewItem childItemContainer = itemsControl.Items[i] as RadTreeViewItem;
                // the item container maybe null if it is still not generated from the runtime   
                if (childItemContainer != null)
                {
                    //if (childItemContainer.CheckState == System.Windows.Automation.ToggleState.On)
                    allItems.Add(childItemContainer);
                    Collection<RadTreeViewItem> childItems = GetAllNodes(childItemContainer);
                    foreach (RadTreeViewItem childItem in childItems)
                    {
                        //if (childItem.CheckState == System.Windows.Automation.ToggleState.On)
                        allItems.Add(childItem);
                    }
                }
            }
            return allItems;
        }

        private void radGeographyTreeView_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            RadTreeView treeView = sender as RadTreeView;
            if (e.Key == Key.Enter)
                searchTreeview(treeView, searchTree.SearchText.ToLower(), false);

        }

        private void searchTreeview(RadTreeView treeView, string searchText, bool isRestart)
        {
            if (!string.IsNullOrEmpty(searchText) && treeView.SelectedItem != null)
            {
                Collection<RadTreeViewItem> allTreeContainers = GetAllNodes(treeView);
                int index = 0;

                if (isRestart)
                    index = -1;
                else if (allTreeContainers.Contains(treeView.SelectedItem))
                {
                    index = allTreeContainers.IndexOf(treeView.SelectedItem as RadTreeViewItem);
                }

                for (int i = index + 1; i < allTreeContainers.Count; i++)
                {
                    RadTreeViewItem item = allTreeContainers[i];
                    if (item.Header.ToString().ToLower().ToString().Contains(searchText))
                    {
                        item.Focus();
                        item.IsSelected = true;

                        return;
                    }
                }
                if (index == -1)
                {
                    CustomMessageBoxExcel.Show("No records found ", "Confirmation", ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                    return;
                }
                searchTreeview(treeView, searchText.ToLower(), true);
            }
        }

        private void searchradIndustryTree_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

            string searchText = string.Empty;
            if (e.Key == Key.Enter)
            {
                searchIndustryTreeView();
            }
        }

        bool isIndustryitemSelected = false;
        bool isFilter1itemSelected = false;
        bool isFilter2itemSelected = false;
        private void searchIndustryTreeView()
        {
            radIndustryTreeView.ExpandAll();
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(200);
                Dispatcher.Invoke(() =>
                {
                    if (!string.IsNullOrEmpty(searchradIndustryTree.SearchText))
                    {
                        Collection<RadTreeViewItem> allTreeContainers = GetAllNodes(radIndustryTreeView);
                        foreach (RadTreeViewItem item in allTreeContainers)
                        {
                            if (item.Header.ToString().ToLower().ToString().Contains(searchradIndustryTree.SearchText.ToLower()))
                            {
                                item.IsSelected = true;
                                item.Focus();
                                isIndustryitemSelected = true;
                                return;
                            }

                        }
                        isIndustryitemSelected = false;
                        CustomMessageBoxExcel.Show("No records found ", "Confirmation", ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                    }
                });
            });
        }

        private void radIndustryTreeView_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            RadTreeView treeView = sender as RadTreeView;
            if (e.Key == Key.Enter)
                searchTreeview(treeView, searchradIndustryTree.SearchText.ToLower(), false);
        }

        bool checkBoxClicked = false;
        private async void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            notFoundLogos.Clear();
            if (selectCheckBox.IsChecked == true)
            {
                if (radGridViewGeoAndIndustry.Items.Count > 300)
                {
                    var res = CustomMessageBoxExcel.Show("You can add only upto 300 logos to Cart ", "Logo Search", ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                    selectCheckBox.IsChecked = false;
                }
                else
                {
                    try
                    {
                        if (radGridViewGeoAndIndustry.Items.Count > 0)
                        {
                            List<string> lstLogoNames = new List<string>();
                            masterDataClient.Clear();
                            foreach (LogoToolCompanyDataFrontEnd item in radGridViewGeoAndIndustry.Items)
                            {
                                lstLogoNames.AddRange(ManageCompanyMasterDataCollection.Where(x => x.ID == item.ID).Select(x => x.SystemLogoName).ToList());
                                masterDataClient.AddRange(ManageCompanyMasterDataCollection.Where(x => x.ID == item.ID).ToList());
                            }
                            RadProgressBar1.Value = 0;
                            txtLoadingLabel.Text = "Preparing to download " + lstLogoNames.Count + " logo(s)...";
                            BrdrProgressIndicator.Visibility = Visibility.Visible;
                            await System.Threading.Tasks.Task.Factory.StartNew(() =>
                            {
                                System.Threading.Thread.Sleep(200);
                            });
                            await DownloadAllLogosArtifacts(lstLogoNames);
                        }
                    }
                    catch
                    { }

                    Utils.GlobalEventForExcel.GetEvent<CrossModuleCommunication>().Publish(null);
                    System.Windows.Forms.Cursor.Current = Utils.CustomCursor;
                    foreach (var item in radGridViewGeoAndIndustry.Items)
                    {
                        if (ManageCompanyMasterDataCollectionCart.Count >= 300)
                        {
                            //selectCheckBox.Click -= CheckBox_Click;
                            CustomMessageBoxExcel.Show("You can add only upto 300 logos to Cart ", "Logo search", ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                            selectCheckBox.IsChecked = false;
                            //selectCheckBox.Click += CheckBox_Click;
                            return;
                        }
                        if (!radGridViewGeoAndIndustry.SelectedItems.Contains(item))
                            radGridViewGeoAndIndustry.SelectedItems.Add(item);
                    }
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;

                }
            }
            else
            {
                selectCheckBox.IsChecked = false;
                foreach (var item in radGridViewGeoAndIndustry.Items)
                {
                    radGridViewGeoAndIndustry.SelectedItems.Remove(item);
                }

            }

            if (notFoundLogos.Count > 0)
            {
                string joined = string.Join(",", notFoundLogos);
                CustomMessageBoxExcel.Show("No logo found for " + joined, "Logo Search", ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
            }
            checkBoxClicked = false;
            selectAllState(null);

        }

        private async void selectCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectCheckBox.IsChecked == true)
                {
                    if (radGridViewGeoAndIndustry.Items.Count > 300)
                    {
                        var res = CustomMessageBoxExcel.Show("You can add only upto 300 logos to Cart ", "Logo Search", ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                        selectCheckBox.IsChecked = false;
                    }
                    else
                    {

                        if (radGridViewGeoAndIndustry.Items.Count > 0)
                        {
                            List<string> lstLogoNames = new List<string>();
                            lstLogoNames = FileToDownload(radGridViewGeoAndIndustry.Items.Cast<LogoToolCompanyDataFrontEnd>().ToList());
                            if (lstLogoNames.Count > 0)
                            {
                                RadProgressBar1.Value = 0;
                                txtLoadingLabel.Text = "Preparing to download " + lstLogoNames.Count + " logo(s)...";
                                BrdrProgressIndicator.Visibility = Visibility.Visible;
                                await System.Threading.Tasks.Task.Factory.StartNew(() =>
                                {
                                    System.Threading.Thread.Sleep(200);
                                });
                                await DownloadAllLogosArtifacts(lstLogoNames);
                            }
                        }
                        Utils.GlobalEventForExcel.GetEvent<CrossModuleCommunication>().Publish(null);
                        System.Windows.Forms.Cursor.Current = Utils.CustomCursor;
                        foreach (LogoToolCompanyDataFrontEnd item in radGridViewGeoAndIndustry.Items)
                        {
                            if (!CartSource.Any(o => o.LogoData.ID.Equals(item.ID)))
                            {
                                string actualcountID = Guid.NewGuid().ToString();
                                int actualAddedSrNo = 1;
                                if (CartSource.Count > 0)
                                    actualAddedSrNo = cartSource.Max(o => o.SrNoText);
                                LogoTileVM LooTileModal = new LogoTileVM()
                                {
                                    LogoData = item,
                                    Tooltip = item.CompanyName,
                                    Strindex = actualcountID,
                                    SrNoText = actualAddedSrNo++
                                };
                                LooTileModal.LogoData.IsActive = true;
                                LooTileModal.DeleteFromCart += DeleteDataFromCart;
                                LooTileModal.CreateCopy += CreateCartDataCopy;
                                CartSource.Add(LooTileModal);
                                LooTileModal.loadData();
                                wrapPanelScroll.ScrollToBottom();

                                if (!radGridViewGeoAndIndustry.SelectedItems.Contains(item))
                                    radGridViewGeoAndIndustry.SelectedItems.Add(item);
                            }
                        }
                        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                        ResetCartStats();
                    }
                }
            }
            catch
            { }
        }

        private List<string> FileToDownload(List<LogoToolCompanyDataFrontEnd> CompanyList)
        {
            List<string> filesList = new List<string>();
            var tempFolderLocation = Utils.GetToolTempPath(Tooltype.LogoTool);
            foreach (var item in CompanyList)
            {
                if (!File.Exists(tempFolderLocation + item.SystemLogoName))
                    filesList.Add(item.SystemLogoName);
            }
            return filesList;
        }

        private void selectCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            radGridViewGeoAndIndustry.SelectedItems.Clear();
            //CartSource.Clear();
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
            try
            {
                CartTilePanel.ItemsSource = CartSource;
                radCartGridView.ItemsSource = CartSource;
                DesktopAlertManagerObj.CloseAllAlerts();
                busyIndicator.BusyContent = "Loading data...";
                busyIndicator.IsBusy = true;
                System.Threading.Tasks.Task.Factory.StartNew(async () =>
                {
                    Utils.TryDeleteFolder(Utils.GetToolTempPath(Tooltype.LogoTool));
                    await LoadFilterGroupData();
                    if (ManageFilterGroupCollection != null && ManageFilterGroupCollection.Count > 0)
                    {
                        await LoadData();
                    }
                    else
                    {
                        var alert = CustomMessageBoxExcel.Show("Unable to load data, please retry.", "Logo Tool", ExcelMessageBoxType.Retry, ExcelMessageBoxImage.Retry);
                        if (alert)
                            Window_Loaded(null, null);
                    }
                    Geography();
                    SectorIndustry();
                    FreeFilter1();
                    FreeFilter2();

                    Dispatcher.Invoke(() =>
                    {
                        if (IsActiveGeoFilter == false && IsActiveIndustryFilter == false && IsActiveFilter1 == false && IsActiveFilter2 == false)
                        {
                            stkPnlOR.Visibility = Visibility.Collapsed;
                            grdBottomBtns.Visibility = Visibility.Collapsed;
                            lblFilters.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            stkPnlOR.Visibility = Visibility.Visible;
                            grdBottomBtns.Visibility = Visibility.Visible;
                            lblFilters.Visibility = Visibility.Visible;
                            grdBottomBtns.IsEnabled = true;
                        }

                        setImportTypeSource();

                        busyIndicator.IsBusy = false;
                    });
                });
            }
            catch (Exception ex)
            {
                busyIndicator.BusyContent = "Loading...";
                busyIndicator.IsBusy = false;
            }
            editColumnChooser();
            SetIsEnabled(this.radCartGridView, true);
        }

        private void OnDragDropCompleted(object sender, DragDropCompletedEventArgs e)
        {
            HideDropPositionFeedbackPresenter();
        }

        private void searchradIndustryTree_SearchTextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(searchradIndustryTree.SearchText))
            {
                radIndustryTreeView.CollapseAll();
                RadTreeViewItem item = radIndustryTreeView.Items[0] as RadTreeViewItem;
                item.IsExpanded = true;
            }
        }

        private void searchTree_SearchTextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(searchTree.SearchText))
            {
                radGeographyTreeView.CollapseAll();
                RadTreeViewItem item = radGeographyTreeView.Items[0] as RadTreeViewItem;
                item.IsExpanded = true;
            }
        }

        private void searchBtnGeo_Click(object sender, RoutedEventArgs e)
        {
            if (isGeoitemSelected)
                searchTreeview(radGeographyTreeView, searchTree.SearchText.ToLower(), false);
            else
                searchGeographyTreeView();
        }

        private void searchBtnInd_Click(object sender, RoutedEventArgs e)
        {
            if (isIndustryitemSelected)
                searchTreeview(radIndustryTreeView, searchradIndustryTree.SearchText.ToLower(), false);
            else
                searchIndustryTreeView();
        }


        private void radBrandGridView_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {

        }

        System.Windows.Forms.SaveFileDialog dialog = null;
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (radGridViewGeoAndIndustry.Items.Count == 0)
            {
                CustomMessageBoxExcel.Show("Data grid is empty.", "Export", ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                return;
            }

            dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.Filter = "Excel Files|*.xlsx";
            dialog.Title = "Save the Exported excel";
            string datatime = DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("hhmmss");
            dialog.FileName = "LogoToolData_" + datatime;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                busyIndicator.IsBusy = true;
                busyIndicator.BusyContent = "Exporting Data...";
                //Dispatcher.Invoke(() =>
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    System.Threading.Thread.Sleep(200);
                    if (ExportToExcelButton == null)
                    {
                        ExportToExcelButton = new BackgroundWorker();
                        ExportToExcelButton.DoWork += ExportToExcelButton_DoWork;
                        ExportToExcelButton.RunWorkerCompleted += ExportToExcelButton_RunWorkerCompleted;
                    }
                    ExportToExcelButton.RunWorkerAsync();
                });

            }
        }
        private void ExportToExcelButton_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //busyIndicator.IsBusy = false;
        }

        private void ExportToExcelButton_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Dictionary<string, bool> dicClmchosser = new Dictionary<string, bool>();
                Dispatcher.Invoke(() =>
                {
                    using (Stream stream = dialog.OpenFile())
                    {
                        foreach (var item in lbColumnChooser.ItemsSource.Cast<GridViewDataColumn>().ToList())
                        {
                            if (!dicClmchosser.ContainsKey(item.Header.ToString()))
                                dicClmchosser.Add(item.Header.ToString(), item.IsVisible);
                        }
                        foreach (var item in lbColumnChooser.ItemsSource.Cast<GridViewDataColumn>().ToList())
                        {
                            item.IsVisible = true;
                        }
                        var opt = new GridViewDocumentExportOptions()
                        {
                            ShowColumnFooters = true,
                            ShowColumnHeaders = true,
                            ShowGroupFooters = true,
                            AutoFitColumnsWidth = true
                        };

                        opt.ExcludedColumns.Add(radGridViewGeoAndIndustry.Columns[2]);
                        radGridViewGeoAndIndustry.ExportToXlsx(stream, opt);
                    }
                    foreach (var item in dicClmchosser)
                    {
                        lbColumnChooser.ItemsSource.Cast<GridViewDataColumn>().ToList().FirstOrDefault(o => o.Header.ToString() == item.Key).IsVisible = item.Value;
                    }
                });
                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                Dispatcher.Invoke(() =>
                {
                    busyIndicator.IsBusy = false;
                    var res = CustomMessageBoxExcel.Show("Data exported successfully. " + dialog.FileName + ".Do you want to open " + System.IO.Path.GetFileName(dialog.FileName) + "?", "Logo Library", ExcelMessageBoxType.ConfirmationWithYesNo, ExcelMessageBoxImage.Information);
                    if (res)
                    {
                        xlApp.Workbooks.Open(dialog.FileName);
                        xlApp.Visible = true;
                    }
                });

                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
            }
            catch (IOException ex)
            {
                CustomMessageBoxExcel.Show(ex.Message + "\n Data exported Failed.", "Logo Library", ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Error);
                Dispatcher.Invoke(() => { busyIndicator.IsBusy = false; });
            }
        }

        private void btnExportCart_Click(object sender, RoutedEventArgs e)
        {
            dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.Filter = "Excel Files|*.xlsx";
            dialog.Title = "Save the Exported excel";
            string datatime = DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("hhmmss");
            dialog.FileName = "LogoToolData_" + datatime;
            bool IsLstViewSelected = tabItemListView.IsSelected;
            tabItemListView.IsSelected = true;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (IsLstViewSelected == false)
                    tabItemTileView.IsSelected = true;

                busyIndicator.IsBusy = true;
                busyIndicator.BusyContent = "Exporting Data...";

                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(200);
                    if (ExportToExcelButtonCart == null)
                    {
                        ExportToExcelButtonCart = new BackgroundWorker();
                        ExportToExcelButtonCart.DoWork += ExportToExcelButtonCart_DoWork;
                        ExportToExcelButtonCart.RunWorkerCompleted += ExportToExcelButtonCart_RunWorkerCompleted;
                    }
                    ExportToExcelButtonCart.RunWorkerAsync();
                });
            }
            else
            {
                if (IsLstViewSelected == false)
                    tabItemTileView.IsSelected = true;
            }
        }

        private void ExportToExcelButtonCart_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //busyIndicator.IsBusy = false;
        }

        private void ExportToExcelButtonCart_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    var opt = new GridViewDocumentExportOptions()
                    {
                        ShowColumnFooters = true,
                        ShowColumnHeaders = true,
                        ShowGroupFooters = true,
                        AutoFitColumnsWidth = true
                    };
                    opt.ExcludedColumns.Add(radCartGridView.Columns[2]);

                    using (Stream stream = dialog.OpenFile())
                    {
                        radCartGridView.ExportToXlsx(stream, opt);
                    }
                });
                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                Dispatcher.Invoke(() =>
                {
                    busyIndicator.IsBusy = false;
                    var res = CustomMessageBoxExcel.Show("Data exported successfully. " + dialog.FileName + ".Do you want to open " + System.IO.Path.GetFileName(dialog.FileName) + "?", "Logo Library", ExcelMessageBoxType.ConfirmationWithYesNo, ExcelMessageBoxImage.Information);
                    if (res)
                    {
                        xlApp.Workbooks.Open(dialog.FileName);
                        xlApp.Visible = true;
                    }
                });

                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
            }
            catch (IOException ex)
            {
                CustomMessageBoxExcel.Show(ex.Message + "\n Data exported Failed.", "Logo Library", ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Error);
                Dispatcher.Invoke(() => { busyIndicator.IsBusy = false; });
            }
        }

        private void logoGridPanel_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
        }

        private void previewColumn_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                btnDelete_Click(null, null);
        }

        private void tabcontrolPanelViews_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DesktopAlertManagerObj.CloseAllAlerts();
            if (tabItemTileView.IsSelected)
            {
                tablogoImg.Source = new BitmapImage(new Uri(@"pack://application:,,,/PitchreadyGlobal;component/ApplicationResources/Media/thumnail.png"));
                tablogoList.Source = new BitmapImage(new Uri(@"pack://application:,,,/PitchreadyPowerPoint;component/Media/Icons/LogoTool/Logo_list_G.png"));
                //selectAllState(null);
            }
            else
            {
                tablogoList.Source = new BitmapImage(new Uri(@"pack://application:,,,/PitchreadyGlobal;component/ApplicationResources/Media/logo_tool_list.png"));
                tablogoImg.Source = new BitmapImage(new Uri(@"pack://application:,,,/PitchreadyPowerPoint;component/Media/Icons/LogoTool/logo_images_G.png"));
                radGridViewlogoPreviewGrid.Visibility = Visibility.Visible;
            }
        }

        private void radGridViewGeoAndIndustry_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (radGridViewGeoAndIndustry.CurrentCell != null && e.Key == Key.Space)
            {
                var row = radGridViewGeoAndIndustry.CurrentCell.Content;
                if (row is System.Windows.Controls.CheckBox)
                {
                    (row as System.Windows.Controls.CheckBox).IsChecked = !(row as System.Windows.Controls.CheckBox).IsChecked;
                    e.Handled = true; //this is necessary because otherwise when the checkbox cell is selected, it will apply this keyup and also apply the default behavior for the checkbox
                }
            }
        }

        #region //Added and implemented by Anand 09/July/2019
        private Window _dragdropWindow = null; //Anand

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void logoWrapPanel_GiveFeedback(object sender, System.Windows.GiveFeedbackEventArgs e)
        {
            // update the position of the visual feedback item
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            this._dragdropWindow.Left = w32Mouse.X;
            this._dragdropWindow.Top = w32Mouse.Y;
        }
        private void CreateDragDropWindow(Visual dragElement)
        {
            this._dragdropWindow = new Window();
            _dragdropWindow.WindowStyle = WindowStyle.None;
            _dragdropWindow.AllowsTransparency = true;
            _dragdropWindow.AllowDrop = false;
            _dragdropWindow.Background = null;
            _dragdropWindow.IsHitTestVisible = false;
            _dragdropWindow.SizeToContent = SizeToContent.WidthAndHeight;
            _dragdropWindow.Topmost = true;
            _dragdropWindow.ShowInTaskbar = false;

            System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();
            r.Width = ((FrameworkElement)dragElement).ActualWidth;
            r.Height = ((FrameworkElement)dragElement).ActualHeight;
            r.Fill = new VisualBrush(dragElement);
            this._dragdropWindow.Content = r;


            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            this._dragdropWindow.Left = w32Mouse.X;
            this._dragdropWindow.Top = w32Mouse.Y;
            this._dragdropWindow.Show();
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        private void logoWrapPanel_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var logoWrapPanel = sender as WrapPanel;
            System.Windows.Point pt = e.GetPosition(logoWrapPanel);
            if (this._dragdropWindow != null)
            {
                if (pt.X < 0 || pt.Y < 0)
                {
                    if (e.Source is LogoTileUC)
                    {
                        (e.Source as LogoTileUC).Effect = null;
                        (e.Source as LogoTileUC).RenderTransform = null;
                    }
                    this._dragdropWindow.Close();
                    this._dragdropWindow = null;
                }
            }
        }
        private void brandTab_Loaded(object sender, RoutedEventArgs e)
        {
            string brandPath = System.IO.Path.Combine(targetdatabasePath, "Logo Tool", "BrandLogos");
            if (!Directory.Exists(brandPath))
                brandTab.Visibility = Visibility.Collapsed;

        }

        #endregion

        #region DRAG DROP Methods

        public void SetIsEnabled(DependencyObject obj, bool value)
        {
            if (value)
            {
                this.Initialize();
            }
            else
            {
                this.CleanUp();
            }
            obj.SetValue(IsEnabledProperty, value);
        }

        protected virtual void Initialize()
        {
            this.radCartGridView.RowLoaded -= this.AssociatedObject_RowLoaded;
            this.radCartGridView.RowLoaded += this.AssociatedObject_RowLoaded;
            this.UnsubscribeFromDragDropEvents();
            this.SubscribeToDragDropEvents();

            this.radCartGridView.Dispatcher.BeginInvoke((Action)(() =>
            {
                this.dropPositionFeedbackPresenter = new ContentPresenter();
                this.dropPositionFeedbackPresenter.Name = DropPositionFeedbackElementName;
                this.dropPositionFeedbackPresenter.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                this.dropPositionFeedbackPresenter.VerticalAlignment = VerticalAlignment.Top;
                this.dropPositionFeedbackPresenter.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);

                this.AttachDropPositionFeedback();
            }));
        }
        private bool IsDropPositionFeedbackAvailable()
        {
            return this.dropPositionFeedbackPresenter != null;
        }
        protected virtual void CleanUp()
        {
            this.radCartGridView.RowLoaded -= this.AssociatedObject_RowLoaded;
            this.UnsubscribeFromDragDropEvents();

            this.DetachDropPositionFeedback();
        }
        private void SubscribeToDragDropEvents()
        {
            DragDropManager.AddDragInitializeHandler(this.radCartGridView, OnDragInitialize);
            DragDropManager.AddGiveFeedbackHandler(this.radCartGridView, OnGiveFeedback);
            DragDropManager.AddDropHandler(this.radCartGridView, OnDrop);
            DragDropManager.AddDragDropCompletedHandler(this.radCartGridView, OnDragDropCompleted);
        }

        private void UnsubscribeFromDragDropEvents()
        {
            DragDropManager.RemoveDragInitializeHandler(this.radCartGridView, OnDragInitialize);
            DragDropManager.RemoveGiveFeedbackHandler(this.radCartGridView, OnGiveFeedback);
            DragDropManager.RemoveDropHandler(this.radCartGridView, OnDrop);
            DragDropManager.RemoveDragDropCompletedHandler(this.radCartGridView, OnDragDropCompleted);
        }

        private void OnDragInitialize(object sender, DragInitializeEventArgs e)
        {
            var radGridView = (sender as RadGridView);
            var sourceRow = e.OriginalSource as GridViewRow ?? (e.OriginalSource as FrameworkElement).ParentOfType<GridViewRow>();
            if (sourceRow != null && sourceRow.Name != "PART_RowResizer")
            {
                DropIndicationDetails details = new DropIndicationDetails();
                DragDropModel drModel = new DragDropModel()
                {
                    index = sourceRow.Cells[0].Content.ToString(),
                    ContentData = (sourceRow.Cells[1].Content as TextBlock).Text.ToString()
                };
                var item = sourceRow.Item;
                details.CurrentDraggedItem = drModel;
                var d = radGridView.GetRowForItem(item);
                var grv = d as GridViewRow;
                isChecked = (grv).IsSelected;
                Telerik.Windows.DragDrop.Behaviors.IDragPayload dragPayload = DragDropPayloadManager.GeneratePayload(null);

                dragPayload.SetData("DraggedItem", item);
                dragPayload.SetData("DropDetails", details);

                e.Data = dragPayload;

                e.DragVisual = new DragVisual()
                {
                    Content = details,
                    ContentTemplate = this.radCartGridView.Resources["DraggedItemTemplate"] as DataTemplate
                };
                e.DragVisualOffset = e.RelativeStartPoint;
                e.AllowedEffects = System.Windows.DragDropEffects.All;
            }
        }
        private void OnGiveFeedback(object sender, Telerik.Windows.DragDrop.GiveFeedbackEventArgs e)
        {
            e.SetCursor(System.Windows.Input.Cursors.Arrow);
            e.Handled = true;
        }

        private void OnDrop(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
        {
            bool sourceRowChecked = isChecked;
            bool destinationRowChecked = false;
            var GridRowObj = e.OriginalSource as GridViewRow;

            var destinationRow = e.OriginalSource as GridViewRow ?? (e.OriginalSource as FrameworkElement).ParentOfType<GridViewRow>();
            if (destinationRow == null)
            {
                HideDropPositionFeedbackPresenter();
                e.Handled = true;
                return;
            }
            LogoTileVM destinationDataRow = (destinationRow.DataContext as LogoTileVM);
            LogoTileVM sourceDataRow = (DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedItem") as LogoTileVM);

            if (destinationDataRow == null || sourceDataRow == null || destinationDataRow == sourceDataRow)
            {
                HideDropPositionFeedbackPresenter();
                e.Handled = true;
                return;
            }
            radCartGridView.SelectionChanged -= radGridViewlogoPreview_SelectionChanged;
            sourceRowChecked = radCartGridView.SelectedItems.Contains(sourceDataRow);
            destinationRowChecked = radCartGridView.SelectedItems.Contains(destinationDataRow);

            radCartGridView.SelectedItems.Remove(sourceDataRow);
            radCartGridView.SelectedItems.Remove(destinationDataRow);

            LogoTileVM toInsert = sourceDataRow;
            int index1 = CartSource.IndexOf(destinationDataRow);

            CartSource.Remove(sourceDataRow);
            CartSource.Insert(index1, toInsert);

            radCartGridView.Rebind();

            if (sourceRowChecked)
                radCartGridView.SelectedItems.Add(toInsert);
            if (destinationRowChecked)
                radCartGridView.SelectedItems.Add(destinationDataRow);
            radCartGridView.SelectionChanged += radGridViewlogoPreview_SelectionChanged;

            HideDropPositionFeedbackPresenter();

        }
        private void ShowDropPositionFeedbackPresenter(GridViewDataControl gridView, GridViewRow row)
        {
            if (!this.IsDropPositionFeedbackAvailable() || row == null)
                return;
            var yOffset = row.TransformToVisual(this.dropPositionFeedbackPresenterHost).Transform(new System.Windows.Point(0, 0)).Y;
            this.dropPositionFeedbackPresenter.Visibility = Visibility.Visible;
            this.dropPositionFeedbackPresenter.Width = row.ActualWidth;
            this.dropPositionFeedbackPresenter.RenderTransform = new TranslateTransform()
            {
                Y = yOffset
            };
        }
        void AssociatedObject_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            if (e.Row is GridViewHeaderRow || e.Row is GridViewNewRow || e.Row is GridViewFooterRow)
                return;

            GridViewRow row = e.Row as GridViewRow;
            this.InitializeRowDragAndDrop(row);
        }

        private void InitializeRowDragAndDrop(GridViewRow row)
        {
            if (row == null)
                return;

            DragDropManager.RemoveDragOverHandler(row, OnRowDragOver);
            DragDropManager.AddDragOverHandler(row, OnRowDragOver);
        }
        private void OnRowDragOver(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
        {
            var row = sender as GridViewRow;
            var details = DragDropPayloadManager.GetDataFromObject(e.Data, "DropDetails") as DropIndicationDetails;

            if (details == null || row == null)
            {
                return;
            }

            details.CurrentDraggedOverItem = new DragDropModel { index = row.Cells[0].Content.ToString(), ContentData = (row.Cells[1].Content as TextBlock).Text.ToString() };

            if (details.CurrentDraggedItem == details.CurrentDraggedOverItem)
            {
                e.Effects = System.Windows.DragDropEffects.None;
                e.Handled = true;
                return;
            }

            int dropIndex = (this.radCartGridView.Items as IList).IndexOf(row.DataContext);
            int draggedItemIdex = (this.radCartGridView.Items as IList).IndexOf(DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedItem"));

            details.DropIndex = dropIndex;
            this.ShowDropPositionFeedbackPresenter(this.radCartGridView, row);
        }

        private void HideDropPositionFeedbackPresenter()
        {
            if (this.dropPositionFeedbackPresenter == null) return;
            this.dropPositionFeedbackPresenter.RenderTransform = new TranslateTransform()
            {
                X = 0,
                Y = 0
            };
            this.dropPositionFeedbackPresenter.Visibility = Visibility.Collapsed;
            int count = 1;
            foreach (var item in CartSource)
            {
                item.LogoData.RowCounter = count;
                count++;
            }
        }

        private double GetDropPositionFeedbackOffset(GridViewRow row, DropPosition dropPosition)
        {
            var yOffset = row.TransformToVisual(this.dropPositionFeedbackPresenterHost).Transform(new System.Windows.Point(0, 0)).Y;
            if (dropPosition == DropPosition.After)
                yOffset += row.ActualHeight;
            yOffset -= (this.dropPositionFeedbackPresenter.ActualHeight / 2.0);
            return yOffset;
        }

        private void DetachDropPositionFeedback()
        {
            if (this.IsDropPositionFeedbackAvailable())
            {
                this.dropPositionFeedbackPresenterHost.Children.Remove(this.dropPositionFeedbackPresenter);
                this.dropPositionFeedbackPresenter = null;
            }
        }

        private void AttachDropPositionFeedback()
        {
            this.dropPositionFeedbackPresenterHost = radCartGridView.ParentOfType<Grid>();

            if (this.dropPositionFeedbackPresenterHost != null)
            {
                this.dropPositionFeedbackPresenter.Content = CreateDefaultDropPositionFeedback();
                if (dropPositionFeedbackPresenterHost != null && dropPositionFeedbackPresenterHost.FindName(this.dropPositionFeedbackPresenter.Name) == null)
                {
                    this.dropPositionFeedbackPresenterHost.Children.Add(this.dropPositionFeedbackPresenter);
                }
            }
            this.HideDropPositionFeedbackPresenter();
        }

        private UIElement CreateDefaultDropPositionFeedback()
        {
            Grid grid = new Grid()
            {
                Height = 8,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                IsHitTestVisible = false,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            grid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(8)
            });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(8)
            });
            System.Windows.Shapes.Ellipse ellipse = new System.Windows.Shapes.Ellipse()
            {
                Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#4682B4")),
                StrokeThickness = 2,
                Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#4682B4")),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 8,
                Height = 8
            };
            System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle()
            {
                Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#4682B4")),
                RadiusX = 2,
                RadiusY = 2,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                Height = 2
            };
            System.Windows.Shapes.Ellipse ellipseEnd = new System.Windows.Shapes.Ellipse()
            {
                Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#4682B4")),
                StrokeThickness = 2,
                Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#4682B4")),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 8,
                Height = 8
            };
            Grid.SetColumn(ellipse, 0);
            Grid.SetColumn(rectangle, 1);
            Grid.SetColumn(ellipseEnd, 2);
            grid.Children.Add(ellipse);
            grid.Children.Add(rectangle);
            grid.Children.Add(ellipseEnd);

            Canvas.SetZIndex(grid, 10000);

            return grid;
        }
        #endregion

        private void searchradFreeFilter1Tree_SearchTextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(searchradFreeFilter1Tree.SearchText))
            {
                radFreeFilter1TreeView.CollapseAll();
                RadTreeViewItem item = radFreeFilter1TreeView.Items[0] as RadTreeViewItem;
                item.IsExpanded = true;
            }
        }

        private void searchradFreeFilter1Tree_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string searchText = string.Empty;
            if (e.Key == Key.Enter)
            {
                searchFreeFilter1TreeView();
            }
        }

        private void searchBtnFilter3_Click(object sender, RoutedEventArgs e)
        {
            if (isFilter1itemSelected)
                searchTreeview(radFreeFilter1TreeView, searchradFreeFilter1Tree.SearchText.ToLower(), false);
            else
                searchFreeFilter1TreeView();
        }

        private void radFreeFilter1TreeView_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            RadTreeView treeView = sender as RadTreeView;
            if (e.Key == Key.Enter)
                searchTreeview(treeView, searchradFreeFilter1Tree.SearchText.ToLower(), false);
        }

        private void searchFreeFilter1TreeView()
        {
            radFreeFilter1TreeView.ExpandAll();
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(200);
                Dispatcher.Invoke(() =>
                {
                    if (!string.IsNullOrEmpty(searchradFreeFilter1Tree.SearchText))
                    {
                        Collection<RadTreeViewItem> allTreeContainers = GetAllNodes(radFreeFilter1TreeView);
                        foreach (RadTreeViewItem item in allTreeContainers)
                        {
                            if (item.Header.ToString().ToLower().ToString().Contains(searchradFreeFilter1Tree.SearchText.ToLower()))
                            {
                                item.IsSelected = true;
                                item.Focus();
                                isFilter1itemSelected = true;
                                return;
                            }

                        }
                        isFilter1itemSelected = false;
                        CustomMessageBoxExcel.Show("No records found ", "Confirmation", ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                    }
                });
            });
        }
        private void searchradFreeFilter2Tree_SearchTextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(searchradFreeFilter2Tree.SearchText))
            {
                radFreeFilter2TreeView.CollapseAll();
                RadTreeViewItem item = radFreeFilter2TreeView.Items[0] as RadTreeViewItem;
                item.IsExpanded = true;
            }
        }

        private void searchradFreeFilter2Tree_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string searchText = string.Empty;
            if (e.Key == Key.Enter)
            {
                searchFreeFilter2TreeView();
            }
        }

        private void searchBtnFilter4_Click(object sender, RoutedEventArgs e)
        {
            if (isFilter2itemSelected)
                searchTreeview(radFreeFilter2TreeView, searchradFreeFilter2Tree.SearchText.ToLower(), false);
            else
                searchFreeFilter2TreeView();
        }

        private void radFreeFilter2TreeView_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            RadTreeView treeView = sender as RadTreeView;
            if (e.Key == Key.Enter)
                searchTreeview(treeView, searchradFreeFilter2Tree.SearchText.ToLower(), false);
        }
        private void searchFreeFilter2TreeView()
        {
            radFreeFilter2TreeView.ExpandAll();
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(200);
                Dispatcher.Invoke(() =>
                {
                    if (!string.IsNullOrEmpty(searchradFreeFilter2Tree.SearchText))
                    {
                        Collection<RadTreeViewItem> allTreeContainers = GetAllNodes(radFreeFilter2TreeView);
                        foreach (RadTreeViewItem item in allTreeContainers)
                        {
                            if (item.Header.ToString().ToLower().ToString().Contains(searchradFreeFilter2Tree.SearchText.ToLower()))
                            {
                                item.IsSelected = true;
                                item.Focus();
                                isFilter2itemSelected = true;
                                return;
                            }

                        }
                        isFilter2itemSelected = false;
                        CustomMessageBoxExcel.Show("No records found ", "Confirmation", ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                    }
                });
            });
        }

        private void btnCompanyWebsite_Click(object sender, RoutedEventArgs e)
        {
            string Url = string.Empty;
            System.Windows.Controls.Button btn = e.Source as System.Windows.Controls.Button;
            string websiteUrl = (btn.DataContext as LogoToolCompanyDataFrontEnd).CompanyWebsite.ToString();
            if (string.IsNullOrEmpty(websiteUrl))
                return;
            if (websiteUrl.ToString().Contains("http") || websiteUrl.ToString().Contains("https"))
                Url = websiteUrl.ToString();
            else
                Url = "http://" + websiteUrl;
            System.Diagnostics.Process.Start(Url);
        }

        private void searchCompany_SearchTextChanged(object sender, EventArgs e)
        {
            if (SerchStats.Visibility == Visibility.Visible)
                SerchStats.Visibility = Visibility.Collapsed;
        }

        private void LogoForm_Closed(object sender, EventArgs e)
        {
            //logoPreview = null;
            Task.Factory.StartNew(() =>
            {
                Utils.TryDeleteFolder(Utils.GetToolTempPath(Enumrations.Tooltype.LogoTool));
            });
            this.Close();
        }

        private void txtLogoSize_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void txtLogoSize_GotFocus(object sender, RoutedEventArgs e)
        {
            Dictionary<object, object> s = new Dictionary<object, object>();
            s.Add("LogoToolFrontEndExcel", "");
            s.Add(cmbImportType.SelectedItem as string, "");
            s.Add("Resize", txtLogoSize.Text);
            foreach (LogoTileVM luc in CartTilePanel.Items)
            {
                s.Add(luc.LogoData.LocalLogoPath.ToString(), luc.LogoData.CompanyName + "_" + luc.LogoData.CompanyTicker);
            }

            Utils.InitiateWorkOnExcelEvent.GetEvent<CrossModuleCommunication>().Publish(s);
            this.WindowState = WindowState.Minimized;
        }


    }

    public class DragDropModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string index { get; set; }
        public string ContentData { get; set; }


    }
}
