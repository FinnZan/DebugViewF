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

namespace FinnZan.Utilities
{
    /// <summary>
    /// Interaction logic for AppNameDialog.xaml
    /// </summary>
    public partial class AppNameDialog : Window
    {
        public AppNameDialog()
        {
            InitializeComponent();
        }

        public string AppName
        {
            get;
            private set;
        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            AppName = tbAppName.Text;
            this.DialogResult = true;  
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
