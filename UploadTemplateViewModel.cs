using GlobalUtilityLibrary;
using GlobalUtilityLibrary.Entities;
using GlobalUtilityLibrary.Helpers;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using PitchreadyExcel.Config;
using PitchreadyGlobal.DependencyServices;
using PitchreadyGlobal.Enumrations;
using PitchreadyGlobal.Helpers;
using PitchreadyGlobal.UserControls;
using PitchreadyGlobal.ViewModels;
using PitchreadyGlobal.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Packaging;
using Application = Microsoft.Office.Interop.Excel.Application;
using RelayCommand = PitchreadyGlobal.Helpers.RelayCommand;

namespace PitchreadyExcel.ViewModel
{
    public class UploadTemplateViewModel : PitchreadyGlobalBaseVM
    {
        public static int previewPresentWorkbookLevel = 0;
        public bool IsCurrentWorkbookCorrupt = false;
        public List<string> CorruptWorkbookNames = new List<string>();
        Application excelApp;
        Workbook excelWorkbook;
        int buttonCounter = 1;
        int counter = 0;
        string xpsFileName = string.Empty;
        public bool IsAddtemplateInProgress = false;
        public int parentid { get; set; }

        private RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get { return _addCommand; }
            set
            {
                _addCommand = value;
                OnPropertyChanged("AddCommand");
            }
        }

        private bool _showImageViewer;
        public bool ShowImageViewer
        {
            get { return _showImageViewer; }
            set
            {
                _showImageViewer = value;
                OnPropertyChanged("ShowImageViewer");
            }
        }

        private bool _showDocumentViewer = true;
        public bool ShowDocumentViewer
        {
            get { return _showDocumentViewer; }
            set
            {
                _showDocumentViewer = value;
                OnPropertyChanged("ShowDocumentViewer");
            }
        }

        private bool _showCountGrid;
        public bool ShowCountGrid
        {
            get { return _showCountGrid; }
            set
            {
                _showCountGrid = value;
                OnPropertyChanged("ShowCountGrid");
            }
        }

        private RelayCommand _nextCommand;
        public RelayCommand NextCommand
        {
            get { return _nextCommand; }
            set
            {
                _nextCommand = value;
                OnPropertyChanged("NextCommand");
            }
        }

        private RelayCommand _previousCommand;
        public RelayCommand PreviousCommand
        {
            get { return _previousCommand; }
            set
            {
                _previousCommand = value;
                OnPropertyChanged("PreviousCommand");
            }
        }

        public Dictionary<int, List<WorksheetInfo>> WorkSheetsLookUp = new Dictionary<int, List<WorksheetInfo>>();

        private FixedDocumentSequence _fixedDocumentSequence;
        public FixedDocumentSequence FixedDocumentSequenceVar
        {
            get
            {
                return _fixedDocumentSequence;
            }
            set
            {
                _fixedDocumentSequence = value;
                OnPropertyChanged("FixedDocumentSequenceVar");
            }
        }

        private RelayCommand _closeCommand;
        public RelayCommand CloseCommand
        {
            get { return _closeCommand; }
            set
            {
                _closeCommand = value;
                OnPropertyChanged("CloseCommand");
            }
        }

        private RelayCommand _browseCommand;
        public RelayCommand BrowseCommand
        {
            get { return _browseCommand; }
            set
            {
                _browseCommand = value;
                OnPropertyChanged("BrowseCommand");
            }
        }

        private RelayCommand _formClosedCommand;
        public RelayCommand FormClosedCommand
        {
            get { return _formClosedCommand; }
            set
            {
                _formClosedCommand = value;
                OnPropertyChanged("FormClosedCommand");
            }
        }

        private RelayCommand _selectionChangedCommand;
        public RelayCommand SelectionChangedCommand
        {
            get { return _selectionChangedCommand; }
            set
            {
                _selectionChangedCommand = value;
                OnPropertyChanged("SelectionChangedCommand");
            }
        }

        private templateUploadVM _selectedWorkbookData;

        public templateUploadVM SelectedWorkbookData
        {
            get { return _selectedWorkbookData; }
            set
            {
                _selectedWorkbookData = value;
                OnPropertyChanged("SelectedWorkbookData");
            }
        }

        private ObservableCollection<templateUploadVM> _workBookCollection = new ObservableCollection<templateUploadVM>();
        public ObservableCollection<templateUploadVM> WorkBookCollection
        {
            get { return _workBookCollection; }
            set
            {
                _workBookCollection = value;
                OnPropertyChanged("WorkBookCollection");
            }
        }

