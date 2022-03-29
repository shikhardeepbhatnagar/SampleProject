using PitchreadyGlobal.Enumrations;
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

namespace PitchreadyGlobal.UserControls
{
    /// <summary>
    /// Interaction logic for CustomMessageBoxExcelExcel.xaml
    /// </summary>
    public partial class CustomMessageBoxExcel : Window
    {
        public ExcelCustomMessageResult messageBoxResult = ExcelCustomMessageResult.None;
        public string message = string.Empty;
        public string title = string.Empty;
        ExcelMessageBoxType objExcelMessageBoxType = ExcelMessageBoxType.Ok;
        ExcelMessageBoxImage? objExcelMessageBoxImage = null;
        public CustomMessageBoxExcel(string Message, string Title, ExcelMessageBoxType ExcelMessageBoxType, ExcelMessageBoxImage? ExcelMessageBoxImage = null, ProjectTypeEnum projectTypeEnum = ProjectTypeEnum.Excel)
        {
            InitializeComponent();
            message = Message;
            title = Title;
            objExcelMessageBoxType = ExcelMessageBoxType;
            objExcelMessageBoxImage = ExcelMessageBoxImage;

            if(projectTypeEnum == ProjectTypeEnum.Word)
            {
                txtTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A579A"));
                btnOk.Style = (Style)FindResource("WordButtonStylePrimary");
                btnYes.Style = (Style)FindResource("WordButtonStylePrimary");
                btnRetry.Style = (Style)FindResource("WordButtonStylePrimary");
                btnNo.Style = (Style)FindResource("WordButtonStyleSecondary");
                btnCancel.Style = (Style)FindResource("WordButtonStyleSecondary");
                btnSkip.Style = (Style)FindResource("WordButtonStyleSecondary");
            }
        }

        public static bool Show(string Message, string Title, ExcelMessageBoxType ExcelMessageBoxType, ExcelMessageBoxImage? ExcelMessageBoxImage = null, ProjectTypeEnum projectTypeEnum = ProjectTypeEnum.Excel)
        {
            var windowObj = new CustomMessageBoxExcel(Message, Title, ExcelMessageBoxType, ExcelMessageBoxImage, projectTypeEnum) { Topmost = true }.ShowDialog();
            var dialogResult = windowObj != null ? (bool)windowObj : false;
            return dialogResult;
        }

        public static ExcelCustomMessageResult ShowWithMessageBoxResult(string Message, string Title, ExcelMessageBoxType ExcelMessageBoxType, ExcelMessageBoxImage? ExcelMessageBoxImage = null)
        {
            var windowObj = new CustomMessageBoxExcel(Message, Title, ExcelMessageBoxType, ExcelMessageBoxImage) { Topmost = true };
            var dialogRes = windowObj.ShowDialog();
            var dialogResult = dialogRes != null ? (bool)dialogRes : false;
            return windowObj.messageBoxResult;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtMessage.Text = message;
            txtTitle.Text = title;
            Handlebuttons(objExcelMessageBoxType);
            if (objExcelMessageBoxImage.HasValue)
                HandleIcons((ExcelMessageBoxImage)objExcelMessageBoxImage);


            if (btnYes.Visibility == Visibility.Visible)
                btnYes.Focus();
            else if (btnOk.Visibility == Visibility.Visible)
                btnOk.Focus();
            else if (btnRetry.Visibility == Visibility.Visible)
                btnRetry.Focus();
        }

