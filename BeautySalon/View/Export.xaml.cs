using System;
using Microsoft.Win32;
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
using System.IO;
using MySql.Data.MySqlClient;
using Path = System.IO.Path;
using System.Data;
using BeautySalon.viewBase;

namespace BeautySalon.View
{
    /// <summary>
    /// Interaction logic for Export.xaml
    /// </summary>
    public partial class Export : UserControl
    {
        public Export()
        {
            InitializeComponent();
        }

        private void FileBtn_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Выберите папку для сохранения CSV файлов",
                ShowNewFolderButton = true
            };

            if (folderBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show("Экспорт отменен", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            string outputDirectory = folderBrowserDialog.SelectedPath;
               try
            {
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();

                DataTable dt = con.GetSchema("Tables");

                int exportedTables = 0;

                foreach (DataRow table in dt.Rows)
                {
                    string tableName = table["TABLE_NAME"].ToString();
                    if (tableName.StartsWith("sys") || tableName.StartsWith("MS_")) continue;

                    ExportTableToCsv(con, tableName, outputDirectory);
                    exportedTables++;
                }

                MessageBox.Show($"Успешно экспортировано {exportedTables} таблиц в папку:\n{outputDirectory}",
                        "Экспорт завершен",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                // Открываем папку с результатами
                System.Diagnostics.Process.Start(outputDirectory);
            }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
            }

        }

        private void ExportTableToCsv(MySqlConnection connection, string tableName, string outputDirectory)
        {
            string query = $"SELECT * FROM {tableName}";
            string filePath = Path.Combine(outputDirectory, $"{tableName}.csv");

            using (var command = new MySqlCommand(query, connection))
            using (var reader = command.ExecuteReader())
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Заголовки столбцов
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (i > 0) writer.Write(",");
                    writer.Write(EscapeCsvValue(reader.GetName(i)));
                }
                writer.WriteLine();

                // Данные
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (i > 0) writer.Write(",");

                        if (!reader.IsDBNull(i))
                        {
                            writer.Write(EscapeCsvValue(reader.GetValue(i).ToString()));
                        }
                    }
                    writer.WriteLine();
                }
            }
        }

        private string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }
            return value;
        }

    }
}
