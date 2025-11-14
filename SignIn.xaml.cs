using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using uchebnayaPractica.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows.Media;

namespace uchebnayaPractica
{
    public partial class SignIn : Window
    {

        string? imageUri = null;
        public SignIn()
        {
            InitializeComponent();
            AttachToEventCheckBox.IsChecked = true;
            PhotoImage.Source = new BitmapImage(new Uri("/Resources/foto1.jpg", UriKind.Relative));
            GenerateIdNumber();
            LoadDirections();
            LoadEvents();
        }

        #region === Генерация ID ===
        private void GenerateIdNumber()
        {

            IdNumberText.Text = "13002";
            try
            {
                using (var context = new Praktika2Context())
                {
                    // 4. Поиск пользователя (с загрузкой ролей)
                    var user = context.User
                        .OrderBy(u => u.Id)
                        .Last();
                    IdNumberText.Text = $"{user.Id + 1}";
                }
            }
            catch (Exception ex)
            {
                StatusText.Foreground = Brushes.Red;
                StatusText.Text = $"Ошибка: {ex.Message}";
            }
        }
        #endregion

        #region === Загрузка списков ===
        private void LoadDirections()
        {
            ///DirectionComboBox.Items.Add("Программирование");
            ///DirectionComboBox.Items.Add("Дизайн");
            ///DirectionComboBox.Items.Add("Маркетинг");
            ///DirectionComboBox.Items.Add("Менеджмент");
            ///DirectionComboBox.Items.Add("Наука");

            try
            {
                using (var context = new Praktika2Context())
                {
                    var directions = context.Direction
                        .OrderBy(u => u.Id);
                    DirectionComboBox.ItemsSource = directions;
                }
            }
            catch(Exception ex)
            {
                StatusText.Foreground = Brushes.Red;
                StatusText.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void LoadEvents()
        {
            ///EventComboBox.Items.Add("Хакатон 2024");
            ///EventComboBox.Items.Add("Научная конференция");
            ///EventComboBox.Items.Add("Дизайн-выставка");
            ///EventComboBox.Items.Add("Бизнес-форум");
            try
            {
                using (var context = new Praktika2Context())
                {
                    var directions = context.Event
                        .OrderBy(u => u.Id);
                    EventComboBox.ItemsSource = directions;
                }
            }
            catch (Exception ex)
            {
                StatusText.Foreground = Brushes.Red;
                StatusText.Text = $"Ошибка: {ex.Message}";
            }
        }
        #endregion

        #region === Выбор фото ===
        private void BrowsePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Выберите фото"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    imageUri = openFileDialog.FileName;
                    PhotoImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                    RemovePhotoButton.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки фото: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RemovePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            PhotoImage.Source = null;
            RemovePhotoButton.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region === Показать/скрыть пароль ===
        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Text = PasswordBox.Password;
            ConfirmPasswordTextBox.Text = ConfirmPasswordBox.Password;

            PasswordBox.Visibility = Visibility.Collapsed;
            PasswordTextBox.Visibility = Visibility.Visible;
            ConfirmPasswordBox.Visibility = Visibility.Collapsed;
            ConfirmPasswordTextBox.Visibility = Visibility.Visible;
        }

        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = PasswordTextBox.Text;
            ConfirmPasswordBox.Password = ConfirmPasswordTextBox.Text;

            PasswordBox.Visibility = Visibility.Visible;
            PasswordTextBox.Visibility = Visibility.Collapsed;
            ConfirmPasswordBox.Visibility = Visibility.Visible;
            ConfirmPasswordTextBox.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region === Валидация пароля ===
        private string CheckPasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password))
                return "Введите пароль";

            var errors = new System.Collections.Generic.List<string>();

