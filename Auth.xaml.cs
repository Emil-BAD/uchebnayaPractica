using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using uchebnayaPractica.Models;
using Microsoft.EntityFrameworkCore;

namespace uchebnayaPractica
{
    public partial class Auth : Window
    {
        private string currentCaptcha = "";
        private readonly Random rnd = new Random();
        private readonly string credentialsFile = "user_data.txt";
        private int failedAttempts = 0;
        private DateTime? lockoutTime = null;

        public Auth()
        {
            InitializeComponent();
            LoadSavedCredentials();
            GenerateAndShowCaptcha();
        }

        #region === Загрузка сохранённых данных ===
        private void LoadSavedCredentials()
        {
            try
            {
                if (File.Exists(credentialsFile))
                {
                    string[] lines = File.ReadAllLines(credentialsFile);
                    if (lines.Length >= 2)
                    {
                        IdNumber.Text = lines[0];
                        Password.Password = lines[1];
                        RememberMeCheckBox.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StatusText.Foreground = Brushes.Red;
                StatusText.Text = $"Ошибка: {ex.Message}";
            }
        }
        #endregion

        #region === Обновление капчи ===
        private void BtnRefreshCaptcha_Click(object sender, RoutedEventArgs e)
        {
            GenerateAndShowCaptcha();
            CaptchaInput.Text = "";
            StatusText.Text = "";
        }
        #endregion

        #region === Вход (с CAPTCHA и блокировкой) ===
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "";

            // 1. Проверка блокировки
            if (lockoutTime.HasValue && DateTime.Now < lockoutTime.Value)
            {
                int secondsLeft = (int)(lockoutTime.Value - DateTime.Now).TotalSeconds;
                StatusText.Foreground = Brushes.Red;
                StatusText.Text = $"Заблокировано. Ждите {secondsLeft} сек.";
                return;
            }

            string email = IdNumber.Text.Trim();
            string password = Password.Password.Trim();

            // 2. Проверка полей
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                StatusText.Foreground = Brushes.Red;
                StatusText.Text = "Введите логин и пароль.";
                return;
            }

            // 3. Проверка CAPTCHA
            if (!string.Equals(CaptchaInput.Text?.Trim(), currentCaptcha, StringComparison.OrdinalIgnoreCase))
            {
                StatusText.Foreground = Brushes.Red;
                StatusText.Text = "Неверная CAPTCHA!";
                GenerateAndShowCaptcha();
                CaptchaInput.Text = "";
                return;
            }

            try
            {
                using (var context = new Praktika2Context())
                {
                    // 4. Поиск пользователя (с загрузкой ролей)
                    var user = context.User
                        .Include(u => u.Idrole)
                        .Where(u => u.Email == email && u.Password == password)
                        .Select(u => new
                        {
                            User = u,
                            Roles = u.Idrole.Select(r => r.RoleName).ToList()
                        })
                        .FirstOrDefault();

                    if (user == null)
                    {
                        // 5. Неудачная попытка
                        failedAttempts++;
                        if (failedAttempts >= 3)
                        {
                            lockoutTime = DateTime.Now.AddSeconds(10);
                            failedAttempts = 0; // сброс после блокировки
                            StatusText.Foreground = Brushes.Red;
                            StatusText.Text = "3 ошибки! Блокировка на 10 секунд.";
                        }
                        else
                        {
                            StatusText.Foreground = Brushes.Red;
                            StatusText.Text = $"Неверно. Осталось попыток: {3 - failedAttempts}";
                        }
                        GenerateAndShowCaptcha();
                        CaptchaInput.Text = "";
                        return;
                    }

                    // 6. Успешный вход
                    failedAttempts = 0;
                    lockoutTime = null;

                    string role = user.Roles.FirstOrDefault() ?? "участник";
                    SaveCredentials(email, password);

                    var fio = user.User.Fio.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    string firstName = fio.Length > 1 ? fio[1] : fio[0];
                    string middleName = fio.Length > 2 ? fio[2] : "";

                    var mainWindow = new Organizator(firstName, middleName, user.User, role);
                    mainWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                StatusText.Foreground = Brushes.Red;
                StatusText.Text = $"Ошибка: {ex.Message}";
            }
        }
        #endregion

        #region === Сохранение данных ===
        private void SaveCredentials(string email, string password)
        {
            try
            {
                if (RememberMeCheckBox.IsChecked == true)
                    File.WriteAllLines(credentialsFile, new[] { email, password });
                else if (File.Exists(credentialsFile))
                    File.Delete(credentialsFile);
            }
            catch (Exception ex)
            {
                StatusText.Foreground = Brushes.Red;
                StatusText.Text = $"Ошибка сохранения: {ex.Message}";
            }
        }
        #endregion

        #region === Генерация CAPTCHA (4 символа + шум) ===
        private void GenerateAndShowCaptcha()
        {
            currentCaptcha = GenerateCaptchaCode(4);
            CaptchaImage.Source = GenerateCaptchaImage(currentCaptcha, 180, 70);
        }

        private string GenerateCaptchaCode(int length)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }

        private ImageSource GenerateCaptchaImage(string text, int width, int height)
        {
            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                // Фон
                dc.DrawRectangle(Brushes.WhiteSmoke, null, new Rect(0, 0, width, height));

                // Случайные линии
                for (int i = 0; i < rnd.Next(4, 8); i++)
                {
                    var pen = new Pen(new SolidColorBrush(Color.FromRgb(
                        (byte)rnd.Next(100, 200),
                        (byte)rnd.Next(100, 200),
                        (byte)rnd.Next(100, 200))), 2);
                    dc.DrawLine(pen,
                        new Point(rnd.Next(width), rnd.Next(height)),
                        new Point(rnd.Next(width), rnd.Next(height)));
                }

                // Текст
                double charWidth = width / (double)text.Length;
                for (int i = 0; i < text.Length; i++)
                {
                    double fontSize = rnd.Next(28, 36);
                    var brush = new SolidColorBrush(Color.FromRgb(
                        (byte)rnd.Next(0, 100),
                        (byte)rnd.Next(0, 100),
                        (byte)rnd.Next(0, 100)));

                    var formattedText = new FormattedText(
                        text[i].ToString(),
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Comic Sans MS"),
                        fontSize,
                        brush,
                        1.0);

                    double x = i * charWidth + rnd.Next(-5, 6);
                    double y = rnd.Next(10, (int)(height - fontSize - 10));

                    dc.PushTransform(new RotateTransform(rnd.Next(-30, 31), x + fontSize / 2, y + fontSize / 2));
                    dc.DrawText(formattedText, new Point(x, y));
                    dc.Pop();
                }

                // Шум (точки)
                for (int i = 0; i < 150; i++)
                {
                    var noiseBrush = new SolidColorBrush(Color.FromRgb(
                        (byte)rnd.Next(150, 255),
                        (byte)rnd.Next(150, 255),
                        (byte)rnd.Next(150, 255)));
                    dc.DrawEllipse(noiseBrush, null,
                        new Point(rnd.Next(width), rnd.Next(height)), 1, 1);
                }
            }

            // ВАЖНО: Создаём bitmap ПОСЛЕ закрытия using!
            var bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            return bitmap;
        }
        #endregion
    }
}