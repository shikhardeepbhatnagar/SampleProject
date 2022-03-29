using GlobalUtilityLibrary;
using GlobalUtilityLibrary.Helpers;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using PitchreadyExcel.Forms;
using PitchreadyGlobal.Enumrations;
using PitchreadyGlobal.Helpers;
using PitchreadyGlobal.UserControls;
using PitchreadyGlobal.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows.Controls;
using RelayCommand = PitchreadyGlobal.Helpers.RelayCommand;

namespace PitchreadyExcel.ViewModel
{
    public class TemplateRepositoryFrontEndViewModel : PitchreadyGlobalBaseVM, IDisposable
    {
        public static event EventHandler NotifyViewEventToUncheck;
        public static event EventHandler NotifyViewEventToCheck;
        bool isFiltered;
        private RadTreeView treeViewCategory;
        private System.Windows.Controls.CheckBox selectAllPreviewCheckBox;
        string activeWorkbookName;
        public static bool IsFormOpen = false;
        public bool clearCart = true;
        List<ParentCategoryModel> TemplateCatagoryList = new List<ParentCategoryModel>();
        List<ParentCategoryModel> ParentCategoryList = new List<ParentCategoryModel>();
        List<ChildCategoryModel> TemplateDataList;
        public ObservableCollection<ChildCategoryModel> CartList = new ObservableCollection<ChildCategoryModel>();
        int rootCategoryID;

        private int _previewCount;
        public int PreviewCount
        {
            get
            {
                return _previewCount;
            }
            set
            {
                _previewCount = value;
                OnPropertyChanged("PreviewCount");
            }
        }

        private int _cartCount;
        public int CartCount
        {
            get
            {
                return _cartCount;
            }
            set
            {
                _cartCount = value;
                OnPropertyChanged("CartCount");
            }
        }

        private ObservableCollection<ParentCategoryModel> _itemsSourceForTreeStructure = new ObservableCollection<ParentCategoryModel>();
        public ObservableCollection<ParentCategoryModel> ItemsSourceForTreeStructure
        {
            get { return _itemsSourceForTreeStructure; }
            set
            {
                _itemsSourceForTreeStructure = value;
                OnPropertyChanged("ItemsSourceForTreeStructure");
            }
        }

        private ObservableCollection<TemplateRepoFrontEndModel> _workBookCollection = new ObservableCollection<TemplateRepoFrontEndModel>();
        public ObservableCollection<TemplateRepoFrontEndModel> WorkBookCollection
        {
            get { return _workBookCollection; }
            set
            {
                _workBookCollection = value;
                OnPropertyChanged("WorkBookCollection");
            }
        }

        private ObservableCollection<string> _comboboxItems = new ObservableCollection<string>();
        public ObservableCollection<string> ComboboxItems
        {
            get
            {
                return _comboboxItems;
            }
            set
            {
                _comboboxItems = value;

                OnPropertyChanged("ComboboxItems");
            }
        }

        private string _cmbBoxSelectedItem;
        public string CmbBoxSelectedItem
        {
            get
            {
                return _cmbBoxSelectedItem;
            }
            set
            {
                _cmbBoxSelectedItem = value;
                OnPropertyChanged("CmbBoxSelectedItem");
            }
        }

        private string _searchCategoryText;
        public string SearchCategoryText
        {
            get
            {
                return _searchCategoryText;
            }
            set
            {
                _searchCategoryText = value;
                OnCategoryTextChanged(ItemsSourceForTreeStructure.FirstOrDefault().Childs);
                OnPropertyChanged("SearchCategoryText");
            }
        }

        private RelayCommand _exportCommand;
        public RelayCommand ExportCommand
        {
            get { return _exportCommand; }
            set
            {
                _exportCommand = value;
                OnPropertyChanged("ExportCommand");
            }
        }

        private RelayCommand _deleteAllCommand;
        public RelayCommand DeleteAllCommand
        {
            get { return _deleteAllCommand; }
            set
            {
                _deleteAllCommand = value;
                OnPropertyChanged("DeleteAllCommand");
            }
        }

        private bool _isSearchStatVisible;
        public bool IsSearchStatVisible
        {
            get
            {
                return _isSearchStatVisible;
            }
            set
            {
                _isSearchStatVisible = value;
                OnPropertyChanged("IsSearchStatVisible");
            }
        }

        private System.Windows.Media.Brush _searchForeColor = Brushes.Black;
        public System.Windows.Media.Brush SearchForeColor
        {
            get
            {
                return _searchForeColor;
            }
            set
            {
                _searchForeColor = value;
                OnPropertyChanged("SearchForeColor");
            }
        }

