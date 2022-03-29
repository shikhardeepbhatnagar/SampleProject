using GlobalUtilityLibrary.Helpers;
using PitchreadyGlobal.GlobalCustomControls;
using PitchreadyGlobal.Helpers;
using PitchreadyGlobal.UserControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RelayCommand = PitchreadyGlobal.Helpers.RelayCommand;

namespace PitchreadyGlobal.ViewModels
{
    public class AddEditWindowViewModel : PitchreadyGlobalBaseVM
    {
        public string ActionName;
        public int? ParentId = 0;
        public int? CategoryId;
        public string CategoryName;
        public ParentCategoryModel CategoryData = new ParentCategoryModel();
        private string popUpTitle;
        private bool isArtefactLabelGridVisible;
        private bool isNameLabelGridVisible;
        private string artefactLabelName;
        private string artefactPath;
        private string textBoxName;
        public string previousArtifactName;
        public string previousTextboxName;
        private string addEditButtonName;
        private string labelName;
        private bool isNewFolder;
        private bool isNewCategory;
        private string folderName;
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

        private RelayCommand addEditButtonCommand;
        public RelayCommand AddEditButtonCommand
        {
            get { return addEditButtonCommand; }
            set
            {
                addEditButtonCommand = value;
                OnPropertyChanged("AddEditButtonCommand");
            }
        }

        public string FolderName
        {
            get
            {
                return folderName;
            }

            set
            {
                folderName = value;
                OnPropertyChanged("FolderName");
            }
        }

        public string PopUpTitle
        {
            get
            {
                return popUpTitle;
            }

            set
            {
                popUpTitle = value;
                OnPropertyChanged("PopUpTitle");
            }
        }

        public bool IsArtefactLabelGridVisible
        {
            get
            {
                return isArtefactLabelGridVisible;
            }

            set
            {
                isArtefactLabelGridVisible = value;
                OnPropertyChanged("IsArtefactLabelGridVisible");
            }
        }

        public bool IsNameLabelGridVisible
        {
            get
            {
                return isNameLabelGridVisible;
            }

            set
            {
                isNameLabelGridVisible = value;
                OnPropertyChanged("IsNameLabelGridVisible");
            }
        }

        public bool IsNewFolder
        {
            get
            {
                return isNewFolder;
            }

            set
            {
                isNewFolder = value;
                OnPropertyChanged("IsNewFolder");
            }
        }

        public bool IsNewCategory
        {
            get
            {
                return isNewCategory;
            }

            set
            {
                isNewCategory = value;
                OnPropertyChanged("IsNewCategory");
            }
        }

        public string ArtefactLabelName
        {
            get
            {
                return artefactLabelName;
            }

            set
            {
                artefactLabelName = value;
                OnPropertyChanged("ArtefactLabelName");
            }
        }

        public string ArtefactPath
        {
            get
            {
                return artefactPath;
            }

            set
            {
                artefactPath = value;
                OnPropertyChanged("ArtefactPath");
            }
        }

        public string TextBoxName
        {
            get
            {
                return textBoxName;
            }

            set
            {
                textBoxName = value;
                OnPropertyChanged("TextBoxName");
            }
        }

        public string LabelName
        {
            get
            {
                return labelName;
            }

            set
            {
                labelName = value;
                OnPropertyChanged("LabelName");
            }
        }
        public string AddEditButtonName
        {
            get
            {
                return addEditButtonName;
            }

            set
            {
                addEditButtonName = value;
                OnPropertyChanged("AddEditButtonName");
            }
        }

