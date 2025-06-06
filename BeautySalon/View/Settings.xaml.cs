using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace BeautySalon.View
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void timeBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[а-яА-ЯA-Za-z \W]$")) { e.Handled = true; }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if(timeBox.Text == null)
            {
                MessageBox.Show("Необходимо ввести время бездействия пользователя \nдля сохранения изменений", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int time = int.Parse(timeBox.Text) * 1000;
            if (MessageBox.Show("Сохранить изменения?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Properties.Settings.Default.blockingTime = time;
                MessageBox.Show("Измения успешно сохранены?");
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            timeBox.Text = (Properties.Settings.Default.blockingTime / 1000).ToString();
        }
    }
}
