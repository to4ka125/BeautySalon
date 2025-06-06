using BeautySalon.View;
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

namespace BeautySalon.Forms
{
    /// <summary>
    /// Логика взаимодействия для SisAdminWorkTable.xaml
    /// </summary>
    public partial class SisAdminWorkTable : Window
    {
        public SisAdminWorkTable()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вернуться на форму авторизации?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
        private void Orders_Click(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            StackPanelActive.Children.Clear();
            switch (radioButton.Name){
                case "import":
                    Import import = new Import();
                    StackPanelActive.Children.Add(import);
                    break;
                case "restoringStructure":
                    RestoringStructure restoringStructure = new RestoringStructure();
                    StackPanelActive.Children.Add(restoringStructure);
                    break;
                case "Settings":
                    Settings s = new Settings();
                    StackPanelActive.Children.Add(s);
                    break;  
                case "export":
                    Export ex = new Export();
                    StackPanelActive.Children.Add(ex);
                    break; 
                case "BackupRecovery":
                    BackupRecovery1 b = new BackupRecovery1();
                    StackPanelActive.Children.Add(b);
                    break;
            }
        }

        private void Import_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
