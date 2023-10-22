using Autodesk.Revit.UI;
using Microsoft.Win32;
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


namespace RAA_Level2
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class MyForm : Window
    {
        public MyForm()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = @"C:\";
            openFile.Filter = "csv files (*.csv)|*.csv";

            if(openFile.ShowDialog() == true)
            {
                tbxFilePath.Text = openFile.FileName;
            }
            else
            {
                tbxFilePath.Text = "";
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        public string GetTextBoxValue()
        { 
            return tbxFilePath.Text; 
        }

        public string GetRadioButtonValue()
        {
            if (rbImperial.IsChecked == true)
                return rbImperial.Content.ToString();
            else 
                return rbMetric.Content.ToString();
        }

        public bool GetCheckboxFloorPlans()
        {
            if (chbxFloorPlans.IsChecked == true)
                return true;
            else
                return false;
        }

        public bool GetCheckboxCeilingPlans()
        {
            if (chbxCeilingPlans.IsChecked == true)
                return true;
            else
                return false;
        }
    }
}