        public TemplateRepositoryFrontEndViewModel(int rootCategoryID, RadTreeView treeViewCategory, System.Windows.Controls.CheckBox checkBox)
        {
            this.rootCategoryID = rootCategoryID;
            this.treeViewCategory = treeViewCategory;
            this.selectAllPreviewCheckBox = checkBox;
            ParentCategoryModel.SendParentCategoryToFrontEndWindow += ParentCategoryModel_SendParentCategoryToFrontEndWindow;

            TemplateRepoFrontEndModel.SendObjectDetailsForPreview += TemplateRepoFrontEndModel_SendObjectDetailsForPreview;

            ChildCategoryModel.SendDeleteDataToCart += ChildCategoryModel_SendDeleteDataToCart;

            ChildCategoryModel.SendPreviewDataToCart += ChildCategoryModel_SendPreviewDataToCart;

            TemplateRepoFrontEndModel.SendObjectDetailsForCheckUnCheck += TemplateRepoFrontEndModel_SendObjectDetailsForCheckUnCheck;

            ComboboxItems.Add("Active Workbook");
            ComboboxItems.Add("New Workbook");

            CmbBoxSelectedItem = "Active Workbook";

            ExportCommand = new RelayCommand(ExportCommandHandler, ExecuteExport);

            DeleteAllCommand = new RelayCommand(DeleteAllCommandHandler, ExecuteExport);

            activeWorkbookName = Globals.ThisAddIn.Application.ActiveWorkbook.Name;

            SetItemsOnLoad();
        }

        private void TemplateRepoFrontEndModel_SendObjectDetailsForCheckUnCheck(object sender, EventArgs e)
        {
            if (sender != null)
            {
                var obj = sender as TemplateRepoFrontEndModel;

                if (obj.IsPreviewSelected)
                {
                    var data = TemplateDataList.Where(x => x.TemplateWorkbookId == obj.TemplateWorkbookId).FirstOrDefault();
                    CartList.Add(data);

                    var areAllSelected = true;
                    foreach (var item in WorkBookCollection)
                    {
                        if (!item.IsPreviewSelected)
                        {
                            areAllSelected = false;
                            break;
                        }
                    }
                    if (areAllSelected)
                    {
                        NotifyViewEventToCheck?.Invoke(this, null);
                    }
                }

                else if (!obj.IsPreviewSelected)
                {
                    NotifyViewEventToUncheck?.Invoke(this, null);

                    var data = TemplateDataList.Where(x => x.TemplateWorkbookId == obj.TemplateWorkbookId).FirstOrDefault();
                    CartList.Remove(data);
                }

                AddRemoveComboBoxItem();
            }
        }

        public async void SetItemsOnLoad()
        {
            await OnWindowLoaded();

            var tempList = ParentCategoryList.Where(o => o.Id == rootCategoryID).ToList();

            ItemsSourceForTreeStructure.Clear();
            foreach (var item in tempList)
            {
                ItemsSourceForTreeStructure.Add(item);
            }


            treeViewCategory.ExpandAll();
        }

        private void ChildCategoryModel_SendPreviewDataToCart(object sender, EventArgs e)
        {
            var itemToPreview = (sender as ChildCategoryModel);
            ExcelPreviewViewerWindow window = new ExcelPreviewViewerWindow(itemToPreview);
            window.ShowDialog();
        }

        private void ChildCategoryModel_SendDeleteDataToCart(object sender, EventArgs e)
        {
            try
            {
                var itemId = (sender as ChildCategoryModel).TemplateWorkbookId;
                CartList.Remove(CartList.Where(x => x.TemplateWorkbookId == itemId).FirstOrDefault());
                AddRemoveComboBoxItem();
                WorkBookCollection.Where(x => x.TemplateWorkbookId == itemId).FirstOrDefault().IsPreviewSelected = false;
                NotifyViewEventToUncheck?.Invoke(this, null);
            }
            catch
            {

            }
        }

        private void OnCategoryTextChanged(ObservableCollection<ParentCategoryModel> parentCategoryModels)
        {
            try
            {


                var CategoryDetails = new List<ParentCategoryModel>();
                foreach (var item in TemplateCatagoryList)
                {
                    item.Childs = new ObservableCollection<ParentCategoryModel>();
                    item.isDeleted = false;
                }
                CategoryDetails = TemplateCatagoryList.Where(o => o.Id == rootCategoryID).ToList();
                foreach (var item in CategoryDetails)
                {
                    Getdata(item);
                }
                if (!String.IsNullOrEmpty(_searchCategoryText))
                {
                    isFiltered = false;
                    FilterCollection(CategoryDetails.FirstOrDefault(), _searchCategoryText);
                    if (isFiltered)
                    {
                        SearchForeColor = System.Windows.Media.Brushes.Black;
                        IsSearchStatVisible = false;
                    }
                    else
                    {
                        SearchForeColor = Brushes.Red;
                        IsSearchStatVisible = true;
                    }
                    DeleteNodes(CategoryDetails.FirstOrDefault());
                    ItemsSourceForTreeStructure.Clear();
                    CategoryDetails.ForEach(item => ItemsSourceForTreeStructure.Add(item));
                }
                else
                {
                    IsSearchStatVisible = false;
                    SearchForeColor = Brushes.Black;
                    ItemsSourceForTreeStructure.Clear();
                    CategoryDetails.ForEach(item => ItemsSourceForTreeStructure.Add(item));
                }

                treeViewCategory.ExpandAll();
            }
            catch (Exception ex)
            {

            }
        }