            if (password.Length < 6)
                errors.Add("не менее 6 символов");
            if (!password.Any(char.IsUpper) || !password.Any(char.IsLower))
                errors.Add("заглавные и строчные буквы");
            if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]"))
                errors.Add("не менее одного спецсимвола");
            if (!password.Any(char.IsDigit))
                errors.Add("не менее одной цифры");

            return errors.Count == 0 ? "Пароль соответствует требованиям" : $"Требуется: {string.Join(", ", errors)}";
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdatePasswordValidation();
        }

        private void PasswordTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdatePasswordValidation();
        }

        private void UpdatePasswordValidation()
        {
            string password = ShowPasswordCheckBox.IsChecked == true ? PasswordTextBox.Text : PasswordBox.Password;
            string confirm = ShowPasswordCheckBox.IsChecked == true ? ConfirmPasswordTextBox.Text : ConfirmPasswordBox.Password;

            PasswordStrengthText.Text = CheckPasswordStrength(password);

            if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(confirm))
            {
                PasswordMatchText.Visibility = password == confirm ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                PasswordMatchText.Visibility = Visibility.Collapsed;
            }

            UpdateRegisterButton();
        }
        #endregion

        #region === Форматирование телефона ===
        private string FormatPhoneNumber(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            // Удаляем все нецифровые символы кроме +
            var digits = new string(input.Where(c => char.IsDigit(c) || c == '+').ToArray());

            if (digits.StartsWith("+7") && digits.Length > 2)
            {
                string numbers = digits.Substring(2);
                if (numbers.Length >= 3)
                {
                    string result = $"+7({numbers.Substring(0, 3)})";
                    if (numbers.Length >= 6)
                    {
                        result += $"-{numbers.Substring(3, 3)}";
                        if (numbers.Length >= 8)
                        {
                            result += $"-{numbers.Substring(6, 2)}";
                            if (numbers.Length >= 10)
                            {
                                result += $"-{numbers.Substring(8, 2)}";
                            }
                        }
                    }
                    return result;
                }
            }

            return input;
        }

        private void PhoneTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Сохраняем курсор
            int cursorPosition = PhoneTextBox.SelectionStart;

            string formatted = FormatPhoneNumber(PhoneTextBox.Text);
            if (formatted != PhoneTextBox.Text)
            {
                PhoneTextBox.Text = formatted;
                PhoneTextBox.SelectionStart = Math.Min(cursorPosition, formatted.Length);
            }

            UpdateRegisterButton();
        }
        #endregion

        #region === Обновление кнопки регистрации ===
        private void UpdateRegisterButton()
        {
            bool canRegister = !string.IsNullOrWhiteSpace(FullNameTextBox.Text) &&
                              GenderComboBox.SelectedItem != null &&
                              RoleComboBox.SelectedItem != null &&
                              !string.IsNullOrWhiteSpace(EmailTextBox.Text) &&
                              !string.IsNullOrWhiteSpace(PhoneTextBox.Text) &&
                              (DirectionComboBox.SelectedItem != null || !string.IsNullOrWhiteSpace(CustomDirectionTextBox.Text)) &&
                              (!AttachToEventCheckBox.IsChecked.Value || EventComboBox.SelectedItem != null) &&
                              PasswordStrengthText.Text == "Пароль соответствует требованиям" &&
                              PasswordMatchText.Visibility != Visibility.Visible;

            RegisterButton.IsEnabled = canRegister;
        }

        private void Field_Changed(object sender, System.Windows.RoutedEventArgs e)
        {
            UpdateRegisterButton();
        }
        #endregion

        #region === Ручной ввод направления ===
        private void CustomDirectionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            DirectionComboBox.IsEnabled = false;
            CustomDirectionTextBox.Visibility = Visibility.Visible;
            UpdateRegisterButton();
        }

        private void CustomDirectionCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DirectionComboBox.IsEnabled = true;
            CustomDirectionTextBox.Visibility = Visibility.Collapsed;
            UpdateRegisterButton();
        }
        #endregion

        #region === Прикрепление к мероприятию ===
        private void AttachToEventCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EventComboBox.Visibility = Visibility.Visible;
            UpdateRegisterButton();
        }

        private void AttachToEventCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            EventComboBox.Visibility = Visibility.Collapsed;
            UpdateRegisterButton();
        }
        #endregion

        #region === Регистрация ===
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем пароль в зависимости от режима отображения
                string password = ShowPasswordCheckBox.IsChecked == true ?
                    PasswordTextBox.Text : PasswordBox.Password;

                // Получаем направление
                string direction = CustomDirectionCheckBox.IsChecked == true ?
                    CustomDirectionTextBox.Text : DirectionComboBox.SelectedItem.ToString();


                try
                {
                    using (var context = new Praktika2Context())
                    {
                        var user = context.User
                            .Include(u => u.Idrole)
                            .Where(u => u.Email == EmailTextBox.Text && u.Password == password)
                            .Select(u => new
                            {
                                User = u,
                                Roles = u.Idrole.Select(r => r.RoleName).ToList()
                            })
                            .FirstOrDefault();

                        if (user == null)
                        {
                            var newUser = new User
                            {
                                Fio = FullNameTextBox.Text,
                                Email = EmailTextBox.Text,
                                Phone = PhoneTextBox.Text,
                                Password = password,
                                // Обязательные поля, которые нужно заполнить
                                Dob = DateOnly.FromDateTime(DateTime.Today), // Укажите правильную дату рождения
                                Image = imageUri // Укажите путь к изображению по умолчанию
                            };

                            context.User.Add(newUser);
                            MessageBox.Show("Регистрация успешно завершена!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                            this.Close();
                        } else
                        {
                            StatusText.Foreground = Brushes.Red;
                            StatusText.Text = "Пользователь с такими данными уже есть";
                        }
                    }
                } 
                catch (Exception ex) {
                    StatusText.Foreground = Brushes.Red;
                    StatusText.Text = $"Ошибка: {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveToDatabase(string fullName, string gender, string role, string email,
                                  string phone, string direction, string eventName, string password)
        {
            // Заглушка для сохранения в БД
            // В реальном приложении здесь будет работа с Praktika2Context

            string userData = $@"
ID: {IdNumberText.Text}
ФИО: {fullName}
Пол: {gender}
Роль: {role}
Email: {email}
Телефон: {phone}
Направление: {direction}
Мероприятие: {eventName ?? "Не прикреплено"}
Пароль: [захеширован]";

            // Временное сохранение в файл для демонстрации
            File.WriteAllText($"user_{IdNumberText.Text}.txt", userData);
        }
        #endregion

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdatePasswordValidation();
        }
    }
}