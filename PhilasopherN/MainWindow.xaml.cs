using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhilasopherN
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        MainClass mc;
        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            mc = new MainClass();
            Name.Text += mc.ph.Name;
            if (Name.Text.Contains("1"))
            {
                PHil.Opacity = 1;
                PHi2.Opacity = 0;
                PHi3.Opacity = 0;
                PHi4.Opacity = 0;
                PHi5.Opacity = 0;
            }
            if (Name.Text.Contains("2"))
            {
                PHil.Opacity = 0;
                PHi2.Opacity = 1;
                PHi3.Opacity = 0;
                PHi4.Opacity = 0;
                PHi5.Opacity = 0;
            }
            if (Name.Text.Contains("3"))
            {
                PHil.Opacity = 0;
                PHi2.Opacity = 0;
                PHi3.Opacity = 1;
                PHi4.Opacity = 0;
                PHi5.Opacity = 0;
            }
            if (Name.Text.Contains("4"))
            {
                PHil.Opacity = 0;
                PHi2.Opacity = 0;
                PHi3.Opacity = 0;
                PHi4.Opacity = 1;
                PHi5.Opacity = 0;
            }
            if (Name.Text.Contains("5"))
            {
                PHil.Opacity = 0;
                PHi2.Opacity = 0;
                PHi3.Opacity = 0;
                PHi4.Opacity = 0;
                PHi5.Opacity = 1;
            }
            mc.UIStatus += (phs) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    Statustxt.Text = phs.ToString();
                    if (phs == PhilosopherStatus.eating)
                    {
                        MainGrid.Background = Brushes.Green;
                        statuseat.Opacity = 1;
                        statusthink.Opacity = 0;
                        statuswait.Opacity = 0;
                    }
                    if (phs == PhilosopherStatus.waiting)
                    {
                        MainGrid.Background = Brushes.Red;
                        statuseat.Opacity = 0;
                        statusthink.Opacity = 0;
                        statuswait.Opacity = 1;
                    }
                    if (phs == PhilosopherStatus.thinking)
                    {
                        MainGrid.Background = Brushes.Blue;
                        statuseat.Opacity = 0;
                        statusthink.Opacity = 1;
                        statuswait.Opacity = 0;
                    }
                }));

            };
            mc.OnForwardChanged += (phs) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    if (phs == PhilosopherStatus.eating) Forward.Background = Brushes.Green;
                    if (phs == PhilosopherStatus.waiting) Forward.Background = Brushes.Red;
                    if (phs == PhilosopherStatus.thinking) Forward.Background = Brushes.Blue;
                }));
            };
            mc.OnBackwardChanged += (phs) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    if (phs == PhilosopherStatus.eating) Backward.Background = Brushes.Green;
                    if (phs == PhilosopherStatus.waiting) Backward.Background = Brushes.Red;
                    if (phs == PhilosopherStatus.thinking) Backward.Background = Brushes.Blue;
                }));
            };
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string[] lines = File.ReadAllLines("config.txt");
            int index = 0;
            foreach (var item in lines)
            {
                if (item.Contains("R"))
                {
                    lines[index] = lines[index].Replace(";R", "");
                }
                index++;
            }
            File.WriteAllLines("config.txt", lines);
            mc.Forward.Send("Stop");
            Application.Current.Shutdown();
        }
    }
}