        private void FilterCollection(ParentCategoryModel filterList, string text)
        {
            if (!filterList.CategoryName.ToLower().Contains(text.ToLower()))
                filterList.isDeleted = true;
            else
                isFiltered = true;

            foreach (var item in filterList.Childs)
            {
                if (!item.CategoryName.ToLower().Contains(text.ToLower()))
                {
                    item.isDeleted = true;
                }
                else
                {
                    isFiltered = true;
                    item.ParentNode.isDeleted = false;
                    retainparent(item);
                }
                FilterCollection(item, text);
            }
        }

        private void retainparent(ParentCategoryModel item)
        {
            if (item.ParentNode != null)
            {
                item.ParentNode.isDeleted = false;
                retainparent(item.ParentNode);
            }
        }

        private void DeleteNodes(ParentCategoryModel filteredList)
        {
            if (filteredList.isDeleted)
            {
                if (filteredList.ParentNode != null)
                    filteredList.ParentNode.Childs.Remove(filteredList);
            }
            else
            {
                ParentCategoryModel[] array = new ParentCategoryModel[filteredList.Childs.Count()];
                filteredList.Childs.ToList().CopyTo(array);
                foreach (var item in array)
                {
                    DeleteNodes(item);
                }
            }
        }

        //To Add and Remove from Cart
        public void SelectionChangedCommandHandler(TemplateRepoFrontEndModel obj, string key)
        {
            if (obj != null)
            {
                var item = TemplateDataList.Where(x => x.TemplateWorkbookId == obj.TemplateWorkbookId).FirstOrDefault();
                if (key == "AddToCart")
                    CartList.Add(item);
                else
                    CartList.Remove(item);

                AddRemoveComboBoxItem();
            }
        }

        public override void Dispose()
        {
            ParentCategoryModel.SendParentCategoryToFrontEndWindow -= ParentCategoryModel_SendParentCategoryToFrontEndWindow;

            TemplateRepoFrontEndModel.SendObjectDetailsForPreview -= TemplateRepoFrontEndModel_SendObjectDetailsForPreview;

            ChildCategoryModel.SendDeleteDataToCart -= ChildCategoryModel_SendDeleteDataToCart;

            ChildCategoryModel.SendPreviewDataToCart -= ChildCategoryModel_SendPreviewDataToCart;

            TemplateRepoFrontEndModel.SendObjectDetailsForCheckUnCheck -= TemplateRepoFrontEndModel_SendObjectDetailsForCheckUnCheck;

            if (TemplateCatagoryList != null)
                TemplateCatagoryList.Clear();

            if (TemplateDataList != null)
                TemplateDataList.Clear();
        }

        public void SelectionChangedCommandHandlerMultipleChecks(List<dynamic> obj, string key)
        {
            if (obj != null)
            {
                foreach (var item in obj)
                {
                    var data = TemplateDataList.Where(x => x.TemplateWorkbookId == item.TemplateWorkbookId).FirstOrDefault();
                    item.IsGridViewItemSelected = true;

                    if (key == "AddToCart")
                        CartList.Add(data);
                    else
                        CartList.Remove(data);
                }
                AddRemoveComboBoxItem();
            }
        }

        public void AddRemoveComboBoxItem()
        {
            if (CartList.Count > 1)
            {
                if (!ComboboxItems.Contains("Different Workbooks"))
                    ComboboxItems.Add("Different Workbooks");
            }
            else
            {
                if (ComboboxItems.Contains("Different Workbooks"))
                {
                    ComboboxItems.Remove("Different Workbooks");
                }
            }

            ComboboxItems.DistinctBy(x => x);
            CartCount = CartList.Count;
            PreviewCount = WorkBookCollection.Count();
        }

        private void TemplateRepoFrontEndModel_SendObjectDetailsForPreview(object sender, EventArgs e)
        {
            try
            {
                //Dictionary<string, string> Params = new Dictionary<string, string>();
                //Params.Add("CategoryID", rootCategoryID.ToString());

                //var TrData = await ServiceCallHelper.GetDataByParameter<ObservableCollection<ChildCategoryModel>>
                //    ("pitchready/TemplateRepositoryExcel/GetAllChildCatagories", Params, GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);


                //if (TrData != null)
                //{
                //var item = TrData.Where(o => o.TemplateWorkbookId != null && o.TemplateWorkbookId == (sender as TemplateRepoFrontEndModel).TemplateWorkbookId).FirstOrDefault();
                var item = TemplateDataList.Where(o => o.TemplateWorkbookId == (sender as TemplateRepoFrontEndModel).TemplateWorkbookId).FirstOrDefault();
                ExcelPreviewViewerWindow window = new ExcelPreviewViewerWindow(item);
                window.Show();
                //}
            }
            catch (Exception ex)
            {

            }
        }