        private string _filePath = string.Empty;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                OnPropertyChanged("FilePath");
            }
        }

        private string _filePathTag = string.Empty;
        public string FilePathTag
        {
            get { return _filePathTag; }
            set
            {
                _filePathTag = value;
                OnPropertyChanged("FilePathTag");
            }
        }

        ParentCategoryModel obj;

        public AddEditWindowViewModel(ParentCategoryModel obj)
        {
            this.obj = obj;
            Predicate<object> submitPredicate = EnableDisableSubmitButton;
            AddEditButtonCommand = new RelayCommand(AddEditButtonCommandHandler, submitPredicate);
            BrowseCommand = new RelayCommand(BrowseCommandHandler);
            EscCommand = new RelayCommand(EscCommandHandler);
        }

        private bool EnableDisableSubmitButton(object obj)
        {
            if (ActionName == "InsertFolder")
            {
                if (!string.IsNullOrEmpty(TextBoxName))
                    return true;
            }
            if (ActionName == "UpdateFolder")
            {
                if (!string.IsNullOrEmpty(TextBoxName) && previousTextboxName.Trim() != TextBoxName.Trim())
                    return true;
            }
            if (ActionName == "InsertCategory")
            {
                if (!string.IsNullOrEmpty(ArtefactPath) && !string.IsNullOrEmpty(TextBoxName))
                    return true;
            }
            if (ActionName == "UpdateCategory")
            {
                if (!string.IsNullOrEmpty(ArtefactPath) && !string.IsNullOrEmpty(TextBoxName) && (previousArtifactName != ArtefactPath || previousTextboxName != TextBoxName))
                    return true;
            }

            return false;
        }

        private void AddEditButtonCommandHandler(object obj)
        {
            if (ActionName == "InsertFolder")
                AddFolderTRAdmin();
            if (ActionName == "UpdateFolder")
                UpdateFolderTRAdmin();
            if (ActionName == "InsertCategory")
                AddCategoryTRAdmin();
            if (ActionName == "UpdateCategory")
                UpdateCategoryTRAdmin();
        }



        private void EscCommandHandler(object obj)
        {
            this.Close();
        }

        private async void UpdateFolderTRAdmin()
        {
            IsLoading = true;
            LoadingText = "Updating Sub-category...";
            await System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
            });
            var CategoryData = new ChildCategoryModel();
            CategoryData.CategoryName = TextBoxName.Trim();
            CategoryData.CategoryID = CategoryId;
            CategoryData.CategoryParentId = ParentId;
            Dictionary<string, FileInfo> filelist = new Dictionary<string, FileInfo>();

            if (!string.IsNullOrEmpty(FilePathTag))
            {
                filelist.Add(Path.GetFileName(FilePathTag), new FileInfo(FilePathTag));
            }
            var result = await ServiceCallHelper.DMLSaveUpdateData<ChildCategoryModel>(
                       "pitchready/TemplateRepositoryExcel/DMLOperationWithoutFiles", CategoryData,
                       GlobalUtilityLibrary.Enumrations.CRUDType.Update, GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);
            if (result != null)
            {
                TemplateRepositoryAdminViewModel.istreeDataUpdated = true;

                if (result.SatusCode > 0)
                {
                    if (result.SatusCode == 4) //Code 4 is to check duplicacy
                    {
                        CustomMessageBoxExcel.Show("Sub-category cannot be duplicate. Please try again with a unique name.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                    }
                    else
                    {
                        CustomMessageBoxExcel.Show("Sub-category updated successfully.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                    }
                }
                else
                {
                    CustomMessageBoxExcel.Show("Template/Folder data has not been modified, Please reload the form and try again.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                }
            }
            else
            {
                LoadingText = "Loading...";
                IsLoading = false;
                CustomMessageBoxExcel.Show("Sub-category cannot be duplicate. Please try again with a unique name.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                //return;
            }
            Close();
        }

        private async void AddFolderTRAdmin()
        {
            IsLoading = true;
            LoadingText = "Adding Sub-category...";
            await System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
            });
            var CategoryData = new ParentCategoryModel();
            CategoryData.CategoryName = textBoxName.Trim();
            CategoryData.CategoryParentId = ParentId;
            CategoryData.StatementType = "Insert";
            Dictionary<string, FileInfo> filelist = new Dictionary<string, FileInfo>();

            if (!string.IsNullOrEmpty(FilePathTag))
            {
                filelist.Add(Path.GetFileName(FilePathTag), new FileInfo(FilePathTag));
            }
            var result = await ServiceCallHelper.DMLSaveUpdateDataWithFiles<ParentCategoryModel>(
                       "pitchready/TemplateRepositoryExcel/DMLCategoryDetailsTR_EXL", CategoryData,
                       GlobalUtilityLibrary.Enumrations.CRUDType.Insert, filelist, GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);
            if (result != null)
            {
                TemplateRepositoryAdminViewModel.istreeDataUpdated = true;
                if (result.SatusCode > 0)
                {
                    CustomMessageBoxExcel.Show("Sub-category inserted successfully.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                }
                else
                {
                    CustomMessageBoxExcel.Show("Sub-category cannot be duplicate. Please try again with a unique name.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                }
            }
            else
            {
                CustomMessageBoxExcel.Show("Unable to save sub-category name. Please try again.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Warning);
            }
            LoadingText = "Loading...";
            IsLoading = false;
            Close();
        }


        private async void AddCategoryTRAdmin()
        {
            IsLoading = false;
            LoadingText = "Adding Category...";
            await System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
            });
            CategoryData = new ParentCategoryModel();
            CategoryData.CategoryName = TextBoxName.Trim();
            CategoryData.CategoryIconName = FilePathTag;
            CategoryData.Status = true;
            CategoryData.CategoryParentId = null;
            CategoryData.StatementType = "Insert";
            Dictionary<string, FileInfo> filelist = new Dictionary<string, FileInfo>();

            if (!string.IsNullOrEmpty(FilePathTag))
            {
                filelist.Add(Path.GetFileName(FilePathTag), new FileInfo(FilePathTag));
            }
            var result = await ServiceCallHelper.DMLSaveUpdateDataWithFiles<ParentCategoryModel>(
                       "pitchready/TemplateRepositoryExcel/DMLCategoryDetailsTR_EXL", CategoryData,
                       GlobalUtilityLibrary.Enumrations.CRUDType.Insert, filelist, GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);

            if (result != null)
            {
                if (result.SatusCode > 0)
                {
                    TemplateRepositoryAdminViewModel.isValueUpdate = true;
                    TemplateRepositoryAdminViewModel.istreeDataUpdated = true;
                    CustomMessageBoxExcel.Show("Category inserted successfully.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                }
                else
                {
                    CustomMessageBoxExcel.Show("Category cannot be duplicate. Please try again with a unique name.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                }
            }
            else
            {
                CustomMessageBoxExcel.Show("Unable to save catagory name. Please try again.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
            }
            LoadingText = "Loading...";
            IsLoading = false;
            Close();
            //RequestClose += AddCategoryViewModel_RequestClose;
        }

        private async void UpdateCategoryTRAdmin()
        {
            IsLoading = false;
            LoadingText = "Updating Category...";
            await System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
            });
            var CategoryData = new ParentCategoryModel();
            CategoryData.Status = true;
            CategoryData.CategoryName = TextBoxName.Trim();
            CategoryData.Id = CategoryId;
            CategoryData.CategoryParentId = ParentId;
            List<FileInfo> fileInfo = new List<FileInfo>();
            if (!string.IsNullOrEmpty(FilePathTag))
            {
                FileInfo fileinfo1 = new FileInfo(FilePathTag);
                fileInfo = new List<FileInfo>() { fileinfo1 };
            }
            else
            {
                CategoryData.CategoryIconName = ArtefactPath;
            }
            Dictionary<string, FileInfo> filelist = new Dictionary<string, FileInfo>();

            if (!string.IsNullOrEmpty(FilePathTag))
            {
                if (File.Exists(FilePathTag))
                    filelist.Add(Path.GetFileName(FilePathTag), new FileInfo(FilePathTag));
            }
            var result = await ServiceCallHelper.DMLSaveUpdateDataWithFiles<ParentCategoryModel>(
                       "pitchready/TemplateRepositoryExcel/DMLCategoryDetailsTR_EXL", CategoryData,
                       GlobalUtilityLibrary.Enumrations.CRUDType.Update, filelist, GlobalUtilityLibrary.Enumrations.Tooltype.TemplateRepository);
            if (result != null)
            {
                TemplateRepositoryAdminViewModel.isValueUpdate = true;
                if (result.SatusCode > 0)
                {
                    if (TextBoxName.Trim() == CategoryName)
                    {
                        CustomMessageBoxExcel.Show("Category cannot be duplicate. Please try again with a unique name.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                    }
                    else
                    {
                        CustomMessageBoxExcel.Show("Category updated successfully.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Information);
                    }
                }
                else
                {
                    CustomMessageBoxExcel.Show("Template/Category data has been modified, Please reload the form and try again.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Warning);
                }
            }
            else
            {
                LoadingText = "Loading...";
                IsLoading = false;
                CustomMessageBoxExcel.Show("Category cannot be duplicate. Please try again with a unique name.", Constants.TemplateRepositoryTitle, ExcelMessageBoxType.Ok, ExcelMessageBoxImage.Warning);
                //return;
            }
            Close();
            //RequestClose += AddCategoryViewModel_RequestClose;
        }

        private void BrowseCommandHandler(object obj)
        {
            System.Windows.Forms.OpenFileDialog iconFileDialog = new System.Windows.Forms.OpenFileDialog();
            iconFileDialog.Filter = "Image Files | *.png; *.jpeg; *.thumb";
            iconFileDialog.Multiselect = false;
            if (iconFileDialog.ShowDialog() == DialogResult.OK)
            {
                FilePath = iconFileDialog.FileName;
                FilePathTag = iconFileDialog.FileName;
                ArtefactPath = iconFileDialog.FileName;
            }
        }
    }
}
