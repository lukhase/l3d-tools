using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace L3dStrJahreszeiten
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbPath.Text = RegistryAccess.GetLoksimDataDir();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            string dir = tbPath.Text.TrimEnd('\\');
            L3dFilePath.LoksimDirectory = new L3dFilePath(dir);
            Close();
        }
    }
}
