using BeautySalon.viewBase;
using MySql.Data.MySqlClient;
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

using System.Windows.Shapes;

using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BeautySalon.Forms
{
    /// <summary>
    /// Interaction logic for testForm.xaml
    /// </summary>
    public partial class testForm : Window, INotifyPropertyChanged
    {

       // private readonly HashSet<DateTime> _selectedDates = new HashSet<DateTime>();
        private readonly List<string> _selectedDates = new List<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public testForm()
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


        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void customCalendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (customCalendar.SelectedDate.HasValue)
            {
                var selectedDate = customCalendar.SelectedDate.Value.Date;

                //  _selectedDates.Add(selectedDate);
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
    }


}
