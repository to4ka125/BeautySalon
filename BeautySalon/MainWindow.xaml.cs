using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BeautySalon.viewBase;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.IO;
using BeautySalon.Forms;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Drawing;
using System.Threading;
using Path = System.IO.Path;
using System.Diagnostics;

namespace BeautySalon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string captchaText;
        bool captchaCheck;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            passwordCheckPsaholder(passwordBox);
        }

        private void passwordCheckPsaholder(PasswordBox passwordBox)
        {
            TextBlock textBlock = (TextBlock)passwordBox.Template.FindName("textBlock", passwordBox);
            if (passwordBox.Password.Length > 0)
            {
                textBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                textBlock.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.login == loginBox.Text)
            {
                if (Properties.Settings.Default.pwd == passwordBox.Password)
                {
                    MessageBox.Show("Вы успешно авторизовались под локальным пользователем");
                    SisAdminWorkTable siadmin = new SisAdminWorkTable();
                    this.Visibility = Visibility.Collapsed;
                    siadmin.ShowDialog();
                    Visibility = Visibility.Visible;
                    passwordBox.Clear();
                    passwordTextBox.Clear();
                    return;
                }
                else 
                {
                    MessageBox.Show("Неверный пароль.");
                    passwordBox.Clear();
                    passwordTextBox.Clear();

                    MessageBox.Show("Необходимо пройти captcha");
                    Autorizetionbox1.IsEnabled = false;
                    CaptchaBox.Visibility = Visibility.Visible;
                    captchaText = GenerateRandomText(4);
                    Bitmap captchaBitmap = GenerateCaptchaImage(captchaText);
                    CAPTCHA.Source = ConvertBitmapToBitmapSource(captchaBitmap);
                    return;
                }
            }
            MySqlConnection con = new MySqlConnection(SqlConnection.connectionString); 
            try
            {
                con.Open();
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка с подключением","Ошикба",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }

            string login = loginBox.Text;
            string password = passwordBox.Password;

            if (string.IsNullOrWhiteSpace(login))
            {
                MessageBox.Show("Введите логин.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MySqlCommand cmd = new MySqlCommand($"Select * From Employees where login = '{login}'",con);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count>0)
            {
                string passwordBd = dt.Rows[0].ItemArray[5].ToString();

                if (passwordBd == GetHashPass(password) || passwordBd == GetHashPass(passwordTextBox.Text)   )
                {

                    MyData.role = dt.Rows[0].ItemArray[6].ToString();
                    MyData.name = $"{dt.Rows[0].ItemArray[1]} {dt.Rows[0].ItemArray[2]}";
                    MyData.employee_idLoaded = dt.Rows[0][0].ToString();


                    FormUtils.workTable  = new workTable();
                    Visibility = Visibility.Collapsed;


                    FormUtils.workTable.ShowDialog();
                    
                    loginBox.Clear();
                    passwordBox.Clear();
                    passwordTextBox.Clear();
                    Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Неверный пароль");
                    passwordBox.Clear();
                    passwordTextBox.Clear();

                    MessageBox.Show("Необходимо пройти captcha");
                    CaptchaBox.Visibility = Visibility.Visible;
                    captchaText = GenerateRandomText(4);
                    Bitmap captchaBitmap = GenerateCaptchaImage(captchaText);
                    CAPTCHA.Source = ConvertBitmapToBitmapSource(captchaBitmap);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Пользователь не найден");
            }
        }
        public static string GetHashPass(string password)
        {

            byte[] bytesPass = Encoding.UTF8.GetBytes(password);

            SHA256Managed hashstring = new SHA256Managed();

            byte[] hash = hashstring.ComputeHash(bytesPass);

            string hashPasswd = string.Empty;

            foreach (byte x in hash)
            {
                hashPasswd += String.Format("{0:x2}", x);
            }

            hashstring.Dispose();

            return hashPasswd;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
                      
        }
        private void CheckBoxClose_Click(object sender, RoutedEventArgs e)
        {       
            passwordBox.Visibility = Visibility.Visible;
        }

        private void CheckBoxOpen_Click(object sender, RoutedEventArgs e)
        {
             passwordBox.Visibility = Visibility.Collapsed;
        
        }
        private void loginBox_KeyDown(object sender, KeyEventArgs e)
        {
           
         
        }
        private void loginBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text,@"^[\W]$")) { e.Handled = true;}
            if (Regex.IsMatch(e.Text,@"^[а-яА-Я]$")) { e.Handled = true;}
          
            var passwordBox = sender as PasswordBox;
          
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти из приложения?", "Выход из приложения", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                CreateSqlDump();
                CreateSqlDump();
                this.Close();
            }
        }

        private void CreateSqlDump()
        {
            string server = "localhost";
            string database = "restaurant";
            string userId = "root";
            string password = "";

            string mysqldumpPath = @"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqldump.exe";

            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string backupDir = Path.Combine(documentsPath, "reservCopy");
            string backupFile = Path.Combine(backupDir, $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql");

            try
            {
                if (!Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                    Console.WriteLine($"Создана папка: {backupDir}");
                }

                string args = $"--host={server} --user={userId} --password={password} " +
                             $"--databases {database} --result-file=\"{backupFile}\" " +
                             "--skip-lock-tables --single-transaction --routines --triggers";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = mysqldumpPath,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"Ошибка mysqldump: {error}");
                    }
                }

                MessageBox.Show($"Резервная копия сохранена в:\n{backupFile}", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                MessageBox.Show($"Ошибка при создании резервной копии:\n{ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            PasswordTwo.Visibility = Visibility.Visible;
            passwordTextBox.Text = passwordBox.Password;
            PasswordOne.Visibility = Visibility.Collapsed;
 
        }
        private void Btn_Click_1(object sender, RoutedEventArgs e)
        {
            PasswordOne.Visibility = Visibility.Visible;

            passwordBox.Password = passwordTextBox.Text;

            PasswordTwo.Visibility = Visibility.Collapsed;
        }
        private void Btn_Click_2(object sender, RoutedEventArgs e)
        {
            captchaText = GenerateRandomText(4);
            Bitmap captchaBitmap = GenerateCaptchaImage(captchaText);
            CAPTCHA.Source = ConvertBitmapToBitmapSource(captchaBitmap);
        }
        private string GenerateRandomText(int length)
        {
            Random rand = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"; // Буквы и цифры
            char[] result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[rand.Next(chars.Length)];
            }

            return new string(result);
        }
        private Bitmap GenerateCaptchaImage(string text)
        {
            int width = 200;
            int height = 50; // высота изображения
            // Создаем изображение
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                Random rand = new Random();

                // Заливаем фон градиентом
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int gray = rand.Next(256);
                        g.FillRectangle(new SolidBrush(System.Drawing.Color.FromArgb(gray, gray, gray)), x, y, 1, 1);
                    }
                }
                Font font = new Font("Arial", 20, System.Drawing.FontStyle.Bold);
                System.Drawing.Brush textBrush = System.Drawing.Brushes.Black;

                SizeF textSize = g.MeasureString(text, font);
                // Вычисляем начальную позицию по X для центрирования
                float startX = (width - textSize.Width) / 2;

                for (int i = 0; i < text.Length; i++)
                {
                    float waveHeight = (float)(Math.Sin(i + DateTime.Now.Second) * 5); // высота волны
                    g.DrawString(text[i].ToString(), font, textBrush, new PointF(startX + i * 11, height / 3 + waveHeight));

                }
            }
            return bitmap;
        }
        private BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            hBitmap,
            IntPtr.Zero,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());

            // Освобождаем дескриптор
            DeleteObject(hBitmap);

            return bitmapSource;
        }
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        private void BtnProv_Click(object sender, RoutedEventArgs e)
        {
            if (CaptchaText.Text == captchaText)
            {
                MessageBox.Show("Captcha успешно пройдена");
                captchaCheck = true;
                CaptchaText.Clear();
                CaptchaBox.Visibility = Visibility.Collapsed;            
            }
            else
            {
                captchaCheck = false;
                MessageBox.Show("Captcha не пройдена");
                MessageBox.Show("Блокировка формы на 10 секунд");
                CaptchaText.Clear();
                CaptchaBox.Visibility = Visibility.Collapsed;
                Autorizetionbox.IsEnabled = false;
                Thread.Sleep(10000);
                Autorizetionbox.IsEnabled = true;
            }

        }
    }
}