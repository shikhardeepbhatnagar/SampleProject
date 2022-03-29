using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PitchreadyGlobal.ViewModels;
using PitchreadyGlobal.Helpers;
using PitchreadyGlobal.Enumrations;
using GlobalUtilityLibrary.Helpers;
using RelayCommand = PitchreadyGlobal.Helpers.RelayCommand;
using GlobalUtilityLibrary;
using GlobalUtilityLibrary.Entities;
using PitchreadyGlobal.Views;
using PitchreadyGlobal.UserControls;
using PitchreadyGlobal.DependencyServices;

namespace PitchreadyGlobal.ViewModels
{
    public class CategoryTemplateMiddlewareVM : PitchreadyGlobalBaseVM
    {
        public event EventHandler<Microsoft.Exchange.WebServices.Data.NotificationEventArgs> LoaderEvent;
        public RelayCommand EditItemCommand { get; set; }
        public RelayCommand DeleteItemCommand { get; set; }
        public RelayCommand UploadItemCommand { get; set; }
        public RelayCommand CreateFolderCommand { get; set; }
        public RelayCommand DownloadItemCommand { get; set; }

        private double _progressStatus = 0.0;
        public double progressStatus
        {
            get { return _progressStatus; }
            set
            {
                _progressStatus = value;
                OnPropertyChanged("progressStatus");
            }
        }


        private string _progressText = string.Empty;
        public string ProgressText
        {
            get { return _progressText; }
            set
            {
                _progressText = value;
                OnPropertyChanged("ProgressText");
            }
        }

        private Visibility _isDownloading = Visibility.Collapsed;
        public Visibility isDownloading
        {
            get { return _isDownloading; }
            set
            {
                _isDownloading = value;
                OnPropertyChanged("isDownloading");
            }
        }

