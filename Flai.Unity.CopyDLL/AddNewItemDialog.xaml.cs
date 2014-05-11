using System.Windows;

namespace Flai.Unity.CopyDLL
{
    /// <summary>
    /// Interaction logic for AddNewItemDialog.xaml
    /// </summary>
    public partial class AddNewItemDialog : Window
    {
        public string EnteredName
        {
            get { return _nameTextBox.Text; }
        }

        public AddNewItemDialog()
        {
            InitializeComponent();
        }

        private void OnOKClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
