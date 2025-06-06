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
using BeautySalon.viewBase;
using BeautySalon.View;
using System.Windows.Threading;

namespace BeautySalon.Forms
{
    /// <summary>
    /// Interaction logic for workTable.xaml
    /// </summary>
    public partial class workTable : Window
    {
        private int idleTimeLimit;
        public workTable()
        {
             InitializeComponent(); InactivityTimer.Start(() =>
             {
                 // Здесь можно добавить логику возврата на форму авторизации
                 MessageBox.Show("Вы не были активны 30 секунд. Вы будете перенаправлены на страницу авторизации.");
                 // Например, открываем окно авторизации
                 this.Close(); // Закрываем текущее окно (если нужно)
             });
            //      InitializeIdleTimer();
            //         this.MouseMove += new MouseEventHandler(Window_MouseMove);
            //      this.KeyDown += new KeyEventHandler(Window_KeyDown); // Дополнительно сбрасываем таймер при нажатии клавиш
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            InactivityTimer.Stop();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // ResetIdleTimer();

            InactivityTimer.ResetTimer();
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            InactivityTimer.ResetTimer();
            // ResetIdleTimer();

        }
        /*   private void ResetIdleTimer()
           {
               // Сбрасываем таймер при активности
               if (MyData.idleTimer.IsEnabled)
               {
                   MyData.idleTimer.Stop();
                   MyData.idleTimer.Start();
               }
          }*/
        private void InitializeIdleTimer()
        {
            // Устанавливаем время бездействия (например, 30 секунд)
            idleTimeLimit = Properties.Settings.Default.blockingTime;

            MyData.idleTimer = new DispatcherTimer();
            MyData.idleTimer.Interval = TimeSpan.FromMilliseconds(idleTimeLimit);
            MyData.idleTimer.Tick += IdleTimer_Tick;
            MyData.idleTimer.Start();
        }
        private void IdleTimer_Tick(object sender, EventArgs e)
        {
            // Блокируем систему и перенаправляем на форму авторизации
            MyData.idleTimer.Stop();
            this.IsEnabled = false;
            MessageBox.Show("Система заблокирована из-за отсутствия активности. Пожалуйста, войдите снова.", "Блокировка системы", MessageBoxButton.OK, MessageBoxImage.Warning);
            this.Close();
        }   
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вернуться на форму авторизации?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            role.Content = MyData.role;
            Name.Content = MyData.name;

            switch (MyData.role) {

                case "Администратор":
                    Materials.Visibility = Visibility.Collapsed;
                    Employees.Visibility = Visibility.Collapsed;
                    break;

                case "Мастер":
                    Employees.Visibility = Visibility.Collapsed;
                    Clients.Visibility = Visibility.Collapsed;
                    Services.Visibility = Visibility.Collapsed;
                    Materials.Visibility = Visibility.Collapsed;
                    Products.Visibility = Visibility.Collapsed;
                    Certificates.Visibility = Visibility.Collapsed;
                    break;

                case "Директор":
                    /**/
                    break;

                case "Менеджер":
                    Employees.Visibility = Visibility.Collapsed;
                    Clients.Visibility = Visibility.Collapsed;
                    Orders.Visibility = Visibility.Collapsed;
                    Schedules.Visibility = Visibility.Collapsed;
                    Services.Visibility = Visibility.Collapsed;
                    Certificates.Visibility = Visibility.Collapsed;
                    break;
            }
        }
        private void Employees_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            StackPanelActive.Children.Clear();
            switch (button.Name)
            {
                case "Employees":
                    Employees employees = new Employees();
                    StackPanelActive.Children.Add(employees);
                    break;
                case "Materials":
                    Materials materials = new Materials();
                    StackPanelActive.Children.Add(materials);
                    break;
                case "Orders":
                    Orders orders = new Orders();
                    StackPanelActive.Children.Add(orders);
                    break;
                case "Services":
                    Services services = new Services();
                    StackPanelActive.Children.Add(services);
                    break;
                case "Products":
                    Product product = new Product();
                    StackPanelActive.Children.Add(product);
                    break;
                case "Clients":
                    Clients Clients = new Clients();
                    StackPanelActive.Children.Add(Clients);
                    break;
                case "Schedules":
                    Schedules schedules = new Schedules();
                    StackPanelActive.Children.Add(schedules);
                    break;
                case "Certificates":
                    Certificates1 certificates = new Certificates1();
                    StackPanelActive.Children.Add(certificates);
                    break;
            }
        }
    }
}