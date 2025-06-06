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
using System.IO;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;
using MySql.Data.MySqlClient;

namespace BeautySalon.View
{
    /// <summary>
    /// Interaction logic for BackupRecovery1.xaml
    /// </summary>
    public partial class BackupRecovery1 : UserControl
    {
        public BackupRecovery1()
        {
            InitializeComponent();
        }

        private void BtnStruct_Click(object sender, RoutedEventArgs e)
        {
            string conStr = "host=localhost;uid=root;pwd=;";

            string defaultBackupPath = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
      "reservCopy");

            // Создаем диалог выбора файла
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Directory.Exists(defaultBackupPath)
                    ? defaultBackupPath
                    : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "SQL файлы (*.sql)|*.sql|Все файлы (*.*)|*.*",
                Title = "Выберите файл резервной копии",
                DefaultExt = ".sql",
                CheckFileExists = true,
                CheckPathExists = true
            };

            // Показываем диалог
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                // Здесь можно добавить обработку выбранного файла
                MessageBox.Show($"Выбран файл: {selectedFilePath}",
                              "Файл выбран",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);

                // Пример: прочитать содержимое файла
                try
                {
                    using (MySqlConnection con = new MySqlConnection(conStr))
                    {
                        con.Open();
                        string script = File.ReadAllText(selectedFilePath);
                        MySqlScript sqlScript = new MySqlScript(con, script);
                        sqlScript.Execute();
                        MessageBox.Show("Востановление структуры прошло успешно");
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка чтения файла: {ex.Message}",
                                  "Ошибка",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                }
            }
        }
    }
}
