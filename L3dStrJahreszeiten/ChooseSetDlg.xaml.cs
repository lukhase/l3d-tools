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
    /// Interaction logic for ChooseSetDlg.xaml
    /// </summary>
    public partial class ChooseSetDlg : Window
    {
        private IEnumerable<ReplacementSet> sets_;

        public ChooseSetDlg(IEnumerable<ReplacementSet> sets)
        {
            sets_ = sets;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lvSets.ItemsSource = sets_;
        }

        public ReplacementSet SelectedSet
        {
            get;
            private set;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            SelectedSet = lvSets.SelectedItem as ReplacementSet;
            if (SelectedSet != null)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
