using PitchreadyGlobal.Helpers;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PitchreadyGlobal.ViewModels
{
    public class PitchreadyGlobalBaseVM : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _isLoading = false;
        private string _loadingText = "Loading...";
        
        private RelayCommand escCommand;

        public RelayCommand EscCommand
        {
            get { return escCommand; }
            set
            {
                escCommand = value;
                OnPropertyChanged("EscCommand");
            }
        }

        

        public event Action RequestClose;
        public virtual void Close()
        {
            RequestClose?.Invoke();
        }

        public event Action RequestReactivateForm;
        public virtual void ReactivateForm()
        {
            RequestReactivateForm?.Invoke();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void Dispose()
        {
            // ...
        }

        public bool IsLoading
        {
            get
            {
                return this._isLoading;
            }
            set
            {
                if (this._isLoading == value) return;
                this._isLoading = value;
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
    }
}
