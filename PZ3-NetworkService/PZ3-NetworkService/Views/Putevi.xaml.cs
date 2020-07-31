using PZ3_NetworkService.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace PZ3_NetworkService.Views
{
    /// <summary>
    /// Interaction logic for Putevi.xaml
    /// </summary>
    public partial class Putevi : Window
    {
        public Putevi()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new PuteviViewModel();
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsValid(((TextBox)sender).Text + e.Text);
        }

        public static bool IsValid(string str)
        {
            int i;
            return int.TryParse(str, out i) && i >= 5 && i <= 9999;
        }
    }
}