        private bool _isEnable = true;
        public bool isEnable
        {
            get { return _isEnable; }
            set
            {
                _isEnable = value;
                OnPropertyChanged("isEnable");
            }
        }

        private bool _isRetryEnabled = false;
        public bool IsRetryEnabled
        {
            get { return _isRetryEnabled; }
            set
            {
                _isRetryEnabled = value;
                OnPropertyChanged("IsRetryEnabled");
            }
        }



        private bool _isGridRowSelected = false;
        public bool IsGridRowSelected
        {
            get { return _isGridRowSelected; }
            set
            {
                _isGridRowSelected = value;
                OnPropertyChanged("IsGridRowSelected");
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                SelectedWorkbookData.Description = value;
                if (!string.IsNullOrEmpty(value))
                {
                    SelectedWorkbookData.IsDescriptionPresent = "Yes";
                }
                else
                {
                    SelectedWorkbookData.IsDescriptionPresent = "No";
                }
                OnPropertyChanged("Description");
            }
        }

        public UploadTemplateViewModel()
        {
            Predicate<object> nextPredicate = EnableDisableNextButton;
            Predicate<object> previousPredicate = EnableDisablePreviousButton;
            AddCommand = new RelayCommand(AddCommandHandler);
            BrowseCommand = new RelayCommand(BrowseCommandHandler);
            EscCommand = new RelayCommand(EscCommandHandler);
            FormClosedCommand = new RelayCommand(FormClosedCommandHandler);
            CloseCommand = new RelayCommand(CloseCommandHandler);
            SelectionChangedCommand = new RelayCommand(SelectionChangedCommandHandler);
            NextCommand = new RelayCommand(NextCommandHandler, nextPredicate);
            PreviousCommand = new RelayCommand(PreviousCommandHandler, previousPredicate);
        }

        private void NextCommandHandler(object obj)
        {
            SelectedWorkbookData.ButtonCount = ++buttonCounter;
            DisplayXPSFile(SelectedWorkbookData.TempWorkBookId, buttonCounter);
        }

        private void PreviousCommandHandler(object obj)
        {
            SelectedWorkbookData.ButtonCount = --buttonCounter;
            DisplayXPSFile(SelectedWorkbookData.TempWorkBookId, buttonCounter);
        }

        private void DisplayXPSFile(int workbookId, int previewId)
        {
            var val = WorkSheetsLookUp[workbookId].Where(o => o.Id == buttonCounter).FirstOrDefault();
            if (val == null || val.FileLocation == string.Empty)
            {
                SelectedWorkbookData.WorksheetName = val.WorksheetName;
                ShowDocumentViewer = false;
                ShowImageViewer = true;
            }
            else
            {
                SelectedWorkbookData.WorksheetName = val.WorksheetName;
                ShowDocumentViewer = true;
                ShowImageViewer = false;
                XpsDocument xpsPackage = new XpsDocument(val.FileLocation, FileAccess.Read, CompressionOption.SuperFast);
                FixedDocumentSequence fixedDocumentSequence = xpsPackage.GetFixedDocumentSequence();
                FixedDocumentSequenceVar = fixedDocumentSequence;
            }
        }

