    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Application = System.Windows.Application;

namespace BeautySalon.viewBase
{
    public static class InactivityTimer
    {
        private static DispatcherTimer _timer;
        private static TimeSpan _inactivityTime = TimeSpan.FromSeconds(5); // Время бездействия
        private static Action _onInactivity; // Действие при бездействии

        public static void Start(Action onInactivity)
        {
            _onInactivity = onInactivity;

            _timer = new DispatcherTimer
            {
                Interval = _inactivityTime
            };
            _timer.Tick += Timer_Tick;

            RegisterEventHandlers();
            ResetTimer();
        }

        public static void Stop()
        {
            UnregisterEventHandlers();
            _timer?.Stop();
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            // Вызываем действие при бездействии
            _onInactivity?.Invoke();
            Stop(); // Останавливаем таймер после срабатывания
        }

        public static void ResetTimer()
        {
            

                _timer.Stop();


            // Сбрасываем время бездействия
            _inactivityTime = TimeSpan.FromSeconds(5);

            // Устанавливаем новый интервал для таймера
            _timer.Interval = _inactivityTime;

            // Запускаем таймер заново
            _timer.Start();
        }

        private static void RegisterEventHandlers()
        {
            Application.Current.MainWindow.PreviewMouseMove += OnUserActivity;
            Application.Current.MainWindow.PreviewKeyDown += OnUserActivity;
        }

        private static void UnregisterEventHandlers()
        {
            Application.Current.MainWindow.PreviewMouseMove -= OnUserActivity;
            Application.Current.MainWindow.PreviewKeyDown -= OnUserActivity;
        }

        private static void OnUserActivity(object sender, EventArgs e)
        {
            ResetTimer(); // Сбрасываем таймер при активности пользователя
        }
    }
}