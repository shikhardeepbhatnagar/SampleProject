using PitchreadyGlobal.ApplicationConstants;
using PitchreadyGlobal.Enumrations;
using PitchreadyGlobal.Helpers;
using PitchreadyGlobal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PitchreadyGlobal.ViewModels
{
    public class LogoTileVM : PitchreadyGlobalBaseVM, ISupportInitialize
    {
        #region Constructor

        //ILogoWebApi logoApiObj = new LogoWebApi();
        public LogoTileVM()
        {
            Delete_command = new RelayCommand(Handle_Delete_Command, param => true);
            Retry_Command = new RelayCommand(Handle_Retry_Command, param => true);
            CreateCopy_Command = new RelayCommand(Handle_CreateCopy_Command, param => true);
        }
        #endregion


        #region private properties

        private bool _isLoading;
        private string _tooltip = string.Empty;
        private string _loadingText = "Loading...";
        private int srNoText = 1;
        private string strindex = string.Empty;
        private LogoToolCompanyDataFrontEnd logoData;
        private Visibility isRetryCommandVisible = Visibility.Collapsed;
        #endregion

        #region public properties

        public event EventHandler<Microsoft.Exchange.WebServices.Data.NotificationEventArgs> DeleteFromCart;
        public event EventHandler<Microsoft.Exchange.WebServices.Data.NotificationEventArgs> CreateCopy;
        public event EventHandler<Microsoft.Exchange.WebServices.Data.NotificationEventArgs> CloseDialogHandler;
        public ICommand Delete_command { get; set; }
        public ICommand Retry_Command { get; set; }
        public ICommand CreateCopy_Command { get; set; }
        public string Tooltip
        {
            get
            {
                return this._tooltip;
            }
            set
            {
                if (this._tooltip != value)
                {
                    this._tooltip = value;
                }
                this.OnPropertyChanged("Tooltip");
            }
        }

        public bool IsLoading
        {
            get
            {
                return this._isLoading;
            }
            set
            {
                if (this._isLoading != value)
                {
                    this._isLoading = value;
                }
                this.OnPropertyChanged("IsLoading");
            }
        }

        public string LoadingText
        {
            get
            {
                return this._loadingText;
            }
            set
            {
                if (this._loadingText != value)
                {
                    this._loadingText = value;
                }
                this.OnPropertyChanged("LoadingText");
            }
        }

        public LogoToolCompanyDataFrontEnd LogoData
        {
            get
            {
                return logoData;
            }

            set
            {
                logoData = value;
                this.OnPropertyChanged("LogoData");
            }
        }

        public Visibility IsRetryCommandVisible
        {
            get
            {
                return isRetryCommandVisible;
            }

            set
            {
                isRetryCommandVisible = value;
                this.OnPropertyChanged("IsRetryCommandVisible");
            }
        }

        public int SrNoText
        {
            get
            {
                return srNoText;
            }
            set
            {
                srNoText = value;
                this.OnPropertyChanged("SrNoText");
            }
        }

        public string Strindex
        {
            get
            {
                return strindex;
            }

            set
            {
                strindex = value;
                this.OnPropertyChanged("Strindex");
            }
        }

        #endregion

        #region private Methods
        private void Handle_Retry_Command(object obj)
        {
            loadData();
        }

        private void Handle_Delete_Command(object obj)
        {
            DeleteFromCart(this, null);
        }

        private void Handle_CreateCopy_Command(object obj)
        {
            CreateCopy(this, null);
        }


        #endregion

        #region public Methods

        public void loadData()
        {
            try
            {
                int RetryCount = 1;
                bool RetryRemains = true;
                if (IsLoading) return;
                IsRetryCommandVisible = Visibility.Collapsed;
                IsLoading = true;
                Task.Factory.StartNew(async () =>
                {
                    //System.Threading.Thread.Sleep(2000);
                    try
                    {
                        var tempPath = Utils.GetToolTempPath(Tooltype.LogoTool);
                        // var filetoDownload = SelectedData.ActualFileName + "_" + SelectedData.SrNo + ".png";
                        if (LogoData != null && !string.IsNullOrEmpty(LogoData.SystemLogoName))
                        {
                            string filetoDownload = tempPath + LogoData.SystemLogoName;
                            if (!File.Exists(filetoDownload))
                            {
                                //if (GlobalUtilityLibrary.Helpers.ServiceCallHelper.isTokenRefreshCalled)
                                //{
                                //    Stopwatch st = new Stopwatch();
                                //    st.Start();
                                //    while (GlobalUtilityLibrary.Helpers.ServiceCallHelper.isTokenRefreshCalled)
                                //    {
                                //        var time = TimeSpan.FromMilliseconds(st.ElapsedMilliseconds);
                                //        if (time > TimeSpan.FromSeconds(10))
                                //        {
                                //            st.Stop();
                                //            IsLoading = false;
                                //            return;
                                //        }
                                //        System.Threading.Thread.Sleep(500);
                                //    }
                                //}
                                List<string> fileList = new List<string>();
                                fileList.Add(LogoData.SystemLogoName);
                                var result = await ServiceManager.DownloadFilesAndExtract(PitchreadyConstants.getLTCompressedfiles, fileList, Tooltype.LogoTool);
                                if (!File.Exists(filetoDownload))
                                {
                                    while (RetryRemains)
                                    {
                                        if (File.Exists(filetoDownload))
                                        {
                                            RetryRemains = false;
                                            continue;
                                        }
                                        if (RetryCount > 3)
                                            RetryRemains = false;
                                        if (RetryCount < 4)
                                            RetryCount++;
                                        System.Threading.Thread.Sleep(1000);
                                        var res = await ServiceManager.DownloadFilesAndExtract(PitchreadyConstants.getLTCompressedfiles, fileList, Tooltype.LogoTool);
                                    }
                                }
                            }

                            if (!File.Exists(filetoDownload))
                            {
                                IsRetryCommandVisible = Visibility.Visible;
                                IsLoading = false;
                                return;
                            }
                            else
                            {

                                LogoData.LocalLogoPath = filetoDownload;
                            }
                        }
                        else
                        {
                            IsRetryCommandVisible = Visibility.Visible;
                        }
                        IsLoading = false;
                    }
                    catch (Exception ex)
                    {
                        IsLoading = false;
                        IsRetryCommandVisible = Visibility.Visible;
                    }
                });
            }
            catch (Exception ex)
            {
                IsLoading = false;
                IsRetryCommandVisible = Visibility.Visible;
                Utils.LogError("Unable to load template preview " + ex.Message + " " + Utils.CurrentTool + ". Datetime: " + DateTime.Now);
            }
        }
        public void BeginInit()
        {
            //throw new NotImplementedException();
        }

        public void EndInit()
        {
            //throw new NotImplementedException();
        }
        #endregion
    }
}
