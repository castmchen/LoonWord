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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WordTool
{
    /// <summary>
    /// Interaction logic for HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        public HomeWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation DAnimation = new DoubleAnimation();
            DAnimation.From = 54; //起点
            DAnimation.To = 750; //终点
            DAnimation.Duration = new Duration(TimeSpan.FromSeconds(3)); //时间

            Storyboard.SetTarget(DAnimation, GroupboxArea);
            Storyboard.SetTargetProperty(DAnimation, new PropertyPath(Canvas.LeftProperty));
            Storyboard story = new Storyboard();

            story.Completed += new EventHandler(RedirectToMainWindow); //完成后要做的事
            //story.RepeatBehavior = RepeatBehavior.Forever; //无限次循环，需要的自己加上
            story.Children.Add(DAnimation);
            story.Begin();
        }

        private void RedirectToMainWindow(object sender, EventArgs e)
        {
            //this.MainWindow = new MainWindow().Content;
            //this.Content = this.MainWindow;
            MainWindow mainWindow = new MainWindow();
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //mainWindow.Left = 200;
            //mainWindow.Top = 200;
            mainWindow.Show();
            this.Close();
        }
    }
}
