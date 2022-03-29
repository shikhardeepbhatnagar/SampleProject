using GlobalUtilityLibrary;
using GlobalUtilityLibrary.Entities;
using GlobalUtilityLibrary.Entities.TemplateRepositoryModals;
using GlobalUtilityLibrary.FactoryMethod;
using GlobalUtilityLibrary.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using PitchreadyGlobal.ViewModels;
using System.Windows.Media.Imaging;
using PitchreadyGlobal.Views;
using PitchreadyGlobal.UserControls;
using PitchreadyGlobal.Enumrations;
using PitchreadyPowerPoint.Enumration;

namespace PitchreadyGlobal.ViewModels
{
    public class TemplateRepositoryAdminViewModel : PitchreadyGlobalBaseVM
    {
        public event EventHandler<Microsoft.Exchange.WebServices.Data.NotificationEventArgs> ExpandAllNodes;
        public bool isRootUpdate = false;
        public static bool isSearchinProgress = false;
        public static int LastSelctionBeforpopup = -1;
        public static bool isValueUpdate = false;
        public static bool istreeDataUpdated = false;

        //public static List<int> ExpandedNodeList = new List<int>();
        public static int lastexpandedNodeIndex = 0;
        #region Private members       
        private RelayCommand _addCategoryCommand;
        private RelayCommand _createFolderCommand;
        private RelayCommand _uploadTemplateCommand;
        private RelayCommand _categorySelectionChangedCommand;
        private RelayCommand _loadCommand;
        private RelayCommand _activatedCommand;
        private RelayCommand _ClearSearchCommand;
        private string currentCriteria = String.Empty;
        private Visibility isSearchedVisibility = Visibility.Collapsed;
        private SolidColorBrush searchForGroundColor = Brushes.Black;
        ParentCategoryModel _selectedCategory = null;

        private int parentCategoryTopId;
        private int parentCategoryBottomId;

        ObservableCollection<ParentCategoryModel> _templateRepositoryList = new ObservableCollection<ParentCategoryModel>();
        ObservableCollection<ParentCategoryModel> _templateCatagoryList = new ObservableCollection<ParentCategoryModel>();
        ObservableCollection<ChildCategoryModel> _templateDataList = new ObservableCollection<ChildCategoryModel>();
        ObservableCollection<ChildCategoryModel> _tempList = new ObservableCollection<ChildCategoryModel>();

        #endregion

        #region RelayCommand properties

        public RelayCommand ClearSearchCommand
        {
            get
            {
                return _ClearSearchCommand;
            }
            set
            {
                _ClearSearchCommand = value;
                OnPropertyChanged("ClearSearchCommand");
            }
        }

        public RelayCommand LoadCommand
        {
            get
            {
                return _loadCommand;
            }
            set
            {
                _loadCommand = value;
                OnPropertyChanged("LoadCommand");
            }
        }


        public RelayCommand ActivatedCommand
        {
            get
            {
                return _activatedCommand;
            }
            set
            {
                _activatedCommand = value;
                OnPropertyChanged("ActivatedCommand");
            }
        }


        public RelayCommand CategorySelectionChangedCommand
        {
            get { return _categorySelectionChangedCommand; }
            set
            {
                _categorySelectionChangedCommand = value;
                OnPropertyChanged("CategorySelectionChangedCommand");
            }
        }
        public RelayCommand AddCategoryCommand
        {
            get { return _addCategoryCommand; }
            set
            {
                _addCategoryCommand = value;
                OnPropertyChanged("AddCategoryCommand");
            }
        }
        public RelayCommand CreateFolderCommand
        {
            get { return _createFolderCommand; }
            set
            {
                _createFolderCommand = value;
                OnPropertyChanged("CreateFolderCommand");
            }
        }
        public RelayCommand UploadTemplateCommand
        {
            get { return _uploadTemplateCommand; }
            set
            {
                _uploadTemplateCommand = value;
                OnPropertyChanged("UploadTemplateCommand");
            }
        }

        #endregion

        #region Collections
        //public ObservableCollection<TemplateRepositoryDetails> Childs { get; set; }
        public ObservableCollection<ChildCategoryModel> TemplateDataList
        {
            get { return _templateDataList; }
            set
            {
                _templateDataList = value;
                OnPropertyChanged("TemplateDataList");
            }
        }
        public ObservableCollection<ChildCategoryModel> TempList
        {
            get { return _tempList; }
            set
            {
                _tempList = value;
                OnPropertyChanged("TempList");
            }
        }
        public ObservableCollection<ParentCategoryModel> TemplateCatagoryList
        {
            get { return _templateCatagoryList; }
            set
            {
                _templateCatagoryList = value;
                OnPropertyChanged("TemplateCatagoryList");
            }
        }

