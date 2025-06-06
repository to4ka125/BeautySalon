using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
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
using MySql.Data.MySqlClient;

namespace BeautySalon.Forms
{
    /// <summary>
    /// Interaction logic for OrdersAdd.xaml
    /// </summary>
    public partial class OrdersAdd : Window
    {
        public DateTime DisplayStartDate => DateTime.Today;
        public DateTime DisplayEndDate => DateTime.Today.AddMonths(6);
        public OrdersAdd()
        {
            InitializeComponent();
        }


        private int DurationTime(string servicesId)
        {
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();

                MySqlCommand cmd = new MySqlCommand($@"SELECT duration FROM vkr.services where service_id = '{servicesId}'", con);

                int duration = int.Parse(cmd.ExecuteScalar().ToString());
                return duration;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Master.SelectedItem == null || Time.SelectedItem == null ||
                datePicker1.SelectedDate == null)
            {
                MessageBox.Show("Выберите мастера, дату и время");
                return;
            }

            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                try
                {
                    con.Open();

                    DateTime selectedData = datePicker1.SelectedDate.Value;
                    string data = selectedData.ToString("yyyy-MM-dd");
                    string time = Time.Text;
                    string dataTime = $"{data} {time}";
                    string client_id = client_idGet(Client.Text);

                    string services_id = services_idGet(Services.Text);

                    string master_id = master_idGet(Master.Text);
                    string status = "На исполнении";


                    int duration = DurationTime(services_id);

                    if (!DateTime.TryParseExact(dataTime,
                        "yyyy-MM-dd HH:mm",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                         out DateTime startTime))
                    {
                        MessageBox.Show("Неверный формат даты или времени");
                    }

                    string endTime = startTime.AddMinutes(duration).ToString("yyyy-MM-dd");


                    List<int> MaterialsQuery = new List<int>();
                    List<int> Quantity = new List<int>();
                    // Создание команды для проверки количества материалов на складе, связанных с определенной услугой

                    MySqlCommand cmdCheck = new MySqlCommand($@"SELECT COUNT(*) FROM recording 
                                WHERE employee_id = '{master_id}' 
                                AND recording_datetime = '{dataTime}'", con);

                    int count = Convert.ToInt32(cmdCheck.ExecuteScalar());

                    if (count > 0)
                    {
                        MessageBox.Show("Это время уже занято");
                        return;
                    }

                    using (MySqlCommand checkMaterialsQuery = new MySqlCommand($@"
                        SELECT quantity_in_stock
                        FROM Materials m
                        INNER JOIN Service_Materials s ON m.material_id = s.material_id
                        WHERE s.service_id = '{services_id}'", con))
                    {
                        using (MySqlDataReader dr = checkMaterialsQuery.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                MaterialsQuery.Add(dr.GetInt32(0));
                            }
                        }
                    }
                    // Создание команды для проверки использованного количества материалов для определенной услуги
                    using (MySqlCommand checkQuantity_usedQuery = new MySqlCommand($@"
                        SELECT quantity_used
                        FROM VKR.Service_Materials
                        WHERE service_id = '{services_id}'", con))
                    {

                        using (MySqlDataReader dr2 = checkQuantity_usedQuery.ExecuteReader())
                        {
                            while (dr2.Read())
                            {
                                Quantity.Add(dr2.GetInt32(0));
                            }
                        }
                    }
                    for (int i = 0; i < MaterialsQuery.Count; i++)
                    {
                        // Если количество материалов на складе минус использованное количество меньше 0,
                        // то выводим сообщение об ошибке и выходим из метода
                        if (MaterialsQuery[i] - Quantity[i] < 0)
                        {
                            MessageBox.Show("На складе недостаточно продуктов");
                            return;
                        }
                    }

                    // Создание команды для вставки новой записи о выполненной услуге в таблицу Recording
                    using (MySqlCommand cmd = new MySqlCommand($@"
                            INSERT INTO VKR.Recording (client_id, service_id, recording_datetime, employee_id, recording_enddatetimel, status)
                            VALUES ('{client_id}','{services_id}', '{dataTime}','{master_id}','{endTime}' ,'{status}')", con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    // Создание команды для обновления количества материалов на складе после выполнения услуги
                    using (MySqlCommand cmd2 = new MySqlCommand($@"
                                UPDATE Materials m
                                JOIN Service_Materials s ON m.material_id = s.material_id
                                SET m.quantity_in_stock = m.quantity_in_stock - s.quantity_used
                                WHERE s.service_id = '{services_id}'", con))
                    {
                        cmd2.ExecuteNonQuery();
                    }

                    MessageBox.Show("Запись сделана");

                    Time.Text = "";
                    Client.Text = "";
                    Master.Text = "";
                    datePicker1.Text = "";
                    Services.Text = "";

                    Master_SelectionChanged(null, null);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка-" + ex.Message);
                }
            }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                try
                {
                    con.Open();
                    MySqlCommand cmd1 = new MySqlCommand("Select * From Clients", con);

                    MySqlDataReader dr1 = cmd1.ExecuteReader();

                    while (dr1.Read())
                    {
                        Client.Items.Add($"{dr1[1]} {dr1[2]}");
                    }
                    con.Close();

                    con.Open();
                    MySqlCommand cmd2 = new MySqlCommand("Select * From Services", con);

                    MySqlDataReader dr2 = cmd2.ExecuteReader();
                    while (dr2.Read())
                    {
                        Services.Items.Add($"{dr2[1]}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка-" + ex.Message);
                }
            }
        }

        private string client_idGet(string name)
        {
            string id;
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();

                MySqlCommand cmd = new MySqlCommand($@"SELECT client_id FROM vkr.clients 
                where concat_ws(' ',first_name,last_name) ='{name}'", con);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                id = dt.Rows[0][0].ToString();
            }
            return id;
        }

        private string services_idGet(string name)
        {
            string id;
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();

                MySqlCommand cmd = new MySqlCommand($@"SELECT service_id FROM vkr.services 
                where service_name= '{name}';", con);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                id = dt.Rows[0][0].ToString();
            }
            return id;
        }

        private string master_idGet(string name)
        {
            string id;
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();

                MySqlCommand cmd = new MySqlCommand($@"SELECT employee_id FROM vkr.employees 
                where concat_ws(' ',first_name,last_name,patronymic) = '{name}';", con);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                id = dt.Rows[0][0].ToString();
            }
            return id;
        }

        private void datePicker1_CalendarOpened(object sender, RoutedEventArgs e)
        {
            if (datePicker1 != null)
            {
                DateTime minDate = DateTime.Now;
                DateTime maxDate = DateTime.Now.AddMonths(1);

                if (datePicker1.SelectedDate < minDate || datePicker1.SelectedDate > maxDate)
                {
                    datePicker1.SelectedDate = null;
                }
            }
        }

        private void datePicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;

            if (datePicker != null && datePicker.SelectedDate.HasValue)
            {
                DateTime selectedDate = datePicker.SelectedDate.Value;

                // Устанавливаем дату в DatePicker, если она не меньше текущей даты
                if (selectedDate < DateTime.Today)
                {
                    datePicker.SelectedDate = DateTime.Today;
                }
            }
        }

        private void Services_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Services.SelectedItem == null)
            {
                MessageBox.Show("Выберите услугу");
                return;
            }

            string selectedService = Services.SelectedItem.ToString();


            string idServices = services_idGet(selectedService);

            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand($@"SELECT concat_ws(' ',e.first_name, e.last_name, e.patronymic) FROM vkr.employees e
                    inner join specialization s on s.idspecialization = e.idSpecialization
                    inner join specializationcategory sc on sc.idservicesCategory = s.idspecialization
                    inner join servicescategory scat on scat.idservicesCategory = sc.idservicesCategory
                    inner join services ser on ser.idservicesCategory = scat.idservicesCategory
                    where ser.service_id = '{idServices}'
                    ;", con);
                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Master.Items.Add($"{dr.GetValue(0)}");
                }
            }
        }

        private List<DateTime> _availableTimes = new List<DateTime>();
        private HashSet<DateTime> _selectedTimes = new HashSet<DateTime>();
        private void Master_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Очищаем предыдущие данные
            Time.Items.Clear();
            _selectedTimes.Clear();
            _availableTimes.Clear();

            // Проверяем, выбран ли мастер
            if (Master.SelectedItem == null)
            {
                return;
            }

            try
            {
                // Получаем ID мастера из выбранного элемента
                string selectedMaster = Master.SelectedItem.ToString();

                string masterId = master_idGet(selectedMaster);

                // Проверяем, выбрана ли дата
                if (datePicker1.SelectedDate == null)
                {
                    MessageBox.Show("Выберите дату");
                    return;
                }

                DateTime selectedDate = datePicker1.SelectedDate.Value;

                // Получаем продолжительность услуги
                int duration = DurationTime(services_idGet(Services.Text));

                // Генерируем доступные временные слоты
                List<DateTime> availableSlots = GenerateAvailableTimeSlots(selectedDate, masterId, duration);

                // Добавляем слоты в выпадающий список
                MySqlConnection connection = new MySqlConnection(SqlConnection.connectionString);
                connection.Open();
                MySqlCommand command = new MySqlCommand($"SELECT recording_datetime FROM VKR.recording where recording_datetime between '{datePicker1.SelectedDate.Value.ToString("yyyy-MM-dd")} 08:00:00' and '{datePicker1.SelectedDate.Value.ToString("yyyy-MM-dd")} 18:00:00' and employee_id ={masterId} and status != 'Отменен'",connection);
                MySqlDataReader mySqlDataReader = command.ExecuteReader();
                List<String> s = new List<string> { };
                s.Clear();
                while (mySqlDataReader.Read())
                {
                    string value = mySqlDataReader[0].ToString();
                    s.Add(value.Substring(11));
                }
                connection.Close();
                Time.Items.Clear();

                foreach (var slot in availableSlots)
                {
                    bool isDuplicate = false;
                    DateTime currentTime = DateTime.Now;
                    foreach (var time in s)
                    {
                        if (time.ToString() == slot.ToString("HH:mm:ss"))
                        {
                            
                            isDuplicate = true;
                            break; // Если нашли совпадение, выходим из внутреннего цикла
                        }
                    }

                    if (!isDuplicate)
                    {
                        // Добавляем только если не нашли дубликат
                        if (datePicker1.SelectedDate.Value.ToString("yyyy-MM-dd") == currentTime.Date.ToString("yyyy-MM-dd"))
                        {
                            if (slot.TimeOfDay >= currentTime.TimeOfDay)
                            {
                                Time.Items.Add(slot.ToString("HH:mm"));
                                _availableTimes.Add(slot);
                            }
                        }
                        else
                        {
                            Time.Items.Add(slot.ToString("HH:mm"));
                            _availableTimes.Add(slot);
                        }
                        
                            
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }

        }

        private List<DateTime> GenerateAvailableTimeSlots(DateTime date, string masterId, int serviceDuration)
        {
            List<DateTime> availableSlots = new List<DateTime>();

            // 1. Получаем рабочее время мастера на выбранную дату
            var workingHours = GetMasterWorkingHours(masterId, date.DayOfWeek);

            if (workingHours == null || !workingHours.IsWorking)
            {
                return availableSlots; // Мастер не работает в этот день
            }

            // 2. Получаем уже занятые слоты
            var bookedSlots = GetBookedTimeSlots(masterId, date);

            // 3. Генерируем возможные слоты с учетом продолжительности услуги
            DateTime currentSlotStart = date.Date.Add(workingHours.StartTime);
            DateTime endTime = date.Date.Add(workingHours.EndTime);

            while (currentSlotStart.AddMinutes(serviceDuration) <= endTime)
            {
                // Проверяем, не пересекается ли текущий слот с занятыми
                bool isAvailable = true;
                DateTime currentSlotEnd = currentSlotStart.AddMinutes(serviceDuration);

                foreach (var bookedSlot in bookedSlots)
                {
                    if (currentSlotStart < bookedSlot.EndTime && currentSlotEnd > bookedSlot.StartTime)
                    {
                        isAvailable = false;
                        break;
                    }
                }

                if (isAvailable)
                {
                    availableSlots.Add(currentSlotStart);
                }

                // Переходим к следующему слоту (можно добавить шаг, например, 30 минут)
                currentSlotStart = currentSlotStart.AddMinutes(30);
            }

            return availableSlots;
        }

        public class WorkingHours
        {
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public bool IsWorking { get; set; }
        }

        // Метод для получения рабочего времени мастера (примерная реализация)
        private WorkingHours GetMasterWorkingHours(string masterId, DayOfWeek dayOfWeek)
        {
            // Здесь должна быть логика получения рабочего времени мастера из БД
            // Пример:
            return new WorkingHours
            {
                StartTime = new TimeSpan(9, 0, 0), // 9:00
                EndTime = new TimeSpan(18, 0, 0),   // 18:00
                IsWorking = true
            };
        }

        // Класс для хранения информации о занятых слотах
        public class BookedSlot
        {
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }

        // Метод для получения занятых слотов (примерная реализация)
        private List<BookedSlot> GetBookedTimeSlots(string masterId, DateTime date)
        {
            // Здесь должна быть логика получения занятых слотов из БД
            // Пример:
            return new List<BookedSlot>();
        }

    }
}