        private bool EnableDisableNextButton(object obj)
        {
            try
            {
                if (SelectedWorkbookData != null && (buttonCounter == SelectedWorkbookData.PreviewCount || SelectedWorkbookData.PreviewCount == 0))
                    return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool EnableDisablePreviousButton(object obj)
        {
            try
            {
                if (buttonCounter == 1 || (SelectedWorkbookData != null && SelectedWorkbookData.PreviewCount == 0))
                    return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void SelectionChangedCommandHandler(object obj)
        {
            try
            {
                if (obj != null)
                {
                    SelectedWorkbookData.ButtonCount = 1;
                    Description = SelectedWorkbookData.Description;
                    IsGridRowSelected = true;
                    buttonCounter = 1;
                    FixedDocumentSequenceVar = null;
                    DisplayXPSFileFirstTime(WorkSheetsLookUp[SelectedWorkbookData.TempWorkBookId]);
                }
            }
            catch
            {

            }
        }

        private void FormClosedCommandHandler(object obj)
        {
            if (WorkBookCollection == null) return;
            foreach (var item in WorkBookCollection)
                item.isProcessCancelled = true;
        }

        private void EscCommandHandler(object obj)
        {
            Close();
        }

        private async void BrowseCommandHandler(object obj)
        {
            try
            {
                var firstActiveWbName = Globals.ThisAddIn.Application.ActiveWorkbook.Name;
                System.Windows.Forms.OpenFileDialog iconFileDialog = new System.Windows.Forms.OpenFileDialog();
                iconFileDialog.Filter = "Excel Files | *.xls; *.xlsx; *.xlsm; *.xlsb; *.xltx; *.xltm; *.xlt; *.xml; *.xlam";
                iconFileDialog.Multiselect = true;
                IsLoading = true;
                LoadingText = "Generating Previews...";
                if (iconFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    templateUploadVM.FileSizeInKB = Math.Round(Convert.ToDouble(new FileInfo(iconFileDialog.FileName).Length / 1024), 2);

                    foreach (var item in iconFileDialog.FileNames)
                    {
                        var fileExtension = "." + Path.GetExtension(item);
                        var LocalFilePath = GlobalUtility.GetTempFilePath(GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository, GlobalUtilityLibrary.Enumrations.FileExtentions.xlsx);
                        LocalFilePath = LocalFilePath.Replace(".xlsx", fileExtension);
                        File.Copy(item, LocalFilePath, true);
                        previewPresentWorkbookLevel = 0;
                        xpsFileName = string.Empty;
                        var tempData = new templateUploadVM()
                        {
                            parentid = parentid,
                            FilePath = System.IO.Path.GetFileName(item),
                            TemplateName = System.IO.Path.GetFileNameWithoutExtension(item),
                            FilePathTag = item,
                            DeleteCommand = new RelayCommand(DeletecommandHandler),
                            TempWorkBookId = ++counter
                        };
                        FixedDocumentSequenceVar = null;
                        WorkBookCollection.Add(tempData);
                        SelectedWorkbookData = tempData;
                        IsLoading = true;
                        ShowCountGrid = false;
                        await System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            OpenExcel(tempData.TempWorkBookId, LocalFilePath, tempData.TemplateName);
                        });
                        if (IsCurrentWorkbookCorrupt)
                        {
                            WorkBookCollection.Remove(tempData);
                        }
                    }
                    ShowCountGrid = true;
                }
                IsLoading = false;
                if (CorruptWorkbookNames.Any())
                {
                    var dialogResult = new GenericListDialog(CorruptWorkbookNames);
                    dialogResult.ShowDialog();
                }

                if (WorkBookCollection.Any())
                {
                    var Id = WorkBookCollection.Max(x => x.TempWorkBookId);
                    SelectedWorkbookData = WorkBookCollection.Where(x => x.TempWorkBookId == Id).FirstOrDefault();
                    SelectionChangedCommandHandler(SelectedWorkbookData);
                }

                ReactivateForm();
            }
            catch (Exception ex)
            {
                IsLoading = false;
            }
            finally
            {
                CorruptWorkbookNames.Clear();
            }
        }

        public void CloseExcel()
        {
            if (excelWorkbook != null)
            {
                excelWorkbook.Close(0);

                Marshal.ReleaseComObject(excelWorkbook);

                excelWorkbook = null;
            }
        }


        private void OpenExcel(int workbookId, string path, string originalWorkbbokName)
        {
            try
            {
                Globals.ThisAddIn.Application.DisplayAlerts = false;
                Globals.ThisAddIn.Application.Visible = true;
                Globals.ThisAddIn.Application.ScreenUpdating = false;
                excelWorkbook = Globals.ThisAddIn.Application.Workbooks.Open(path, Type.Missing, false, Type.Missing, Type.Missing,
                       Type.Missing, false, XlPlatform.xlWindows, Type.Missing,
                       true, false, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                ExportXPS(workbookId, excelWorkbook, path, originalWorkbbokName); // XPS Exporter is explained at next description
                CloseExcel();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Stack Trace : " + ex.StackTrace);
            }
        }


        void ExportXPS(int workbookId, Workbook excelWorkbook, string path, string originalWorkbbokName)
        {
            try
            {
                var xpsFileNameDef = (new DirectoryInfo(path)).FullName;

                xpsFileNameDef = xpsFileNameDef.Replace(new FileInfo(path).Extension, "");

                SelectedWorkbookData.WorksheetCount = excelWorkbook.Worksheets.Count;

                var count = 0;
                List<WorksheetInfo> tempList = new List<WorksheetInfo>();
                IsCurrentWorkbookCorrupt = false;
                foreach (Worksheet sheet in excelWorkbook.Worksheets)
                {
                    LoadingText = "Generating Preview " + (count + 1) + " of " + SelectedWorkbookData.WorksheetCount;
                    Range tableRange = null;

                    foreach (Name name in sheet.Names)
                    {
                        if (name.Name.ToLower().Contains("pr_preview"))
                        {
                            tableRange = name.RefersToRange;

                            if (tableRange.Rows.Count > 50000 || tableRange.Columns.Count > 50000)
                            {
                                CorruptWorkbookNames.Add(originalWorkbbokName);
                                IsCurrentWorkbookCorrupt = true;
                                return;
                            }
                            break;
                        }
                    }

                    if (tableRange != null && sheet.Visible == XlSheetVisibility.xlSheetVisible)
                    {
                        sheet.PageSetup.PrintArea = tableRange.AddressLocal;
                        sheet.PageSetup.Zoom = false;
                        sheet.PageSetup.FitToPagesWide = 1;
                        sheet.PageSetup.FitToPagesTall = 1;
                        sheet.PageSetup.LeftMargin = 5;
                        sheet.PageSetup.RightMargin = 5;
                        sheet.PageSetup.TopMargin = 5;
                        sheet.PageSetup.BottomMargin = 5;
                        sheet.PageSetup.HeaderMargin = 5;
                        sheet.PageSetup.FooterMargin = 5;

                        var FileUniqueName = Guid.NewGuid();

                        var getTempPath = Path.GetTempPath();

                        if (!Directory.Exists(getTempPath + "\\" + "TemplateRepositoryXPS"))
                        {
                            Directory.CreateDirectory(getTempPath + "\\" + "TemplateRepositoryXPS");
                        }


                        var TempFileName = getTempPath + "\\" + "TemplateRepositoryXPS\\" + FileUniqueName + "_" + count + ".xps";

                        sheet.ExportAsFixedFormat(XlFixedFormatType.xlTypeXPS,
                                                              Filename: TempFileName,
                                                              OpenAfterPublish: false);

                        var obj = new WorksheetInfo();
                        obj.Id = ++count;

                        obj.FileLocation = TempFileName;

                        obj.WorksheetName = sheet.Name;

                        obj.HasPreview = true;

                        tempList.Add(obj);

                        ++previewPresentWorkbookLevel;
                    }
                    else
                    {
                        tempList.Add(new WorksheetInfo
                        {
                            Id = ++count,
                            WorksheetName = sheet.Name,
                            FileLocation = string.Empty,
                            HasPreview = false
                        });
                    }
                    xpsFileName = string.Empty;
                }

                WorkSheetsLookUp.Add(workbookId, tempList);

                SelectedWorkbookData.PreviewCount = tempList.Count;
            }
            catch (Exception ex)
            {

            }
        }

        void DisplayXPSFileFirstTime(List<WorksheetInfo> worksheetInfos)
        {
            try
            {
                if (worksheetInfos.Count > 0)
                {
                    if (worksheetInfos[0].FileLocation != string.Empty)
                    {
                        //To Trigger The Button
                        SendKeys.Send("{TAB}");
                        SelectedWorkbookData.WorksheetName = worksheetInfos[0].WorksheetName;
                        ShowDocumentViewer = true;
                        ShowImageViewer = false;
                        XpsDocument xpsPackage = new XpsDocument(worksheetInfos[0].FileLocation, FileAccess.Read, CompressionOption.SuperFast);
                        FixedDocumentSequence fixedDocumentSequence = xpsPackage.GetFixedDocumentSequence();
                        FixedDocumentSequenceVar = fixedDocumentSequence;
                    }
                    else
                    {
                        //SendKeys.Send("{TAB}");
                        SelectedWorkbookData.WorksheetName = worksheetInfos[0].WorksheetName;
                        ShowDocumentViewer = false;
                        ShowImageViewer = true;
                    }
                }
                else
                {
                    ShowDocumentViewer = false;
                    ShowImageViewer = true;
                }
            }
            catch
            {

            }
        }

        private void DeletecommandHandler(object obj)
        {
            var templateObj = obj as templateUploadVM;
            if (templateObj == null) return;
            if (IsAddtemplateInProgress)
            {
                CustomMessageBoxExcel.Show("Unable to process your request as uploading Template is still in processing.", GlobalUtilityLibrary.Helpers.Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Warning);
                return;
            }
            WorkBookCollection.Remove(templateObj);
            if (WorkBookCollection.Count == 0 || WorkBookCollection.All(o => o.IsUploadSuccess))
            {
                isEnable = true;
                IsRetryEnabled = false;
            }
            if (WorkBookCollection.Any())
            {
                var Id = WorkBookCollection.Min(x => x.TempWorkBookId);
                FixedDocumentSequenceVar = null;
                SelectedWorkbookData = WorkBookCollection.Where(x => x.TempWorkBookId == Id).FirstOrDefault();
                SelectionChangedCommandHandler(SelectedWorkbookData);
            }
            else
            {
                SelectedWorkbookData = null;
                FixedDocumentSequenceVar = null;
            }
        }

        private async void AddCommandHandler(object obj)
        {
            //foreach (var item in WorkBookCollection)
            //{
            //    if (string.IsNullOrEmpty(item.Description))
            //    {
            //        CustomMessageBoxExcel.Show("Please add template(s) description.", GlobalUtilityLibrary.Helpers.Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Warning);
            //        return;
            //    }
            //}
            if (WorkSheetsLookUp.Count == 0)
            {
                CustomMessageBoxExcel.Show("The message filter indicated that the application is busy. Please Re-try", GlobalUtilityLibrary.Helpers.Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Warning);
                this.Close();
                return;
            }

            if (WorkBookCollection.Count() == 0)
            {
                CustomMessageBoxExcel.Show("Please add template(s).", GlobalUtilityLibrary.Helpers.Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Warning);
                return;
            }
            if (WorkBookCollection.Any(o => o.isEnable == Visibility.Visible))
            {
                CustomMessageBoxExcel.Show("Please provide Template name for all entries.", GlobalUtilityLibrary.Helpers.Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Warning);
                return;
            }

            if (WorkBookCollection.Count > WorkBookCollection.Select(m => m.TemplateName).Distinct().Count())
            {
                CustomMessageBoxExcel.Show("Template(s) name should be unique for all record.", GlobalUtilityLibrary.Helpers.Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Warning);
                return;
            }

            isEnable = false;
            IsAddtemplateInProgress = true;
            foreach (var item in WorkBookCollection)
                item.IsNotUploadig = false;
            foreach (templateUploadVM item in WorkBookCollection)
            {
                if (item.isProcessCancelled)
                    break;
                if (!item.IsUploadSuccess)
                    await item.StartUpload(WorkSheetsLookUp, item.TempWorkBookId, item.FilePathTag);
                else
                    continue;
                if (item.IsUploadSuccess)
                {
                    TemplateRepositoryAdminViewModel.istreeDataUpdated = true;
                    TemplateRepositoryAdminViewModel.isValueUpdate = true;
                }
            }

            IsAddtemplateInProgress = false;
            if (WorkBookCollection.All(o => !o.isProcessCancelled))
            {
                if (WorkBookCollection.Any(o => !o.IsUploadSuccess))
                {
                    IsRetryEnabled = true;
                    CustomMessageBoxExcel.Show("Template cannot be duplicate. Please try again with a unique name.", GlobalUtilityLibrary.Helpers.Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);

                }
                else
                    CustomMessageBoxExcel.Show("Template(s) uploaded successfully.", GlobalUtilityLibrary.Helpers.Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
            }

            this.Close();
        }

        private void CloseCommandHandler(object obj)
        {
            this.Close();
        }
    }

    public class templateUploadVM : PitchreadyGlobalBaseVM
    {
        public static double FileSizeInKB { get; set; }

        private string _workSheetName;
        public string WorksheetName
        {
            get
            {
                return _workSheetName;
            }
            set
            {
                _workSheetName = value;
                OnPropertyChanged("WorksheetName");
            }
        }

        private int _buttonCount = 1;
        public int ButtonCount
        {
            get { return _buttonCount; }
            set
            {
                _buttonCount = value;
                OnPropertyChanged("ButtonCount");
            }
        }

        private int _previewCount = 1;
        public int PreviewCount
        {
            get { return _previewCount; }
            set
            {
                _previewCount = value;
                OnPropertyChanged("PreviewCount");
            }
        }

        private int _worksheetCount;
        public int WorksheetCount
        {
            get { return _worksheetCount; }
            set
            {
                _worksheetCount = value;
                OnPropertyChanged("WorksheetCount");
            }
        }

        public int IsPreviewAvailable { get; set; }
        public int TempWorkBookId { get; set; }
        public int parentid { get; set; }
        private RelayCommand _deleteCommand;
        private SolidColorBrush errorFontColor = Brushes.DarkRed;
        public RelayCommand DeleteCommand
        {
            get { return _deleteCommand; }
            set
            {
                _deleteCommand = value;
                OnPropertyChanged("DeleteCommand");
            }
        }

        private ImageSource _statusImage = new BitmapImage(new Uri("/PitchreadyPowerPoint;component/Media/Icons/ErrorIcon.png", UriKind.Relative));
        public ImageSource StatusImage
        {
            get { return _statusImage; }
            set
            {
                _statusImage = value;
                OnPropertyChanged("StatusImage");
            }
        }

        private string _templateName = string.Empty;
        public string TemplateName
        {
            get { return _templateName; }
            set
            {
                _templateName = value;
                validateEntries();
                OnPropertyChanged("TemplateName");
            }
        }

        private bool _isUploadSuccess = false;
        public bool IsUploadSuccess
        {
            get { return _isUploadSuccess; }
            set
            {
                _isUploadSuccess = value;
                StatusImage = value ? new BitmapImage(new Uri("/PitchreadyPowerPoint;component/Media/Icons/RightIcon.png", UriKind.Relative)) : new BitmapImage(new Uri("/PitchreadyPowerPoint;component/Media/Icons/ErrorIcon.png", UriKind.Relative));
                ErrorFontColor = value ? Brushes.Green : Brushes.DarkRed;
                OnPropertyChanged("IsUploadSuccess");
            }
        }

        private Visibility _isUploadComplete = Visibility.Collapsed;
        public Visibility IsUploadComplete
        {
            get { return _isUploadComplete; }
            set
            {
                _isUploadComplete = value;
                OnPropertyChanged("IsUploadComplete");
            }
        }



        private Visibility _isEnable = Visibility.Visible;
        public Visibility isEnable
        {
            get { return _isEnable; }
            set
            {
                _isEnable = value;
                OnPropertyChanged("isEnable");
            }
        }

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

        private string _errorText = string.Empty;
        public string ErrorText
        {
            get { return _errorText; }
            set
            {
                _errorText = value;
                OnPropertyChanged("ErrorText");
            }
        }

        private long _totalFileSize = 0;
        public long TotalFileSize
        {
            get { return _totalFileSize; }
            set
            {
                _totalFileSize = value;
                OnPropertyChanged("TotalFileSize");
            }
        }

        private double _percentage = 0.0;
        public double Percentage
        {
            get { return _percentage; }
            set
            {
                _percentage = value;
                OnPropertyChanged("Percentage");
            }
        }

        private long _uploadedFileSize = 0;
        public long UploadedFileSize
        {
            get { return _uploadedFileSize; }
            set
            {
                _uploadedFileSize = value;
                OnPropertyChanged("UploadedFileSize");
            }
        }

        private string _filePath = string.Empty;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                validateEntries();
                OnPropertyChanged("FilePath");
            }
        }

        public string Description { get; set; }

        private string _isDescriptionPresent = "No";
        public string IsDescriptionPresent
        {
            get
            {
                return _isDescriptionPresent;
            }
            set
            {
                _isDescriptionPresent = value;
                validateEntries();
                OnPropertyChanged("IsDescriptionPresent");
            }
        }

        private string _filePathTag = string.Empty;
        public string FilePathTag
        {
            get { return _filePathTag; }
            set
            {
                _filePathTag = value;
                validateEntries();
                OnPropertyChanged("FilePathTag");
            }
        }

        private bool _isNotUploadig = true;
        public bool IsNotUploadig
        {
            get { return _isNotUploadig; }
            set
            {
                _isNotUploadig = value;
                OnPropertyChanged("IsNotUploadig");
            }
        }


        private bool _isProcessCancelled = false;
        public bool isProcessCancelled
        {
            get { return _isProcessCancelled; }
            set
            {
                _isProcessCancelled = value;
                OnPropertyChanged("isProcessCancelled");
            }
        }

        public SolidColorBrush ErrorFontColor
        {
            get
            {
                return errorFontColor;
            }
            set
            {
                errorFontColor = value;
                OnPropertyChanged("ErrorFontColor");
            }
        }

        private void FormClosedCommandHandler(object obj)
        {
            isProcessCancelled = true;
        }

        Dictionary<int, List<string>> GetCapturedFilePaths(Dictionary<int, List<WorksheetInfo>> WorkSheetsLookUp, int tempWorkBookId, string Xl_Path)
        {
            var returnValue = new Dictionary<int, List<string>>();
            List<string> filestrdata = new List<string>();
            var filesToUpload = WorkSheetsLookUp[tempWorkBookId];

            foreach (var item in filesToUpload)
            {
                filestrdata.Add(item.FileLocation);
            }

            var fileExtension = "." + Path.GetExtension(Xl_Path);
            var LocalFilePath = GlobalUtility.GetTempFilePath(GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository, GlobalUtilityLibrary.Enumrations.FileExtentions.xlsx);
            LocalFilePath = LocalFilePath.Replace(".xlsx", fileExtension);
            File.Copy(Xl_Path, LocalFilePath, true);
            filestrdata.Add(LocalFilePath);
            returnValue.Add(filestrdata.Count, filestrdata);
            return returnValue;
        }

        public async System.Threading.Tasks.Task StartUpload(Dictionary<int, List<WorksheetInfo>> WorkSheetsLookUp, int tempWorkBookId, string xl_path)
        {
            if (isEnable == Visibility.Visible) return;
            IsUploadComplete = Visibility.Collapsed;
            Dictionary<int, List<string>> capturedXPSFilePathList = null;
            string TemplateRepositoryPath = GlobalUtility.GetTemplateRepositoryPath();

            ChildCategoryRequestModel parentNode = new ChildCategoryRequestModel();
            ProgressText = "Preparing...";
            await System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(50);
            });
            if (File.Exists(_filePathTag.Trim()))//in edit mode we show only template name. if user want to update it, browse updated template full path
            {
                ProgressText = "Preparing XPS...";



                capturedXPSFilePathList = GetCapturedFilePaths(WorkSheetsLookUp, tempWorkBookId, xl_path);
                if (isProcessCancelled)
                    return;
                if (capturedXPSFilePathList == null || capturedXPSFilePathList.Count == 0)
                {
                    IsUploadComplete = Visibility.Visible;
                    if (!IsUploadSuccess)
                    {
                        progressStatus = 0.0;
                        ProgressText = string.Empty;
                        IsNotUploadig = true;
                    }
                    return;
                }
            }
            ProgressText = "Initiating Upload...";
            parentNode.TemplateName = this.TemplateName;
            parentNode.Description = this.Description == null ? string.Empty : this.Description;

            parentNode.IsPreviewAvailable = WorkSheetsLookUp[tempWorkBookId].Where(x => x.HasPreview == true).ToList().Count > 1 ? true : false;
            parentNode.OrigFileName = Path.GetFileName(_filePathTag);
            parentNode.CategoryID = parentid;
            parentNode.WorksheetCount = this.WorksheetCount;
            parentNode.FileSizeInKB = (int?)FileSizeInKB;

            List<FileInfo> fileInfo = new List<FileInfo>();
            if (capturedXPSFilePathList != null && capturedXPSFilePathList.Count > 0)
            {
                var FilesData = capturedXPSFilePathList.FirstOrDefault();
                var index = 0;
                fileInfo = new List<FileInfo>();
                foreach (var item in FilesData.Value)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        FileInfo CapturedImageInfo = new FileInfo(item);
                        fileInfo.Add(CapturedImageInfo);
                    }
                    ++index;
                }
            }
            var result = await UpdateDataWithProgress<ChildCategoryModel>(fileInfo, WorkSheetsLookUp[tempWorkBookId], CRUDType.Insert, "pitchready/TemplateRepositoryExcel/InsertTemplatesWorkbookInfo_EXL", parentNode);
            if (result != null)
            {
                if (result.SatusCode > 0)
                {
                    IsUploadSuccess = true;
                    ErrorText = "Uploaded Successfully";
                }
                else
                {
                    ErrorText = "Duplicate Template";
                }
            }
            else
            {
                ErrorText = "Failed.";
            }

            if (capturedXPSFilePathList != null && capturedXPSFilePathList.Count > 0)
            {
                var FilesData = capturedXPSFilePathList.FirstOrDefault();
                foreach (var item in FilesData.Value)
                {
                    GlobalUtility.TryDeleteFile(item);
                }
            }

            IsUploadComplete = Visibility.Visible;
            if (!IsUploadSuccess)
            {
                progressStatus = 0.0;
                ProgressText = string.Empty;
                IsNotUploadig = true;
            }
        }

        public async Task<BaseModal<T>> UpdateDataWithProgress<T>(List<FileInfo> filePaths, List<WorksheetInfo> WorksheetNameList, CRUDType operationType, string saveTemplateData, ChildCategoryRequestModel EntryData)
        {
            BaseModal<T> returnObj = null;
            try
            {
                if (!ServiceCallHelper.IsConnetedWithServer(GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository))
                {
                    return null;
                    //var res = new CustomUserControl.CustomMessageBox(GlobalUtility.CurrentTool, Constants.ConncectivityCheckErrorMessage, CustomUserControl.MessageBoxType.Retry, CustomUserControl.MessageBoxImage.Retry);
                    //res.ShowDialog();
                    //await UpdateData<T>(filePaths, dataStr, EntryData);
                }

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var processMsgHander = new ProgressMessageHandler(new HttpClientHandler());
                HttpClient client = new HttpClient(processMsgHander);
                long? totalbytes = 0;
                client.Timeout = TimeSpan.FromMinutes(10);
                processMsgHander.HttpSendProgress += (sender, e) =>
                {
                    if (isProcessCancelled)
                        client.CancelPendingRequests();
                    if (e.TotalBytes != null)
                    {
                        totalbytes = e.TotalBytes;
                        progressStatus = (double)e.ProgressPercentage;
                        UpdateprogressText(e.TotalBytes, e.BytesTransferred, e.ProgressPercentage);
                    }
                    //add your codes base on e.BytesTransferred and e.ProgressPercentage
                };
                string urlPath = GlobalUtility.DataLocationPath(GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository) + saveTemplateData;

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + await GlobalUtilityLibrary.Helpers.ServiceCallHelper.getAccessToken(GlobalUtility.DataLocationPath(GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository)));

                MultipartFormDataContent form = new MultipartFormDataContent();
                if (EntryData != null)
                {
                    EntryData.InsertedAt = DateTime.Now;
                    EntryData.InsertedBy = GlobalUtility.GetUserName();
                    EntryData.UpdatedAt = DateTime.Now;
                    EntryData.UpdatedBy = GlobalUtility.GetUserName();
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(EntryData));
                    form.Add(content, "OperationData");
                    HttpContent OperationTypecontent = new StringContent(((int)operationType).ToString());
                    form.Add(OperationTypecontent, "OperationType");
                    HttpContent userNameecontent = new StringContent(GlobalUtility.GetUserName());
                    form.Add(userNameecontent, "UserName");
                    HttpContent worksheetNameListcontent = new StringContent(JsonConvert.SerializeObject(WorksheetNameList));
                    form.Add(worksheetNameListcontent, "WorksheetNameList");
                }

                var count = 1;
                HttpContent Streamcontent = null;
                foreach (var filedata in filePaths)
                {
                    if (filedata != null)
                    {
                        Streamcontent = new StreamContent(File.OpenRead(filedata.FullName));
                        Streamcontent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = filedata.Name,
                            FileName = filedata.Name
                        };
                    }

                    form.Add(Streamcontent, "File_Stream_" + count);
                    count++;
                }

                var response = await client.PostAsync(urlPath, form);
                if (response.IsSuccessStatusCode)
                {
                    if (ServiceCallHelper.isTokenRefreshCalled)
                        GlobalUtilityLibrary.Helpers.ServiceCallHelper.isTokenRefreshCalled = false;
                    var read = response.Content.ReadAsStringAsync().Result.Replace("\\\"", "\"").TrimStart("\"".ToCharArray()).TrimEnd("\"".ToCharArray());
                    returnObj = Newtonsoft.Json.JsonConvert.DeserializeObject<BaseModal<T>>(read);
                }
                else
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (GlobalUtilityLibrary.Helpers.ServiceCallHelper.isTokenRefreshCalled)
                    {
                        GlobalUtilityLibrary.Helpers.ServiceCallHelper.isTokenRefreshCalled = false;
                        return null;
                    }
                    await GlobalUtilityLibrary.Helpers.ServiceCallHelper.getAccessToken(GlobalUtility.DataLocationPath(GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository), true);
                    return await UpdateDataWithProgress<T>(filePaths, WorksheetNameList, operationType, saveTemplateData, EntryData);
                }
                else
                {
                    GlobalUtility.LogError("Can not get the response from the server " + (int)response.StatusCode + ",reason phrase= " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return returnObj;
        }

        private void UpdateprogressText(long? totalBytes, long bytesTransferred, int progressPercentage)
        {
            ProgressText = progressPercentage + "% (" + ((Convert.ToDecimal(bytesTransferred) / 1024) / 1024).ToString("00.00") + " MB / " + ((Convert.ToDecimal(totalBytes) / 1024) / 1024).ToString("00.00") + " MB)";
        }

        private void validateEntries()
        {
            isEnable = string.IsNullOrEmpty(_templateName) ? Visibility.Visible : Visibility.Collapsed;
            ProgressText = string.IsNullOrEmpty(_templateName) ? "Please enter Template Name." : "Ready";
        }
    }

    public class WorksheetInfo
    {
        public int? Id { get; set; }

        public string FileLocation { get; set; }

        public string WorksheetName { get; set; }

        public bool HasPreview { get; set; }
    }
}