        public ParentCategoryModel SelectedCategorydata
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                OnPropertyChanged("SelectedCategorydata");
            }
        }

        public ObservableCollection<ParentCategoryModel> TemplateRepositoryList
        {
            get { return _templateRepositoryList; }
            set
            {
                _templateRepositoryList = value;
                OnPropertyChanged("TemplateRepositoryList");
            }
        }

        public ObservableCollection<CategoryTemplateMiddlewareVM> _categoryListdata;
        public ObservableCollection<CategoryTemplateMiddlewareVM> categoryListdata
        {
            get { return _categoryListdata; }
            set
            {
                _categoryListdata = value;
                isEnable = _categoryListdata != null;
                OnPropertyChanged("categoryListdata");
            }
        }

        public bool _isEnable = false;
        public bool isEnable
        {
            get { return _isEnable; }
            set
            {
                _isEnable = value;
                OnPropertyChanged("isEnable");
            }
        }

        public int _lstSelectedIdx = -1;
        public int LstSelectedIdx
        {
            get { return _lstSelectedIdx; }
            set
            {
                _lstSelectedIdx = value;
                OnPropertyChanged("LstSelectedIdx");
            }
        }



        #endregion

        #region Public properties

        #endregion

        #region Contructor
        public TemplateRepositoryAdminViewModel()
        {
            isSearchinProgress = false;
            LastSelctionBeforpopup = -1; AddCategoryCommand = new RelayCommand(AddCategoryCommandHandler);
            isValueUpdate = false; CreateFolderCommand = new RelayCommand(CreateFolderCommandHandler);
            istreeDataUpdated = false;
            UploadTemplateCommand = new RelayCommand(UploadTemplateCommandHandler);
            lastexpandedNodeIndex = 0;
            CategorySelectionChangedCommand = new RelayCommand(CategorySelectionChangedCommandHandler);
            LoadCommand = new RelayCommand(LoadCommandHandler);
            ActivatedCommand = new RelayCommand(FormActivatedHandler);
            //EscCommand = new RelayCommand(EscCommandHandler);
            ClearSearchCommand = new RelayCommand(ClearSearchCommandhandler);
        }

        #endregion

        #region Private methods
        private void Getdata(ParentCategoryModel nodes, List<ParentCategoryModel> objCategoryDetails)
        {
            try
            {
                nodes.TemplateChildlist = TemplateDataList.Where(o => Convert.ToInt32(o.CategoryID) == nodes.Id).ToList();
                nodes.ChildCount = (TemplateDataList.Where(o => Convert.ToInt32(o.CategoryID) == nodes.Id)).Count();
                IncreaseParentCount(nodes, nodes.ChildCount);
                if (nodes.Childs == null)
                    nodes.Childs = new System.Collections.ObjectModel.ObservableCollection<ParentCategoryModel>();
                var RefinedChilds = objCategoryDetails.Where(o => o.CategoryParentId == nodes.Id);
                foreach (var child in RefinedChilds)
                {
                    child.ParentNode = nodes;
                    nodes.Childs.Add(child);
                    Getdata(child, objCategoryDetails);
                }
                nodes.CategoryChildCount = nodes.Childs.Count().ToString();
            }
            catch
            {

            }
        }

        private void populateCustomTreedata(ParentCategoryModel item, List<CategoryTemplateMiddlewareVM> categoryTemplateMiddlewareVM, List<ParentCategoryModel> categoryDetails, CategoryTemplateMiddlewareVM childNodemiddleware = null)
        {
            if (childNodemiddleware == null)
            {
                childNodemiddleware = new CategoryTemplateMiddlewareVM();
                childNodemiddleware.LoaderEvent += Item_LoaderEvent;
                categoryTemplateMiddlewareVM.Add(childNodemiddleware);
            }

            childNodemiddleware.Categorydata = item;
            foreach (var Categoryitem in item.Childs)
            {
                if (childNodemiddleware.Childs == null)
                    childNodemiddleware.Childs = new ObservableCollection<CategoryTemplateMiddlewareVM>();
                var objmiddle = new CategoryTemplateMiddlewareVM();
                objmiddle.LoaderEvent += Item_LoaderEvent;
                objmiddle.ParentNode = childNodemiddleware;
                childNodemiddleware.Childs.Add(objmiddle);
                populateCustomTreedata(Categoryitem, categoryTemplateMiddlewareVM, categoryDetails, objmiddle);
            }
            var TemplateChildlist = TemplateDataList.Where(o => Convert.ToInt32(o.CategoryID) == item.Id).ToList();
            if (TemplateChildlist.Count > 0)
                foreach (var Objtemplate in TemplateChildlist)
                {
                    if (childNodemiddleware.Childs == null)
                        childNodemiddleware.Childs = new ObservableCollection<CategoryTemplateMiddlewareVM>();
                    var objmiddle = new CategoryTemplateMiddlewareVM();
                    objmiddle.LoaderEvent += Item_LoaderEvent;
                    objmiddle.templatedata = Objtemplate;
                    objmiddle.ParentNode = childNodemiddleware;

                    childNodemiddleware.Childs.Add(objmiddle);
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
            catch
            {

            }
        }
        private async Task GetParentCatagories()
        {
            try
            {
                IsLoading = true;
                var result = await GlobalUtilityLibrary.Helpers.ServiceCallHelper.GetData<List<ParentCategoryModel>>("pitchready/TemplateRepositoryExcel/GetAllParentCatagories", GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);

                TemplateRepositoryList = new ObservableCollection<ParentCategoryModel>(result);

                if (TemplateRepositoryList != null)
                {
                    if (TemplateRepositoryList == null || TemplateRepositoryList.Count == 0)
                    {
                        IsLoading = false;
                        return;
                    }
                    var fileNames = new List<string>();
                    foreach (var item in TemplateRepositoryList)
                    {
                        item.LoaderEvent += Item_LoaderEvent;
                        if (!File.Exists(GlobalUtility.GetToolTempPath(GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository) + item.CategoryIconName))
                            fileNames.Add(item.CategoryIconName);
                        else
                            item.CategoryIconName = GlobalUtility.GetToolTempPath(GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository) + item.CategoryIconName;
                    }
                    var files = new BaseModal<Dictionary<string, string>>();
                    if (fileNames.Count > 0)
                    {
                        var cancellationToken = new CancellationTokenSource();
                        files = await ServiceCallHelper.DownloadFileAsync("pitchready/TemplateRepositoryExcel/getCompressedfiles", fileNames, GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository, cancellationToken.Token);
                    }
                    if (files != null && files.inputData != null)
                    {
                        if (files.inputData.Count > 0)
                        {
                            foreach (var item in files.inputData)
                            {
                                if (TemplateRepositoryList.FirstOrDefault(o => o.CategoryIconName == item.Key) != null)
                                    TemplateRepositoryList.FirstOrDefault(o => o.CategoryIconName == item.Key).CategoryIconName = item.Value;
                            }
                        }
                    }
                }
                IsLoading = false;
            }
            catch (Exception ex)
            {
                IsLoading = false;
                GlobalUtility.LogError("Exception inside FillClient() : " + ex.Message + ", " + ex.InnerException);
            }

        }
        private async Task GetCategoriesAndTemplates(string Id)
        {
            TemplateDataList.Clear();
            TemplateCatagoryList.Clear();
            Dictionary<string, string> Params = new Dictionary<string, string>();
            Params.Add("CategoryID", Id);
            var TrData = await ServiceCallHelper.GetDataByParameter<ObservableCollection<ChildCategoryModel>>("pitchready/TemplateRepositoryExcel/GetAllChildCatagories", Params, GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);

            if (TrData != null)
            {
                string jsondata = Newtonsoft.Json.JsonConvert.SerializeObject(TrData);
                TemplateDataList = new ObservableCollection<ChildCategoryModel>(JsonConvert.DeserializeObject<List<ChildCategoryModel>>(jsondata).Where(o => o.TemplateWorkbookId != null));
                TemplateCatagoryList = new ObservableCollection<ParentCategoryModel>(JsonConvert.DeserializeObject<List<ParentCategoryModel>>(jsondata).Distinct(new TemplateRepositoryDetailsComparer()));
            }
        }
        #endregion

        #region CommandHandler
        private async void LoadCommandHandler(object obj)
        {
            //IsLoading = false;
            LoadingText = "Loading Category...";
            await GetParentCatagories();
            // IsLoading = Visibility.Collapsed;

            //if (SelectedCategorydata == null)
            //{
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    Thread.Sleep(200);
                    if (TemplateRepositoryList.Count > 0)
                    {
                        if (LastSelctionBeforpopup > -1)
                            LstSelectedIdx = LastSelctionBeforpopup;
                        else
                            LstSelectedIdx = 0;
                    }
                }
                catch (Exception ex)
                {
                    GlobalUtility.LogError("Exception inside FillClient() : " + ex.Message + ", " + ex.InnerException);
                }
            });
            //}
        }

        public async void UpdateCategoryTRAdmin(int catId, bool status, string catName, string catIcon)
        {
            IsLoading = false;
            LoadingText = "Updating Category...";
            await System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
            });
            var CategoryData = new ParentCategoryModel();

            CategoryData.Id = catId;
            CategoryData.Status = status;
            CategoryData.CategoryName = catName;
            CategoryData.CategoryIconName = Path.GetFileName(catIcon);
            List<FileInfo> fileInfo = new List<FileInfo>();

            var result = await ServiceCallHelper.DMLSaveUpdateData<ParentCategoryModel>(
                       "pitchready/TemplateRepositoryExcel/DMLOperationWithoutFiles", CategoryData,
                       GlobalUtilityLibrary.Enumrations.CRUDType.Update, GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);
            if (result != null)
            {
                ////TemplateRepositoryAdminViewModel.isValueUpdate = true;
                //if (result.SatusCode > 0)
                //{
                //    CustomMessageBoxExcel.Show("Category updated successfully.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                //}
                //else
                //{
                //    CustomMessageBoxExcel.Show("Template/Category data has been modified, Please reload the form and try again.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Warning);
                //}
            }
            else
            {
                LoadingText = "Loading...";
                IsLoading = false;
                CustomMessageBoxExcel.Show("Unable to update catagory status. Please try again.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Warning);
                return;
            }
            //Close();
            //RequestClose += AddCategoryViewModel_RequestClose;
        }

        //To Review
        private async void FormActivatedHandler(object obj)
        {
            GlobalUtility.SetResourcesForTool(GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);
            if (isValueUpdate)
            {
                isValueUpdate = false;
                isRootUpdate = true;
                await GetParentCatagories();
                if (SelectedCategorydata != null && TemplateRepositoryList.FirstOrDefault(o => o.Id == SelectedCategorydata.Id) == null)
                {
                    SelectedCategorydata = null;
                    categoryListdata = new ObservableCollection<CategoryTemplateMiddlewareVM>();
                    isEnable = false;
                }
            }

            if (istreeDataUpdated)
            {
                if (SelectedCategorydata != null && TemplateRepositoryList.FirstOrDefault(o => o.Id == SelectedCategorydata.Id) != null)
                    CategorySelectionChangedCommandHandler(SelectedCategorydata);
                else
                {
                    var Id = TemplateRepositoryList.Min(x => x.Id);
                    CategorySelectionChangedCommandHandler(TemplateRepositoryList.Where(x => x.Id == Id).FirstOrDefault());
                    istreeDataUpdated = false;
                }
            }

        }

        private async void CategorySelectionChangedCommandHandler(object obj)
        {
            LoadingText = "Loading Folder...";
            try
            {
                if (obj == null)
                {
                    if (SelectedCategorydata == null && categoryListdata.Count == 0)
                        isEnable = false;
                    return;
                }
                IsLoading = true;

                SelectedCategorydata = (obj as ParentCategoryModel);
                
                if (TemplateRepositoryList.IndexOf(SelectedCategorydata) > -1)
                    LastSelctionBeforpopup = TemplateRepositoryList.IndexOf(SelectedCategorydata);
                if (!isSearchinProgress)
                    CurrentCriteria = string.Empty;
                System.Threading.Thread.Sleep(200);
                //if (categoryListdata != null)
                //    categoryListdata.Clear();
                //else
                //    categoryListdata = new ObservableCollection<CategoryTemplateMiddlewareVM>();
                await GetCategoriesAndTemplates(SelectedCategorydata.Id.ToString());

                foreach (var item in TemplateRepositoryList.Where(o => o.IsCategorySelected == Visibility.Visible))
                {
                    item.IsCategorySelected = Visibility.Collapsed;
                }
                if (TemplateRepositoryList.FirstOrDefault(o => o.Id == SelectedCategorydata.Id) != null)
                {
                    TemplateRepositoryList.FirstOrDefault(o => o.Id == SelectedCategorydata.Id).IsCategorySelected = Visibility.Visible;
                }
                var SelectedCategory = SelectedCategorydata.Id;
                TemplateCatagoryList.Add(SelectedCategorydata);
                var ObjCategoryDetails = CommonMethods.CreateCopy(TemplateCatagoryList.ToList());
                var CategoryDetails = ObjCategoryDetails.Where(o => o.Id == SelectedCategory).ToList();
                List<CategoryTemplateMiddlewareVM> categoryTemplateMiddlewareVM = new List<CategoryTemplateMiddlewareVM>();
                foreach (var item in CategoryDetails)
                {
                    Getdata(item, ObjCategoryDetails);
                }
                foreach (var item in CategoryDetails)
                {
                    populateCustomTreedata(item, categoryTemplateMiddlewareVM, CategoryDetails);
                }
                categoryListdata = new ObservableCollection<CategoryTemplateMiddlewareVM>();
                foreach (var item in categoryTemplateMiddlewareVM[0].Childs)
                {
                    item.LoaderEvent += Item_LoaderEvent;
                    categoryListdata.Add(item);
                }
                if (istreeDataUpdated)
                {
                    istreeDataUpdated = false;
                    if (lastexpandedNodeIndex > 0)
                    {
                        foreach (var item in categoryTemplateMiddlewareVM)
                        {
                            expandlastnode(item);
                        }
                        lastexpandedNodeIndex = 0;
                    }
                }
                IsLoading = false;
            }
            catch (Exception ex)
            {
                istreeDataUpdated = false;
                IsLoading = false;
                GlobalUtility.LogError("Exception inside FillClient() : " + ex.Message + ", " + ex.InnerException);
            }
        }

        private void expandlastnode(CategoryTemplateMiddlewareVM categoryTemplateMiddlewareVM)
        {
            if (categoryTemplateMiddlewareVM.IsCategory == Visibility.Visible)
            {
                if (categoryTemplateMiddlewareVM.Categorydata.Id == lastexpandedNodeIndex)
                {
                    lastexpandedNodeIndex = 0;
                    if (categoryTemplateMiddlewareVM.IsCategory == Visibility.Visible)
                        categoryTemplateMiddlewareVM.IsNodeExpanded = true;
                    ExpandParent(categoryTemplateMiddlewareVM);
                }
                if (lastexpandedNodeIndex > 0)
                {
                    foreach (var item in categoryTemplateMiddlewareVM.Childs)
                    {
                        expandlastnode(item);
                    }
                }
            }
        }

        private void ExpandParent(CategoryTemplateMiddlewareVM categoryTemplateMiddlewareVM)
        {
            if (categoryTemplateMiddlewareVM.ParentNode != null)
            {
                Debug.WriteLine("category ID : " + categoryTemplateMiddlewareVM.ParentNode.Categorydata.Id);
                if (categoryTemplateMiddlewareVM.ParentNode.IsCategory == Visibility.Visible)
                    categoryTemplateMiddlewareVM.ParentNode.IsNodeExpanded = true;
                ExpandParent(categoryTemplateMiddlewareVM.ParentNode);
            }
        }

        private bool isFiltered = false;
        private void ApplyFilter()
        {
            if (string.IsNullOrEmpty(currentCriteria))
            {
                CategorySelectionChangedCommandHandler(SelectedCategorydata);
                searchForGroundColor = Brushes.Black;
                IsSearchedVisibility = Visibility.Collapsed;
                return;
            }
            isFiltered = false;
            var ObjCategoryDetails = CommonMethods.CreateCopy(TemplateCatagoryList.ToList());
            var SelectedCategory = SelectedCategorydata.Id;
            var CategoryDetails = ObjCategoryDetails.Where(o => o.Id == SelectedCategory).ToList();
            List<CategoryTemplateMiddlewareVM> categoryTemplateMiddlewareVM = new List<CategoryTemplateMiddlewareVM>();
            foreach (var item in CategoryDetails)
            {
                Getdata(item, ObjCategoryDetails);
            }
            foreach (var item in CategoryDetails)
            {
                populateCustomTreedata(item, categoryTemplateMiddlewareVM, CategoryDetails);
            }
            FilterCollection(categoryTemplateMiddlewareVM.FirstOrDefault(), CurrentCriteria);
            DeleteNodes(categoryTemplateMiddlewareVM.FirstOrDefault());
            if (isFiltered)
            {
                categoryListdata.Clear();
                foreach (var item in categoryTemplateMiddlewareVM[0].Childs)
                {
                    item.LoaderEvent += Item_LoaderEvent;
                    categoryListdata.Add(item);
                }
                searchForGroundColor = Brushes.Black;
                IsSearchedVisibility = Visibility.Collapsed;
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    ExpandAllNodes(null, null);
                });
            }
            else
            {
                CategorySelectionChangedCommandHandler(SelectedCategorydata);
                searchForGroundColor = Brushes.Red;
                IsSearchedVisibility = Visibility.Visible;
            }


            // CommandManager.InvalidateRequerySuggested();
        }

        private void Item_LoaderEvent(object sender, Microsoft.Exchange.WebServices.Data.NotificationEventArgs e)
        {
            bool? ObjData = sender as bool?;
            if (ObjData != null)
                IsLoading = (bool)ObjData;
        }

        private void FilterCollection(CategoryTemplateMiddlewareVM filterList, string text)
        {
            if (filterList.IsCategory == Visibility.Visible)
            {
                if (!filterList.Categorydata.CategoryName.ToLower().Contains(text.ToLower()))
                    filterList.isDeleted = true;
                else
                    isFiltered = true;
            }
            else
            {
                if (!filterList.templatedata.TemplateName.ToLower().Contains(text.ToLower()))
                    filterList.isDeleted = true;
                else
                    isFiltered = true;
            }

            //if (filterList.isCategory == Visibility.Visible && filterList.Childs.Count > 0)
            //    filterList.IsExpanded = !filterList.isDeleted;

            foreach (var item in filterList.Childs)
            {
                if (item.IsCategory == Visibility.Visible)
                {
                    if (!item.Categorydata.CategoryName.ToLower().Contains(text.ToLower()))
                    {
                        item.isDeleted = true;
                    }
                    else
                    {
                        isFiltered = true;
                        item.ParentNode.isDeleted = false;
                        retainparent(item);
                    }
                }
                else
                {
                    if (!item.templatedata.TemplateName.ToLower().Contains(text.ToLower()))
                    {
                        item.isDeleted = true;
                    }
                    else
                    {
                        isFiltered = true;
                        item.ParentNode.isDeleted = false;
                        retainparent(item);
                    }
                }
                FilterCollection(item, text);
            }
        }

        private void retainparent(CategoryTemplateMiddlewareVM item)
        {
            if (item.ParentNode != null)
            {
                item.ParentNode.isDeleted = false;
                if (item.ParentNode.IsCategory == Visibility.Visible && item.ParentNode.Childs.Count > 0)
                    item.ParentNode.IsNodeExpanded = true;
                retainparent(item.ParentNode);
            }
        }

        private void DeleteNodes(CategoryTemplateMiddlewareVM filteredList)
        {
            if (filteredList.isDeleted)
            {
                if (filteredList.ParentNode != null)
                    filteredList.ParentNode.Childs.Remove(filteredList);
            }
            else
            {
                CategoryTemplateMiddlewareVM[] array = new CategoryTemplateMiddlewareVM[filteredList.Childs.Count()];
                filteredList.Childs.ToList().CopyTo(array);
                foreach (var item in array)
                {
                    DeleteNodes(item);
                }
            }
        }

        private void ClearSearchCommandhandler(object obj)
        {
            CurrentCriteria = string.Empty;
        }

        private void UploadTemplateCommandHandler(object obj)
        {
            //UploadTemplate _uploadTemplate = new UploadTemplate();
            //_uploadTemplate.ShowDialog();
        }
        private void CreateFolderCommandHandler(object obj)
        {
            var maxID = 0;
            if (maxID != 0)
                maxID = TemplateCatagoryList.Max(x => (int)x.Id);
            AddEditWindowViewModel addEditWindowViewModel = new AddEditWindowViewModel(obj as ParentCategoryModel)
            {
                PopUpTitle = "Create Sub-category",
                IsNewFolder = true,
                IsNewCategory = false,
                IsArtefactLabelGridVisible = false,
                IsNameLabelGridVisible = true,
                AddEditButtonName = "Create",
                LabelName = "Name",
                ParentId = SelectedCategorydata?.Id,
                ActionName = "InsertFolder"
            };
            AddEditWindowView createFolder = new AddEditWindowView(addEditWindowViewModel);
            createFolder.ShowDialog();
        }
        private void AddCategoryCommandHandler(object obj)
        {
            var maxID = 0;
            if (maxID != 0)
                maxID = TemplateCatagoryList.Max(x => (int)x.Id);
            AddEditWindowViewModel addEditWindowViewModel = new AddEditWindowViewModel(obj as ParentCategoryModel)
            {
                PopUpTitle = "Add Category",
                IsNewFolder = false,
                IsNewCategory = true,
                IsArtefactLabelGridVisible = true,
                IsNameLabelGridVisible = true,
                AddEditButtonName = "Add",
                LabelName = "Category Name",
                ArtefactLabelName = "Upload Logo",
                ActionName = "InsertCategory"
            };
            AddEditWindowView _addCategory = new AddEditWindowView(addEditWindowViewModel);
            _addCategory.ShowDialog();
            if ((bool)_addCategory.DialogResult)
                LoadCommandHandler(null);
        }

        DispatcherTimer timer = new DispatcherTimer();
        public string CurrentCriteria
        {
            get { return currentCriteria; }
            set
            {
                if (value == currentCriteria)
                    return;

                currentCriteria = value;
                //if (string.IsNullOrEmpty(currentCriteria))
                //{
                //    CategorySelectionChangedCommandHandler(SelectedCategorydata);
                //}
                //IsLoading = Visibility.Collapsed;
                timer.Stop();
                timer.Tick -= Timer_Tick;
                timer = new DispatcherTimer();
                timer.Tick += Timer_Tick;
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Start();
                OnPropertyChanged("CurrentCriteria");
            }
        }

        public Visibility IsSearchedVisibility
        {
            get
            {
                return isSearchedVisibility;
            }

            set
            {
                isSearchedVisibility = value;
                OnPropertyChanged("IsSearchedVisibility");
            }
        }

        public SolidColorBrush SearchForGroundColor
        {
            get
            {
                return searchForGroundColor;
            }

            set
            {
                searchForGroundColor = value;
                OnPropertyChanged("SearchForGroundColor");
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            isSearchinProgress = true;
            ApplyFilter();
            isSearchinProgress = false;
        }
        #endregion
    }

    public class TemplateRepositoryDetailsComparer : IEqualityComparer<ParentCategoryModel>
    {
        // Products are equal if their names and product numbers are equal.
        public bool Equals(ParentCategoryModel x, ParentCategoryModel y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return x.Id == y.Id;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(ParentCategoryModel product)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(product, null)) return 0;
            //Get hash code for the Code field.
            int hashProductCode = product.Id.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductCode;
        }
    }

    public class ParentCategoryModel : PitchreadyGlobalBaseVM
    {
        public event EventHandler<Microsoft.Exchange.WebServices.Data.NotificationEventArgs> LoaderEvent;

        public static event EventHandler SendParentCategoryToFrontEndWindow;

        [JsonProperty("SatusCode")]
        public int SatusCode { get; set; }

        [JsonProperty("Message")]
        public ErrorMessage Message { get; set; }

        [JsonProperty("CategoryID")]
        public int? Id { get; set; }

        private string _categoryIconName;

        [JsonProperty("Id")]
        public int? SystemId
        {
            set
            {
                Id = value;
            }
        }

        public string CategoryName { get; set; }
        public string CategoryIconName
        {
            get
            {
                return _categoryIconName;
            }
            set
            {
                _categoryIconName = value;
                OnPropertyChanged("CategoryIconName");
            }
        }

        [JsonIgnore]
        public bool isDeleted { get; set; }

        [JsonIgnore]
        private ImageSource _nodeImage = new BitmapImage(new Uri("/PitchreadyPowerPoint;component/Media/Icons/FolderImg.png", UriKind.Relative));

        [JsonIgnore]
        public ImageSource nodeImage
        {
            get
            {
                return _nodeImage;
            }
            set
            {
                if (_nodeImage == value) return;
                _nodeImage = value;
                OnPropertyChanged("nodeImage");
            }
        }

        [JsonIgnore]
        private System.Windows.Automation.ToggleState _SelectionState = System.Windows.Automation.ToggleState.Off;

        [JsonIgnore]
        public System.Windows.Automation.ToggleState SelectionState
        {
            get
            {
                return _SelectionState;
            }
            set
            {
                _SelectionState = value;
                OnPropertyChanged("SelectionState");
            }
        }

        public int? CategoryParentId { get; set; }

        public string StatementType { get; set; }
        public RelayCommand EditCategoryCommand { get; set; }
        public RelayCommand DeleteCategoryCommand { get; set; }
        public List<ChildCategoryModel> TemplateChildlist { get; set; }
        public ParentCategoryModel ParentNode { get; set; }
        public ObservableCollection<ParentCategoryModel> Childs { get; set; }

        [JsonProperty("UpdatedAtCategory")]
        public DateTime? UpdatedAtCategory { get; set; }

        private Visibility _isCategorySelected = Visibility.Collapsed;

        public string _categoryChildCount;
        public string CategoryChildCount
        {
            get
            {
                return _categoryChildCount;
            }
            set
            {
                int data = 0;
                if (int.TryParse(value, out data))
                {
                    data = int.Parse(value);
                    if (data > 0)
                        _categoryChildCount = "(" + data.ToString("00") + ")";
                    else
                        _categoryChildCount = string.Empty;
                }
                OnPropertyChanged("CategoryChildCount");
            }
        }
        public int _clildCount = 0;

        public int ChildCount
        {
            get
            {
                return _clildCount;
            }
            set
            {
                if (_clildCount == value) return;
                _clildCount = value;
                OnPropertyChanged("ChildCount");
            }
        }
        public Visibility IsCategorySelected
        {
            get
            {
                return _isCategorySelected;
            }
            set
            {
                _isCategorySelected = value;
                CategorySelctionColor = _isCategorySelected == Visibility.Visible ? new SolidColorBrush(Color.FromRgb(33, 115, 69)) : Brushes.Transparent;
                OnPropertyChanged("IsCategorySelected");
            }
        }

        private Brush _categorySelctionColor = Brushes.Transparent;

        public Brush CategorySelctionColor
        {
            get
            {
                return _categorySelctionColor;
            }
            set
            {
                _categorySelctionColor = value;
                OnPropertyChanged("CategorySelctionColor");
            }
        }

        private bool _status;
        public bool Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        private bool _frontEndRootCategorySelected;

        public bool FrontEndRootCategorySelected
        {
            get
            {
                return _frontEndRootCategorySelected;
            }
            set
            {
                _frontEndRootCategorySelected = value;
                OnPropertyChanged("FrontEndRootCategorySelected");
            }
        }

        private RelayCommand _checkChangeCommand;
        public RelayCommand CheckChangeCommandFrontEnd
        {
            get { return _checkChangeCommand; }
            set
            {
                _checkChangeCommand = value;
                OnPropertyChanged("CheckChangeCommandFrontEnd");
            }
        }

        public ParentCategoryModel()
        {
            EditCategoryCommand = new RelayCommand(param => EditCategoryCommandHandler(param));
            CheckChangeCommandFrontEnd = new RelayCommand(CheckChangeCommandHandlerFrontEnd);
            DeleteCategoryCommand = new RelayCommand(param => DeleteCategoryCommandHandler(param));
        }

        public void CheckChangeCommandHandlerFrontEnd(object obj)
        {
            SendParentCategoryToFrontEndWindow?.Invoke(this, null);
        }

        public void EditCategoryCommandHandler(object obj)
        {
            AddEditWindowViewModel addEditWindowViewModel = new AddEditWindowViewModel(obj as ParentCategoryModel)
            {
                PopUpTitle = "Edit Category",
                IsNewFolder = false,
                IsNewCategory = false,
                IsArtefactLabelGridVisible = true,
                IsNameLabelGridVisible = true,
                AddEditButtonName = "Update",
                LabelName = "Category Name",
                ArtefactLabelName = "Upload Logo",
                ArtefactPath = Path.GetFileName((obj as ParentCategoryModel).CategoryIconName),
                TextBoxName = (obj as ParentCategoryModel).CategoryName,
                CategoryId = (obj as ParentCategoryModel).Id,
                ParentId = (obj as ParentCategoryModel).CategoryParentId,
                CategoryName = (obj as ParentCategoryModel).CategoryName,
                ActionName = "UpdateCategory",
                previousArtifactName = Path.GetFileName((obj as ParentCategoryModel).CategoryIconName),
                previousTextboxName = (obj as ParentCategoryModel).CategoryName,
            };
            AddEditWindowView _editCategory = new AddEditWindowView(addEditWindowViewModel);
            _editCategory.ShowDialog();
        }

        private async void DeleteCategoryCommandHandler(object obj)
        {
            try
            {
                var dialogResult = CustomMessageBoxExcel.Show("Do you want to delete category ?", "Delete Category", ExcelMessageBoxType.ConfirmationWithYesNo, ExcelMessageBoxImage.Question);
                List<ChildCategoryModel> templateListData = null;
                if (dialogResult)
                {
                    LoaderEvent(true, null);
                    bool isDeleted = false;
                    var dialogResult2 = new GenericListDialog(this, new List<ChildCategoryModel>());
                    Dictionary<string, string> Params = new Dictionary<string, string>();
                    Params.Add("CategoryID", this.Id.ToString());
                    var TrData = await ServiceCallHelper.GetDataByParameter<ObservableCollection<ChildCategoryModel>>("pitchready/TemplateRepositoryExcel/GetAllChildCatagories", Params, GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);
                    if (TrData != null)
                    {
                        string jsondata = Newtonsoft.Json.JsonConvert.SerializeObject(TrData);
                        templateListData = JsonConvert.DeserializeObject<List<ChildCategoryModel>>(jsondata).Where(o => o.TemplateWorkbookId != null).ToList();
                        if (templateListData.Count == 0)
                        {
                            isDeleted = true;
                        }
                        else
                        {
                            dialogResult2 = new GenericListDialog(this, templateListData);
                            dialogResult2.ShowDialog();
                            isDeleted = dialogResult2.DialogResult == null ? false : (bool)dialogResult2.DialogResult;
                        }
                    }
                    else
                    {
                        LoaderEvent(false, null);
                        return;
                    }

                    if (isDeleted)
                    {
                        // To collect worksheetname, workbookname and other artefacts for deletion
                        List<string> fileNames = new List<string>();
                        fileNames.Add(Path.GetFileName(this.CategoryIconName));
                        if (templateListData != null && templateListData.Any())
                        {
                            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                            foreach (var item in templateListData)
                            {
                                fileNames.Add(item.SystemFileName);
                                keyValuePairs.Clear();
                                keyValuePairs.Add("TemplateWorkbookId", item.TemplateWorkbookId.ToString());
                                var collectionData = await ServiceCallHelper.GetDataByParameter<ObservableCollection<WorksheetInfoModel>>("pitchready/TemplateRepositoryExcel/GetWorksheetInfo", keyValuePairs, GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);
                                if (collectionData != null)
                                {
                                    foreach (var data in collectionData)
                                    {
                                        fileNames.Add(data.SystemWorksheetName);
                                    }
                                }
                            }
                        }

                        string TemplateRepositoryPath = GlobalUtility.GetTemplateRepositoryPath();
                        var parametersToUpload = new ParentCategoryModel
                        {
                            Id = this.Id,
                            CategoryName = this.CategoryName,
                            CategoryParentId = this.CategoryParentId,
                        };
                        var result = await ServiceCallHelper.DMLSaveUpdateData<ParentCategoryModel>("pitchready/TemplateRepositoryExcel/DMLOperationWithoutFiles", parametersToUpload, GlobalUtilityLibrary.Enumrations.CRUDType.Delete, GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);
                        if (result != null)
                        {
                            if (result.SatusCode > 0)
                            {
                                await ServiceCallHelper.DMLSaveUpdateData<List<string>>("pitchready/TemplateRepositoryExcel/DeleteArtefact", fileNames, GlobalUtilityLibrary.Enumrations.CRUDType.Delete, GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);
                                TemplateRepositoryAdminViewModel.isValueUpdate = true;
                                TemplateRepositoryAdminViewModel.istreeDataUpdated = true;

                                CustomMessageBoxExcel.Show("Category deleted successfully.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                            }
                            else
                            {
                                CustomMessageBoxExcel.Show("Template/Category data has not been modified, Please reload the form and try again.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            CustomMessageBoxExcel.Show("Unable to delete record. Please try again.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Warning);
                        }
                    }
                    LoaderEvent(false, null);
                }
            }
            catch (Exception ex)
            {
                LoaderEvent(false, null);
            }
        }
    }

    public class ChildCategoryModel : PitchreadyGlobalBaseVM
    {
        public static event EventHandler SendPreviewDataToCart;
        public static event EventHandler SendDeleteDataToCart;

        public int? CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int? CategoryParentId { get; set; }
        public DateTime? UpdatedAtCategory { get; set; }
        public int? TemplateWorkbookId { get; set; }
        public string TemplateName { get; set; }
        public string OrigFileName { get; set; }
        public string SystemFileName { get; set; }
        public string Description { get; set; }
        public int? FileSizeInKB { get; set; }
        public int? WorksheetCount { get; set; }
        public bool? IsPreviewAvailable { get; set; }
        public DateTime? UpdatedAtTemplate { get; set; }
        public int SatusCode { get; set; }
        public object Convert { get; set; }

        private string _trimTemplateName;
        public string TrimTemplateName
        {
            get
            {
                if (TemplateName != null)
                {
                    if (TemplateName.Length > 15)
                    {
                        _trimTemplateName = TemplateName.Substring(0, 14) + "...";
                    }
                    else
                    {
                        _trimTemplateName = TemplateName;
                    }
                    return _trimTemplateName;
                }
                return _trimTemplateName;
            }
            set
            {
                _trimTemplateName = value;
                OnPropertyChanged("TrimTemplateName");
            }
        }

        private RelayCommand _deleteFromCartCommand;
        public RelayCommand DeleteFromCartCommand
        {
            get
            {
                return _deleteFromCartCommand;
            }
            set
            {
                _deleteFromCartCommand = value;
                OnPropertyChanged("DeleteFromCartCommand");
            }
        }

        private RelayCommand _previewFromCartCommand;
        public RelayCommand PreviewFromCartCommand
        {
            get
            {
                return _previewFromCartCommand;
            }
            set
            {
                _previewFromCartCommand = value;
                OnPropertyChanged("PreviewFromCartCommand");
            }
        }

        private bool? _isPreviewPresent;
        public bool? IsPreviewPresent
        {
            get
            {
                if (IsPreviewAvailable != null && IsPreviewAvailable.Value)
                {
                    IconPath = "/PitchreadyGlobal;component/ApplicationResources/Media/eye.png";
                    _isPreviewPresent = true;
                }
                else
                {
                    IconPath = "/PitchreadyGlobal;component/ApplicationResources/Media/eye.png";
                    _isPreviewPresent = false;
                }
                return _isPreviewPresent;
            }
            set
            {
                _isPreviewPresent = value;
                OnPropertyChanged("IsPreviewPresent");
            }
        }

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

        public ChildCategoryModel()
        {
            DeleteFromCartCommand = new RelayCommand(DeleteFromCartCommandHandler);
            PreviewFromCartCommand = new RelayCommand(PreviewFromCartCommandHandler);
        }

        public void PreviewFromCartCommandHandler(object obj)
        {
            SendPreviewDataToCart?.Invoke(this, null);
        }

        public void DeleteFromCartCommandHandler(object obj)
        {
            SendDeleteDataToCart?.Invoke(this, null);
        }
    }

    public class ChildCategoryRequestModel
    {
        public int? CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int? CategoryParentId { get; set; }
        public DateTime? UpdatedAtCategory { get; set; }
        public int? TemplateWorkbookId { get; set; }
        public string TemplateName { get; set; }
        public string OrigFileName { get; set; }
        public string SystemFileName { get; set; }
        public string Description { get; set; }
        public int? FileSizeInKB { get; set; }
        public int? WorksheetCount { get; set; }
        public bool? IsPreviewAvailable { get; set; }
        public DateTime? UpdatedAtTemplate { get; set; }
        public DateTime? InsertedAt { get; set; }
        public string InsertedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsWorksheetInfoNeedToUpdate { get; set; }
    }


    public class CategoryRequestModel
    {
        public int? Id { get; set; }
        public string CategoryName { get; set; }
        public string CategoryIconName { get; set; }
        public int? CategoryParentId { get; set; }
        public bool Status { get; set; }
        [JsonProperty("SatusCode")]
        public int SatusCode { get; set; }

        [JsonProperty("Message")]
        public ErrorMessage Message { get; set; }
    }


    public class WorksheetInfoModel
    {
        public int? TemplateWorkbookId { get; set; }

        public string WorksheetName { get; set; }

        public string SystemWorksheetName { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}