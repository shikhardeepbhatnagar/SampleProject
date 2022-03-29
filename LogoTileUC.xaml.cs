using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PitchreadyGlobal.UserControls
{
    /// <summary>
    /// Interaction logic for LogoTileUC.xaml
    /// </summary>
    public partial class LogoTileUC : UserControl
    {
        public LogoTileUC()
        {
            InitializeComponent();
        }

        private void logoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.logoBorder.BorderBrush = Brushes.Red;
        }

        private void logoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.logoBorder.BorderBrush = Brushes.Gray;
        }

        private void BoxView_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
