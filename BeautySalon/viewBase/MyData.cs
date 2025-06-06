using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BeautySalon.viewBase
{
    /// <summary>
    /// класс для хранения глобальных данных
    /// </summary>
    static public class MyData
    {
        static public DispatcherTimer idleTimer;
        static public string role ="Админ";
        static public string name="Капотова Мария";
        static public string id;
        static public string clients_id;
        static public string orders_id;
        static public string products_id;
        static public string employee_id;
        static public string service_id;
        static public string schedules_id;
        static public int countCaptcha = 0;
        static public bool view= false;

        static public string employee_idLoaded;
    }
}
