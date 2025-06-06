using BeautySalon.viewBase;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
    /// Interaction logic for SchedulesAdd.xaml
    /// </summary>
    public partial class SchedulesAdd : Window, INotifyPropertyChanged
    {
        private readonly List<string> _selectedDates = new List<string>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public SchedulesAdd()
        {
            InitializeComponent();
        }
        public HashSet<DateTime> Dates { get; private set; } = new HashSet<DateTime>();

        private void GenerateDates()
        {
            var newDates = new HashSet<DateTime>();


            foreach (string d in _selectedDates)
            {
                DateTime.TryParse(d, out DateTime date);
                newDates.Add(date);
            }
            Dates = newDates;
            OnPropertyChanged(nameof(Dates));
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand($@"SELECT employee_id , concat_ws(' ',first_name,last_name), `role` FROM vkr.employees;",con);

                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Employee.Items.Add($"{dr.GetValue(0)}-{dr.GetValue(1)}");
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Employee.SelectedItem==null || _selectedDates.Count==0)
            {
                MessageBox.Show("Выберите сотрудника");
                return;
            }

            string idEmployer = Employee.Text.Split('-')[0];

            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();

                foreach (var d in _selectedDates)
                {
                    MySqlCommand cmd = new MySqlCommand($@"Insert into employee_schedules(employee_id,dateSheldus,is_working) 
                    Values ('{idEmployer}','{d}','1')",con);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Расписание успешно сохраннено");
                Close();
            }
            
        }



        private void customCalendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (customCalendar.SelectedDate.HasValue)
            {
                var selectedDate = customCalendar.SelectedDate.Value.Date;

                _selectedDates.Add(selectedDate.ToString("yyyy,MM,d"));
            }
            GenerateDates();

            customCalendar.UpdateLayout();
        }

        private void customCalendar_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (customCalendar.SelectedDate.HasValue)
            {
                var selectedDate = customCalendar.SelectedDate.Value.Date;

                _selectedDates.Remove(selectedDate.ToString("yyyy,MM,d"));
            }
            GenerateDates();

            customCalendar.UpdateLayout();
        }

        private void Employee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedDates.Clear();
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();
                string selectedText = Employee.SelectedItem.ToString();
                string idEmployer = selectedText.Split('-')[0].Trim();

                MySqlCommand cmd = new MySqlCommand($@"SELECT dateSheldus FROM 
                vkr.employee_schedules where employee_id= '{idEmployer}';",con);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count>0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (!row.IsNull("dateSheldus"))
                        {
                            DateTime date = (DateTime)row["dateSheldus"];
                            _selectedDates.Add(date.ToString("yyyy-MM-d"));
                        }
                    }
                }
            }
            GenerateDates();
            customCalendar.UpdateLayout();
        }
    }
}
