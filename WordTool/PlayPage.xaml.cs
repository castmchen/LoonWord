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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WordTool
{
    /// <summary>
    /// Interaction logic for PlayPage.xaml
    /// </summary>
    public partial class PlayPage : Page
    {
        public object MainContent { get; set; }

        public PlayPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("MainWindow.xaml", UriKind.Relative));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //MainWindow mainWindow = new MainWindow();
            //mainWindow.Show();
            this.Content = this.MainContent;
        }
    }
}