        private void ParentCategoryModel_SendParentCategoryToFrontEndWindow(object sender, EventArgs e)
        {
            var obj = sender as ParentCategoryModel;
            var templateCategory = TemplateCatagoryList.Where(o => o.Id == obj.Id).FirstOrDefault();

            if (obj.FrontEndRootCategorySelected && templateCategory != null)
            {
                if (templateCategory.Childs != null)
                {
                    CheckAllChilds(templateCategory, templateCategory.Childs);

                    CheckParents(templateCategory);

                    PreviewCount = WorkBookCollection.Count;

                    var count = 0;

                    if (CartList.Count > 0)
                    {
                        foreach (var item in CartList)
                        {
                            foreach (var wcItem in WorkBookCollection)
                            {
                                if (wcItem.TemplateWorkbookId == item.TemplateWorkbookId)
                                {
                                    ++count;
                                    wcItem.IsPreviewSelected = true;
                                }
                            }
                        }
                    }

                    if (WorkBookCollection.Count == count && WorkBookCollection.Count != 0)
                    {
                        NotifyViewEventToCheck?.Invoke(this, null);
                    }

                    else
                    {
                        NotifyViewEventToUncheck?.Invoke(this, null);
                    }

                }
            }
            else
            {
                if (templateCategory != null && templateCategory.Childs != null)
                {
                    UnCheckAllChilds(templateCategory, templateCategory.Childs);

                    UncheckParents(templateCategory);

                    var count = 0;

                    if (CartList.Count > 0)
                    {
                        foreach (var item in CartList)
                        {
                            foreach (var wcItem in WorkBookCollection)
                            {
                                if (wcItem.TemplateWorkbookId == item.TemplateWorkbookId && wcItem.IsPreviewSelected)
                                {
                                    ++count;
                                }
                            }
                        }
                    }

                    if (WorkBookCollection.Count == count && count != 0)
                    {
                        NotifyViewEventToCheck?.Invoke(this, null);
                    }

                    else
                    {
                        NotifyViewEventToUncheck?.Invoke(this, null);
                    }

                }
            }

            PreviewCount = WorkBookCollection.Count();
        }

        private void UncheckParents(ParentCategoryModel parent)
        {
            parent.FrontEndRootCategorySelected = false;
            var tempVar = TemplateCatagoryList.Where(x => x.Id == parent.CategoryParentId).FirstOrDefault();
            if (tempVar == null)
            {
                return;
            }
            UncheckParents(tempVar);
        }

        private void CheckParents(ParentCategoryModel parent)
        {
            var tempVar = TemplateCatagoryList.Where(x => x.Id == parent.CategoryParentId).FirstOrDefault();
            if (tempVar != null)
            {
                bool checkIfAllChildrenSelected = true;
                foreach (var item in tempVar.Childs)
                {
                    if (!item.FrontEndRootCategorySelected)
                    {
                        checkIfAllChildrenSelected = false;
                        break;
                    }
                }

                if (checkIfAllChildrenSelected)
                {
                    tempVar.FrontEndRootCategorySelected = true;
                    CheckParents(tempVar);
                }
            }
        }

        private void CheckAllChilds(ParentCategoryModel parentSender, ObservableCollection<ParentCategoryModel> sender)
        {
            if (parentSender.Childs.Count == 0)
            {
                parentSender.FrontEndRootCategorySelected = true;
                if (parentSender.ChildCount > 0)
                {
                    var templateList = TemplateDataList.Where(o => Convert.ToInt32(o.CategoryID) == parentSender.Id).ToList();
                    foreach (var element in templateList)
                    {
                        WorkBookCollection.Add(new TemplateRepoFrontEndModel
                        {
                            IsPreviewAvailable = element.IsPreviewAvailable,
                            FileName = element.TemplateName.Trim(),
                            Date = element.UpdatedAtTemplate,
                            FileSize = element.FileSizeInKB.Value,
                            Description = element.Description,
                            TemplateWorkbookId = element.TemplateWorkbookId
                        });
                    }
                }
            }

            else
            {
                foreach (var item in sender)
                {
                    item.FrontEndRootCategorySelected = true;
                    var templateList = TemplateDataList.Where(o => Convert.ToInt32(o.CategoryID) == item.Id).ToList();
                    foreach (var element in templateList)
                    {
                        WorkBookCollection.Add(new TemplateRepoFrontEndModel
                        {
                            IsPreviewAvailable = element.IsPreviewAvailable,
                            FileName = element.TemplateName.Trim(),
                            Date = element.UpdatedAtTemplate,
                            FileSize = element.FileSizeInKB.Value,
                            Description = element.Description,
                            TemplateWorkbookId = element.TemplateWorkbookId
                        });
                    }

                    CheckAllChilds(item, item.Childs);
                }
            }
            var temp = WorkBookCollection.DistinctBy(x => x.TemplateWorkbookId).ToList();
            WorkBookCollection.Clear();
            foreach (var item in temp)
            {
                WorkBookCollection.Add(item);
            }
        }