        private void HandleIcons(ExcelMessageBoxImage objExcelMessageBoxImage)
        {
            try
            {
                switch (objExcelMessageBoxImage)
                {
                    case ExcelMessageBoxImage.Warning:
                        imgIcon.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                 System.Drawing.SystemIcons.Warning.Handle,
                                 Int32Rect.Empty,
                                 BitmapSizeOptions.FromEmptyOptions());
                        break;
                    case ExcelMessageBoxImage.Question:
                        imgIcon.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                 System.Drawing.SystemIcons.Question.Handle,
                                 Int32Rect.Empty,
                                 BitmapSizeOptions.FromEmptyOptions());
                        break;
                    case ExcelMessageBoxImage.Information:
                        imgIcon.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                System.Drawing.SystemIcons.Information.Handle,
                                Int32Rect.Empty,
                                BitmapSizeOptions.FromEmptyOptions());
                        break;
                    case ExcelMessageBoxImage.Error:
                        imgIcon.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                 System.Drawing.SystemIcons.Error.Handle,
                                 Int32Rect.Empty,
                                 BitmapSizeOptions.FromEmptyOptions());
                        break;
                    case ExcelMessageBoxImage.Retry:
                        imgIcon.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                System.Drawing.SystemIcons.Exclamation.Handle,
                                Int32Rect.Empty,
                                BitmapSizeOptions.FromEmptyOptions());
                        break;
                    case ExcelMessageBoxImage.None:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void Handlebuttons(ExcelMessageBoxType objExcelMessageBoxType)
        {
            try
            {
                switch (objExcelMessageBoxType)
                {
                    case ExcelMessageBoxType.Ok:
                        btnOk.Visibility = Visibility.Visible;
                        break;
                    case ExcelMessageBoxType.ConfirmationWithOkCancel:
                        btnOk.Visibility = Visibility.Visible;
                        btnCancel.Visibility = Visibility.Visible;
                        break;
                    case ExcelMessageBoxType.ConfirmationWithYesNo:
                        btnYes.Visibility = Visibility.Visible;
                        btnNo.Visibility = Visibility.Visible;
                        break;
                    case ExcelMessageBoxType.ConfirmationWithYesNoCancel:
                        btnYes.Visibility = Visibility.Visible;
                        btnNo.Visibility = Visibility.Visible;
                        btnCancel.Visibility = Visibility.Visible;
                        break;
                    case ExcelMessageBoxType.Retry:
                        btnRetry.Visibility = Visibility.Visible;
                        break;
                    case ExcelMessageBoxType.ConfirmationWithYesNoSkip:
                        btnYes.Visibility = Visibility.Visible;
                        btnNo.Visibility = Visibility.Visible;
                        btnSkip.Visibility = Visibility.Visible;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
            if (e.Key == Key.Enter)
            {
                if (btnYes.Visibility == Visibility.Visible)
                    btnYes_Click(null, null);
                else if (btnOk.Visibility == Visibility.Visible)
                    btnok_Click(null, null);
                else if (btnRetry.Visibility == Visibility.Visible)
                    btnRetry_Click(null, null);
            }
        }

        //private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    messageBoxResult = ExcelCustomMessageResult.Cancel;
        //    this.Close();
        //}

        private void btnok_Click(object sender, RoutedEventArgs e)
        {
            messageBoxResult = ExcelCustomMessageResult.Ok;
            DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            messageBoxResult = ExcelCustomMessageResult.Cancel;
            DialogResult = false;
            this.Close();
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            messageBoxResult = ExcelCustomMessageResult.Yes;
            DialogResult = true;
            this.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            messageBoxResult = ExcelCustomMessageResult.No;
            DialogResult = false;
            this.Close();
        }

        private void btnRetry_Click(object sender, RoutedEventArgs e)
        {
            messageBoxResult = ExcelCustomMessageResult.Retry;
            DialogResult = true;
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            messageBoxResult = ExcelCustomMessageResult.Cancel;
            this.Close();
        }

        private void btnSkip_Click(object sender, RoutedEventArgs e)
        {
            messageBoxResult = ExcelCustomMessageResult.Skip;
            DialogResult = false;
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            messageBoxResult = ExcelCustomMessageResult.Cancel;
            this.Close();
        }
    }

    public enum ExcelCustomMessageResult
    {
        Ok,
        Yes,
        No,
        Cancel,
        Retry,
        None,
        Skip
    }

    public enum ExcelMessageBoxType
    {
        Ok,
        ConfirmationWithOkCancel,
        ConfirmationWithYesNo,
        ConfirmationWithYesNoCancel,
        Retry,
        ConfirmationWithYesNoSkip
    }

    public enum ExcelMessageBoxImage
    {
        Warning = 0,
        Question,
        Information,
        Error,
        Retry,
        None
    }
}