        private bool match = true;
        private bool expanded;
        private ObservableCollection<CategoryTemplateMiddlewareVM> childs = new ObservableCollection<CategoryTemplateMiddlewareVM>();
        public CategoryTemplateMiddlewareVM(IEnumerable<CategoryTemplateMiddlewareVM> childs)
        {
            this.childs = new ObservableCollection<CategoryTemplateMiddlewareVM>(childs);
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        public CategoryTemplateMiddlewareVM()
        {
            EditItemCommand = new RelayCommand(EditItemCommandHandler, param => true);
            DeleteItemCommand = new RelayCommand(DeleteItemCommandHandler, param => true);
            UploadItemCommand = new RelayCommand(UploadItemCommandHandler, param => true);
            CreateFolderCommand = new RelayCommand(CreateFolderCommandHandler, param => true);
            DownloadItemCommand = new RelayCommand(DownloadFolderCommandhandler, param => true);
        }

        private async void DownloadFolderCommandhandler(object obj)
        {
            try
            {
                var template = templatedata;
                if (template == null) return;
                var actualFileName = template.SystemFileName;
                var fileName = template.TemplateName;
                System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
                saveFileDialog1.Filter = "Excel Files | *.xls; *.xlsx; *.xlsm; *.xlsb; *.xltx; *.xltm; *.xlt; *.xml; *.xlam";
                saveFileDialog1.Title = "Save the Downloaded Excel";
                saveFileDialog1.FileName = fileName;
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var name = saveFileDialog1.FileName;
                    var finalName = name.Substring(0, name.LastIndexOf("."));
                    saveFileDialog1.FileName = finalName + Path.GetExtension(actualFileName);
                    isDownloading = Visibility.Visible;
                    ProgressText = "Downloading....";
                    progressStatus = 0.0;
                    await System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                    });
                    var Result = await requestFile(actualFileName);
                    if (Result == null)
                    {
                        ProgressText = "Unable to download file";
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));
                            isDownloading = Visibility.Collapsed;
                        });
                        return;
                    }
                    fileName = Result.FirstOrDefault().Value;
                    File.Copy(fileName, saveFileDialog1.FileName, true);
                    ProgressText = "File saved successfully ";
                    //GlobalUtilityLibrary.CustomUserControl.CustomMessageBox.Show("File saved successfully " + Environment.NewLine + saveFileDialog1.FileName, Constants.TemplateRepositoryTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));
                        isDownloading = Visibility.Collapsed;
                    });
                }
            }
            catch (Exception ex)
            {
                isDownloading = Visibility.Collapsed;
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
                    if (e.TotalBytes != null)
                    {
                        progressStatus = (double)e.ProgressPercentage;
                        UpdateprogressText(e.TotalBytes, e.BytesTransferred, e.ProgressPercentage);
                    }
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

        private void CreateFolderCommandHandler(object obj)
        {
            if (IsNodeExpanded)
                TemplateRepositoryAdminViewModel.lastexpandedNodeIndex = (int)Categorydata.Id;
            AddEditWindowViewModel addEditWindowViewModel = new AddEditWindowViewModel(Categorydata)
            {
                PopUpTitle = "Create Sub-category",
                IsNewFolder = true,
                IsNewCategory = false,
                IsArtefactLabelGridVisible = false,
                IsNameLabelGridVisible = true,
                AddEditButtonName = "Create",
                LabelName = "Name",
                ActionName = "InsertFolder",
                ParentId = (int)Categorydata.Id,
                CategoryName = Categorydata.CategoryName
            };
            AddEditWindowView createfolder = new AddEditWindowView(addEditWindowViewModel);
            createfolder.ShowDialog();
        }
        private void UploadItemCommandHandler(object obj)
        {
            GlobalUtility.LogError("Upload Clicked");
            TemplateRepositoryAdminViewModel.lastexpandedNodeIndex = (int)Categorydata.Id;
            Dictionary<object, object> pubsubobj = new Dictionary<object, object>();
            pubsubobj.Add("TemplateRepositoryExcelAdmin", "");
            pubsubobj.Add("CategoryData", Categorydata);

            Utils.InitiateWorkOnExcelEvent.GetEvent<CrossModuleCommunication>().Publish(pubsubobj);

            //UploadTemplateView uploadTemplateInstance = new UploadTemplateView(Categorydata);
            //uploadTemplateInstance.ShowDialog();
        }
        private void DeleteItemCommandHandler(object obj)
        {
            if (IsCategory == Visibility.Visible)
                deleteCategory();
            else
                deleteTemplate();
        }

        private async void deleteTemplate()
        {
            try
            {
                TemplateRepositoryAdminViewModel.lastexpandedNodeIndex = Convert.ToInt32(templatedata.CategoryID);
                var dialogResult = CustomMessageBoxExcel.Show("Do you want to delete template ?", "Delete Template", ExcelMessageBoxType.ConfirmationWithYesNo, ExcelMessageBoxImage.Information);
                if (dialogResult)
                {
                    ChildCategoryModel Obj = new ChildCategoryModel();
                    Obj.TemplateWorkbookId = templatedata.TemplateWorkbookId;
                    LoaderEvent(true, null);


                    // To collect worksheetname, workbookname and other artefacts for deletion
                    List<string> fileNames = new List<string>();
                    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                    fileNames.Add(templatedata.SystemFileName);
                    keyValuePairs.Add("TemplateWorkbookId", templatedata.TemplateWorkbookId.ToString());
                    var collectionData = await ServiceCallHelper.GetDataByParameter<ObservableCollection<WorksheetInfoModel>>("pitchready/TemplateRepositoryExcel/GetWorksheetInfo", keyValuePairs, GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);
                    if (collectionData != null)
                    {
                        foreach (var data in collectionData)
                        {
                            fileNames.Add(data.SystemWorksheetName);
                        }
                    }

                    var result = await ServiceCallHelper.DMLSaveUpdateData<ChildCategoryModel>("pitchready/TemplateRepositoryExcel/DeletTemplateWorkbookInfo", Obj, GlobalUtilityLibrary.Enumrations.CRUDType.Delete, GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);
                    if (result != null)
                    {
                        if (result.SatusCode > 0)
                        {
                            await ServiceCallHelper.DMLSaveUpdateData<List<string>>("pitchready/TemplateRepositoryExcel/DeleteArtefact", fileNames, GlobalUtilityLibrary.Enumrations.CRUDType.Delete, GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);
                            TemplateRepositoryAdminViewModel.istreeDataUpdated = true;
                            TemplateRepositoryAdminViewModel.isValueUpdate = true;

                            CustomMessageBoxExcel.Show("Template deleted successfully.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                        }
                        else
                        {
                            CustomMessageBoxExcel.Show("Template/Folder data has been modified, Please reload the form and try again.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        CustomMessageBoxExcel.Show("Unable to delete record. Please try again.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Warning);
                    }
                    LoaderEvent(false, null);
                }
            }
            catch (Exception ex)
            {
                LoaderEvent(false, null);
            }
        }

        private async void deleteCategory()
        {
            try
            {
                TemplateRepositoryAdminViewModel.lastexpandedNodeIndex = Convert.ToInt32(Categorydata.ParentNode.Id);
                var dialogResult = CustomMessageBoxExcel.Show("Do you want to delete sub-category ?", "Delete Sub-category", ExcelMessageBoxType.ConfirmationWithYesNo, ExcelMessageBoxImage.Question);
                List<ChildCategoryModel> templateListData = null;
                if (dialogResult)
                {
                    LoaderEvent(true, null);
                    bool isDeleted = false;
                    var dialogResult2 = new GenericListDialog(Categorydata, new List<ChildCategoryModel>());
                    Dictionary<string, string> Params = new Dictionary<string, string>();
                    Params.Add("CategoryID", Categorydata.Id.ToString());
                    //Params.Add("CategoryParentId", Categorydata.Id.ToString());
                    //Params.Add("CategoryName", Categorydata.CategoryName);
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
                            dialogResult2 = new GenericListDialog(Categorydata, templateListData);
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
                        //BaseModal<ParentCategoryModel> result = await ServiceCallHelper.DMLSaveUpdateData(Constants.deleteCatagoyData, new BaseModal<ParentCategoryModel>() { inputData = Categorydata }, GlobalUtilityLibrary.Enumrations.CRUDType.Delete, GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);
                        var parametersToUpload = new ParentCategoryModel
                        {
                            Id = Categorydata.Id,
                            CategoryName = Categorydata.CategoryName,
                            CategoryParentId = Categorydata.ParentNode.Id
                        };
                        var result = await ServiceCallHelper.DMLSaveUpdateData<ParentCategoryModel>("pitchready/TemplateRepositoryExcel/DMLOperationWithoutFiles", parametersToUpload, GlobalUtilityLibrary.Enumrations.CRUDType.Delete, GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);

                        if (result != null)
                        {
                            if (result.SatusCode > 0)
                            {
                                await ServiceCallHelper.DMLSaveUpdateData<List<string>>("pitchready/TemplateRepositoryExcel/DeleteArtefact", fileNames, GlobalUtilityLibrary.Enumrations.CRUDType.Delete, GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);

                                TemplateRepositoryAdminViewModel.istreeDataUpdated = true;
                                CustomMessageBoxExcel.Show("Sub-category deleted successfully.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                            }
                            else
                            {
                                CustomMessageBoxExcel.Show("Template/Folder data has been modified, Please reload the form and try again.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Warning);
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
            catch (Exception)
            {
                LoaderEvent(false, null);
            }
        }

        private void EditItemCommandHandler(object obj)
        {
            if (IsCategory == Visibility.Visible)
            {
                if (IsNodeExpanded)
                    TemplateRepositoryAdminViewModel.lastexpandedNodeIndex = (int)Categorydata.Id;
                AddEditWindowViewModel addEditWindowViewModel = new AddEditWindowViewModel(Categorydata)
                {
                    PopUpTitle = "Update Sub-category",
                    IsNewFolder = true,
                    IsNewCategory = false,
                    IsArtefactLabelGridVisible = false,
                    IsNameLabelGridVisible = true,
                    AddEditButtonName = "Update",
                    LabelName = "Name",
                    ActionName = "UpdateFolder",
                    CategoryId = (int)Categorydata.Id,
                    TextBoxName = Categorydata.CategoryName,
                    ParentId = Categorydata.CategoryParentId,
                    CategoryName = Categorydata.CategoryName,
                    previousTextboxName = Categorydata.CategoryName
                };
                AddEditWindowView editfolder = new AddEditWindowView(addEditWindowViewModel);
                editfolder.ShowDialog();
            }
            else
            {
                TemplateRepositoryAdminViewModel.lastexpandedNodeIndex = Convert.ToInt32(templatedata.CategoryID);
                Dictionary<object, object> pubsubobj = new Dictionary<object, object>();
                pubsubobj.Add("EditTemplateRepositoryExcelAdmin", "");
                pubsubobj.Add("TemplateData", templatedata);

                Utils.InitiateWorkOnExcelEvent.GetEvent<CrossModuleCommunication>().Publish(pubsubobj);
            }
        }

        public ObservableCollection<CategoryTemplateMiddlewareVM> Childs
        {
            get { return childs; }
            set
            {
                childs = value;
                OnPropertyChanged("Childs");
            }
        }

        public bool IsNodeExpanded
        {
            get { return expanded; }
            set
            {
                //if (expanded == value) return;
                expanded = value;
                OnPropertyChanged("IsNodeExpanded");
            }
        }


        public IEnumerable<CategoryTemplateMiddlewareVM> Children
        {
            get { return Childs; }
        }

        private bool IsCriteriaMatched(string criteria)
        {
            if (IsCategory == Visibility.Visible)
                return Categorydata.CategoryName.ToLower().Contains(criteria.ToLower());
            else if (IsTemplate == Visibility.Visible)
                return templatedata.TemplateName.ToLower().Contains(criteria.ToLower());
            else
                return false;
        }

        public ParentCategoryModel Categorydata { get; set; }
        public ChildCategoryModel templatedata { get; set; }

        public bool isDeleted { get; set; }
        public string SearchTitleName
        {
            get
            {
                if (IsCategory == Visibility.Visible)
                    return templatedata.CategoryName;
                else
                    return templatedata.TemplateName;
            }

        }

        public DateTime? SearchDate
        {
            get
            {
                if (IsCategory == Visibility.Visible)
                    return Categorydata.UpdatedAtCategory;
                else
                    return templatedata.UpdatedAtTemplate;
            }

        }

        public CategoryTemplateMiddlewareVM ParentNode { get; set; }

        public Visibility isAddCategoryVisible
        {
            get
            {
                if (IsCategory == Visibility.Visible)
                {
                    if (this.Children.Count() > 0)
                        return this.Children.Any(o => o.IsTemplate == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
                    else
                        return Visibility.Visible;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

        public Visibility isAddTemplateVisible
        {
            get
            {
                if (IsCategory == Visibility.Visible)
                {
                    if (this.Children.Count() > 0)
                        return this.Children.Any(o => o.IsTemplate == Visibility.Visible) ? Visibility.Visible : Visibility.Collapsed;
                    else
                        return Visibility.Visible;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

        public Visibility IsCategory
        {
            get
            {
                return Categorydata != null ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility IsTemplate
        {
            get
            {
                return IsCategory == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        private void UpdateprogressText(long? totalBytes, long bytesTransferred, int progressPercentage)
        {
            ProgressText = progressPercentage + "% (" + ((Convert.ToDecimal(bytesTransferred) / 1024) / 1024).ToString("00.00") + " MB / " + ((Convert.ToDecimal(totalBytes) / 1024) / 1024).ToString("00.00") + " MB)";
        }
    }
}