        public void CheckAllPreviewsForCart()
        {
            //CartList.Clear();
            foreach (var item in WorkBookCollection)
            {
                item.IsPreviewSelected = true;
                var data = TemplateDataList.Where(x => x.TemplateWorkbookId == item.TemplateWorkbookId).FirstOrDefault();
                CartList.Add(data);
            }

            AddRemoveComboBoxItem();
        }

        public void UncheckAllPreviews()
        {
            CartList.Clear();
            WorkBookCollection.ToList().ForEach(item => item.IsPreviewSelected = false);

            AddRemoveComboBoxItem();
        }

        private void UnCheckAllChilds(ParentCategoryModel parentSender, ObservableCollection<ParentCategoryModel> sender)
        {
            if (parentSender.Childs.Count == 0)
            {
                parentSender.FrontEndRootCategorySelected = false;
                if (parentSender.ChildCount > 0)
                {
                    var templateList = TemplateDataList.Where(o => Convert.ToInt32(o.CategoryID) == parentSender.Id).ToList();
                    foreach (var element in templateList)
                    {
                        clearCart = false;
                        var itemToRemove = WorkBookCollection.FirstOrDefault(x => x.TemplateWorkbookId == element.TemplateWorkbookId);
                        WorkBookCollection.Remove(itemToRemove);
                    }
                }
            }

            else
            {
                foreach (var item in sender)
                {
                    item.FrontEndRootCategorySelected = false;
                    var templateList = TemplateDataList.Where(o => Convert.ToInt32(o.CategoryID) == item.Id).ToList();
                    foreach (var element in templateList)
                    {
                        clearCart = false;
                        var itemToRemove = WorkBookCollection.FirstOrDefault(x => x.TemplateWorkbookId == element.TemplateWorkbookId);
                        WorkBookCollection.Remove(itemToRemove);
                    }
                    UnCheckAllChilds(item, item.Childs);
                }
            }
        }

        private async Task GetCatagories()
        {
            ParentCategoryList = await ServiceCallHelper.GetData<List<ParentCategoryModel>>("pitchready/TemplateRepositoryExcel/GetAllParentCatagories", GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);
        }

        private async Task GetTemplateDeatils()
        {
            Dictionary<string, string> Params = new Dictionary<string, string>();
            Params.Add("CategoryID", rootCategoryID.ToString());
            var TrData = await ServiceCallHelper.GetDataByParameter<ObservableCollection<ChildCategoryModel>>("pitchready/TemplateRepositoryExcel/GetAllChildCatagories", Params, GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);

            if (TrData != null)
            {
                string jsondata = Newtonsoft.Json.JsonConvert.SerializeObject(TrData);
                TemplateDataList = TrData.Where(o => o.TemplateWorkbookId != null).ToList();
                var data = new List<ParentCategoryModel>(JsonConvert.DeserializeObject<List<ParentCategoryModel>>(jsondata).Distinct(new TemplateRepositoryDetailsComparer()));
                TemplateCatagoryList.Clear();
                TemplateCatagoryList = data;
            }
        }

        public async Task OnWindowLoaded()
        {
            try
            {
                IsLoading = true;
                //if (Directory.Exists(GlobalUtility.GetToolTempPath(GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepositoryExcel)))
                //{
                //    GlobalUtility.TryDeleteFolder(GlobalUtility.GetToolTempPath(GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepositoryExcel));
                //}
                await GetCatagories();
                await GetTemplateDeatils();
                var ChoosenParentCategory = ParentCategoryList.Where(o => o.Id == rootCategoryID).FirstOrDefault();
                TemplateCatagoryList.Add(ChoosenParentCategory);
                Getdata(ChoosenParentCategory);
                //updatePreviewCount();
                //updateCartCount();
            }
            catch
            {

            }
            finally
            {
                IsLoading = false;

            }
        }

        private void Getdata(ParentCategoryModel nodes)
        {
            nodes.ChildCount = (TemplateDataList.Where(o => Convert.ToInt32(o.CategoryID) == nodes.Id)).Count();
            IncreaseParentCount(nodes, nodes.ChildCount);
            if (nodes.Childs == null)
                nodes.Childs = new System.Collections.ObjectModel.ObservableCollection<ParentCategoryModel>();
            var RefinedChilds = TemplateCatagoryList.Where(o => o.CategoryParentId == nodes.Id).ToList();
            foreach (var child in RefinedChilds)
            {
                child.ParentNode = nodes;
                nodes.Childs.Add(child);
                Getdata(child);
            }
        }

        private void IncreaseParentCount(ParentCategoryModel child, int TempCount)
        {
            try
            {
                if (child.ParentNode == null) return;
                var ParentObj = child.ParentNode as ParentCategoryModel;
                if (ParentObj != null)
                {
                    ParentObj.ChildCount += TempCount;
                    IncreaseParentCount(ParentObj, TempCount);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public bool ExecuteExport(object obj)
        {
            bool val = CartList.Count >= 1 ? true : false;
            return val;
        }


        private async void ExportCommandHandler(object obj)
        {
            Stopwatch ObjStopwatch = new Stopwatch();
            ObjStopwatch.Start();
            IsLoading = true;
            try
            {

                ThisAddIn.IsPitchreadyDefaultTab = true;

                if (CmbBoxSelectedItem == "Different Workbooks")
                {
                    int count = 1;
                    foreach (var item in CartList)
                    {
                        var Result = await requestFile(item.SystemFileName);

                        if (Result == null)
                        {
                            CustomMessageBoxExcel.Show("Unable to download template. Please try again.", GlobalUtilityLibrary.Helpers.Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                            return;
                        }
                        string DirectoryPath = GlobalUtility.GetToolTempPath(GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository) + DtkConstants.NewTemplates + Path.DirectorySeparatorChar
                                          + "TemplateToOpen";

                        if (!Directory.Exists(DirectoryPath))
                        {
                            Directory.CreateDirectory(DirectoryPath);
                        }

                        string templatePath = DirectoryPath + Path.DirectorySeparatorChar + "Book" + count + Path.GetExtension(Result.FirstOrDefault().Value);

                        File.Copy(Result.FirstOrDefault().Value, templatePath, true);

                        Globals.ThisAddIn.Application.Workbooks.Add(templatePath);

                        ++count;

                    }
                }

                if (CmbBoxSelectedItem == "Active Workbook")
                {
                    Globals.ThisAddIn.Application.Workbooks[activeWorkbookName].Activate();

                    foreach (var item in CartList)
                    {
                        var Result = await requestFile(item.SystemFileName);
                        if (Result == null)
                        {
                            return;
                        }

                        var FileLocation = Result.FirstOrDefault().Value;

                        //Open Excel
                        Workbook xlWorkbook = Globals.ThisAddIn.Application.Workbooks.Open(FileLocation, Type.Missing, false, Type.Missing, Type.Missing,
                            Type.Missing, false, XlPlatform.xlWindows, Type.Missing,
                            true, false, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                        Globals.ThisAddIn.Application.DisplayAlerts = false;
                        Globals.ThisAddIn.Application.ScreenUpdating = false;

                        var workbookName = xlWorkbook.Name;

                        for (int i = 0; i < xlWorkbook.Worksheets.Count; i++)
                        {
                            Worksheet temp = xlWorkbook.Sheets[i + 1] as Worksheet;
                            temp.Visible = XlSheetVisibility.xlSheetVisible;
                            Globals.ThisAddIn.Application.Interactive = false;
                            temp.Copy(Type.Missing, Globals.ThisAddIn.Application.Workbooks[activeWorkbookName].Worksheets[i + 1]);
                            Globals.ThisAddIn.Application.Interactive = true;
                        }
                        //End

                        Globals.ThisAddIn.Application.Workbooks[workbookName].Activate();

                        CloseExcel(xlWorkbook);
                    }
                }

                if (CmbBoxSelectedItem == "New Workbook")
                {
                    //var wb = await requestFile(CartList.FirstOrDefault().SystemFileName);
                    //if (wb == null)
                    //{
                    //    CustomMessageBoxExcel.Show("Unable to download template. Please try again.", GlobalUtilityLibrary.Helpers.Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                    //    return;
                    //}

                    //string DirectoryPath = GlobalUtility.GetToolTempPath(GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository) + DtkConstants.NewTemplates + Path.DirectorySeparatorChar
                    //                      + "TemplateToOpen";

                    //if (!Directory.Exists(DirectoryPath))
                    //{
                    //    Directory.CreateDirectory(DirectoryPath);
                    //}

                    //string templatePath = DirectoryPath + Path.DirectorySeparatorChar + "Book" + 1 + Path.GetExtension(wb.FirstOrDefault().Value);

                    //File.Copy(wb.FirstOrDefault().Value, templatePath, true);


                    //Workbook openWb = Globals.ThisAddIn.Application.Workbooks.Open(wb.FirstOrDefault().Value, Type.Missing, false, Type.Missing, Type.Missing,
                    //        Type.Missing, false, XlPlatform.xlWindows, Type.Missing,
                    //        true, false, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                    Globals.ThisAddIn.Application.Workbooks.Add(Type.Missing);

                    var wbName = Globals.ThisAddIn.Application.ActiveWorkbook.Name;

                    for (int i = 0; i < CartList.Count; i++)
                    {
                        var Result = await requestFile(CartList[i].SystemFileName);
                        if (Result == null)
                        {
                            return;
                        }

                        var FileLocation = Result.FirstOrDefault().Value;

                        Globals.ThisAddIn.Application.DisplayAlerts = false;
                        Globals.ThisAddIn.Application.ScreenUpdating = false;

                        //Open Excel
                        Workbook xlWorkbook = Globals.ThisAddIn.Application.Workbooks.Open(FileLocation, Type.Missing, false, Type.Missing, Type.Missing,
                            Type.Missing, false, XlPlatform.xlWindows, Type.Missing,
                            true, false, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                        var workbookName = xlWorkbook.Name;

                        for (int j = 1; j <= xlWorkbook.Worksheets.Count; j++)
                        {
                            Worksheet temp = xlWorkbook.Sheets[j] as Worksheet;
                            temp.Visible = XlSheetVisibility.xlSheetVisible;
                            Globals.ThisAddIn.Application.Interactive = false;
                            temp.Copy(Globals.ThisAddIn.Application.Workbooks[wbName].Worksheets[j]);
                            Globals.ThisAddIn.Application.Interactive = true;
                        }
                        //End

                        Globals.ThisAddIn.Application.Workbooks[workbookName].Activate();

                        PitchreadyExcel.ribbonObj.ActivateTab("PR_EXL_Tab");

                        CloseExcel(xlWorkbook);
                    }
                }

                string macID = Utils.GetMACAddress();
                string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                long elapsedTime = ObjStopwatch.ElapsedMilliseconds / 1000;
                string elapsedTimestring = elapsedTime.ToString();
                if (elapsedTime > 0)
                    elapsedTimestring = elapsedTime.ToString() + " Seconds";
                var Tdata = ServiceCallHelper.SendFeatureLogsToServer
                (userName, macID, "Template Repository",
                "Template Repository",
                "Export - Excel",
                elapsedTimestring, GlobalUtilityLibrary.Helpers.Constants.NA, (CartList.Count).ToString());
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsLoading = false;
                ThisAddIn.IsPitchreadyDefaultTab = false;
                CustomMessageBoxExcel.Show("Template(s) exported successfully.", GlobalUtilityLibrary.Helpers.Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                this.Close();
                Globals.ThisAddIn.Application.Visible = true;
                Globals.ThisAddIn.Application.ScreenUpdating = true;
                ObjStopwatch.Stop();
                ObjStopwatch = null;
            }
        }

        private void DeleteAllCommandHandler(object obj)
        {
            WorkBookCollection.ToList().ForEach(item => item.IsPreviewSelected = false);

            selectAllPreviewCheckBox.IsChecked = false;

            CartList.Clear();

            AddRemoveComboBoxItem();
        }

        private void CloseExcel(Microsoft.Office.Interop.Excel.Workbook excelWorkbook)
        {
            if (excelWorkbook != null)
            {
                excelWorkbook.Close(Type.Missing, Type.Missing, Type.Missing);

                Marshal.ReleaseComObject(excelWorkbook);

                excelWorkbook = null;
            }
        }

        public async Task<Dictionary<string, string>> requestFile(string fileName)
        {
            Dictionary<string, string> returnableFiledetails = new Dictionary<string, string>();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                //if (files.Count == 0)
                //    return returnableFiledetails;

                //if (string.IsNullOrEmpty(GlobalUtility.adToken))
                //{
                //    GlobalUtility.adToken = GlobalUtilityLibrary.Helpers.ServiceCallHelper.GetToken(GlobalUtility.DataBasePathOneTime, Constants.userName, Constants.password);
                //}
                string tempDirectory = Utils.GetToolTempPath(Tooltype.TemplateRepository);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var processMsgHander = new ProgressMessageHandler(new HttpClientHandler());
                HttpClient client = new HttpClient(processMsgHander);
                client.Timeout = TimeSpan.FromMinutes(10);
                processMsgHander.HttpReceiveProgress += (sender, e) =>
                {
                };
                //if (!string.IsNullOrWhiteSpace(GlobalUtility.adToken))
                //{
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + await GlobalUtilityLibrary.Helpers.ServiceCallHelper.getAccessToken(Utils.DataLocationPath(Tooltype.TemplateRepository)));
                //    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + GlobalUtility.adToken);
                //}
                var result = await client.GetAsync(new Uri(Utils.DataLocationPath(Tooltype.TemplateRepository) + "/pitchready/TemplateRepositoryExcel/getfile/" + fileName));
                if (result.IsSuccessStatusCode)
                {
                    if (GlobalUtilityLibrary.Helpers.ServiceCallHelper.isTokenRefreshCalled)
                        GlobalUtilityLibrary.Helpers.ServiceCallHelper.isTokenRefreshCalled = false;
                    var read = result.Content.ReadAsStreamAsync();
                    //var finalResult = read.Result;
                    using (var finalResult = read.Result)
                    {
                        var path = tempDirectory + fileName;

                        string tempDir = Path.GetDirectoryName(path);
                        returnableFiledetails.Add(fileName, path);
                        if (!Directory.Exists(tempDir))
                            Directory.CreateDirectory(tempDir);
                        if (!File.Exists(path))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                finalResult.CopyTo(memoryStream);
                                System.IO.File.WriteAllBytes(path, memoryStream.ToArray());
                            }
                        }
                    }
                }
                watch.Stop();
                return returnableFiledetails;
            }
            catch (Exception ex)
            {
                watch.Stop();
                Utils.LogError("Error occured when downloading the files from the server. Message: " + ex.Message + ". StackTrace: " + ex.StackTrace + ". Datetime: " + DateTime.Now);
                //GlobalUtilityLibrary.CustomUserControl.CustomMessageBox.Show("API can not connect to the Sever. Please try again after some time.");
                return returnableFiledetails;
            }
        }

    }

    public class TemplateRepoFrontEndModel : PitchreadyGlobalBaseVM
    {
        public string Description { get; set; }

        public static event EventHandler SendObjectDetailsForPreview;

        public static event EventHandler SendObjectDetailsForCheckUnCheck;

        private bool _isPreviewSelected;
        public bool IsPreviewSelected
        {
            get
            {
                return _isPreviewSelected;
            }
            set
            {
                _isPreviewSelected = value;
                OnPropertyChanged("IsPreviewSelected");
            }
        }



        private RelayCommand _previewSelectedCommand;
        public RelayCommand PreviewSelectedCommand
        {
            get { return _previewSelectedCommand; }
            set
            {
                _previewSelectedCommand = value;
                OnPropertyChanged("PreviewSelectedCommand");
            }
        }

        private RelayCommand _allPreviewsSelectedCommand;
        public RelayCommand AllPreviewsSelectedCommand
        {
            get { return _allPreviewsSelectedCommand; }
            set
            {
                _allPreviewsSelectedCommand = value;
                OnPropertyChanged("AllPreviewsSelectedCommand");
            }
        }

        private bool? _isPreviewAvailable;
        public bool? IsPreviewAvailable
        {
            get
            {
                return _isPreviewAvailable;
            }
            set
            {
                if (value.Value)
                {
                    IconPath = "/PitchreadyGlobal;component/ApplicationResources/Media/eye.png";
                }
                else
                {
                    IconPath = "/PitchreadyGlobal;component/ApplicationResources/Media/eye_disabled.png";
                }
                _isPreviewAvailable = value;
                OnPropertyChanged("IsPreviewAvailable");
            }
        }

        public string FileName { get; set; }

        public DateTime? Date { get; set; }

        public int FileSize { get; set; }

        private string iconPath;
        public string IconPath
        {
            get
            {
                return iconPath;
            }
            set
            {
                iconPath = value;
                OnPropertyChanged("IconPath");
            }
        }

        private RelayCommand _previewClickedCommand;
        public RelayCommand PreviewClickedCommand
        {
            get { return _previewClickedCommand; }
            set
            {
                _previewClickedCommand = value;
                OnPropertyChanged("PreviewClickedCommand");
            }
        }

        public int? TemplateWorkbookId { get; set; }
        public TemplateRepoFrontEndModel()
        {
            Predicate<object> predicate = ExecutePreview;
            PreviewClickedCommand = new RelayCommand(PreviewClickedCommandHandler, predicate);

            AllPreviewsSelectedCommand = new RelayCommand(AllPreviewsSelectedCommandHandler);

            PreviewSelectedCommand = new RelayCommand(PreviewSelectedCommandHandler);
        }

        public void AllPreviewsSelectedCommandHandler(object obj)
        {
            SendObjectDetailsForCheckUnCheck?.Invoke(this, null);
        }

        public void PreviewSelectedCommandHandler(object obj)
        {
            SendObjectDetailsForCheckUnCheck?.Invoke(this, null);
        }

        public bool ExecutePreview(object obj)
        {
            return this._isPreviewAvailable.Value;
        }

        public void PreviewClickedCommandHandler(object obj)
        {
            SendObjectDetailsForPreview?.Invoke(this, null);
        }
    }
}
