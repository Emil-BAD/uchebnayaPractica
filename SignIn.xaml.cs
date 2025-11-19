using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Win32;
using Microsoft.EntityFrameworkCore;
using uchebnayaPractica.Models;
using System.Windows.Media;
using System.Text;

namespace uchebnayaPractica
{
    public partial class SignIn : Window
    {

        private string imageFileName = "foto1.jpg";
        public SignIn()
        {
            InitializeComponent();
            imageFileName = "foto1.jpg";
            AttachToEventCheckBox.IsChecked = true;
            PhotoImage.Source = new BitmapImage(new Uri("/Resources/foto1.jpg", UriKind.Relative));
            GenerateIdNumber();
            LoadDirections();
            LoadEvents();
        }

        private void GenerateIdNumber ()
        {
            using (var context = new Praktika2Context())
            {
                try
                {
                    var lastUser = context.User
                        .OrderBy(x => x.Id)
                        .Last();
                    var id = lastUser.Id + 1;
                    IdNumberText.Text = id.ToString();
                }
                catch (Exception ex)
                {
                    StatusText.Foreground = Brushes.Red;
                    StatusText.Text = $"Ошибка загрузки направлений: {ex.Message}";
                }
            }
        }

        #region === Загрузка списков ===
        private string GetSelectedDirectionName()
        {
            if (CustomDirectionCheckBox.IsChecked == true)
            {
                return CustomDirectionTextBox.Text.Trim();
            }
            else
            {
                var selected = DirectionComboBox.SelectedItem as Direction;
                return selected?.DirectionName?.Trim();
            }
        }
        private void LoadDirections()
        {
            using (var context = new Praktika2Context())
            {
                try
                {
                    var directions = context.Direction
                            .OrderBy(d => d.Id)
                            .ToList();

                    DirectionComboBox.ItemsSource = directions;
                    DirectionComboBox.DisplayMemberPath = "DirectionName";
                    DirectionComboBox.SelectedValuePath = "Id";
                }
                catch (Exception ex)
                {
                    StatusText.Foreground = Brushes.Red;
                    StatusText.Text = $"Ошибка загрузки направлений: {ex.Message}";
                }
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
                    var events = context.Event
                        .OrderBy(u => u.Id)
                        .ToList();
                    var eventsName = new List<string>();
                    for (var i = 0; i < events.Count; i++)
                    {
                        eventsName.Add(events[i].NameEvent);
                    }
                    EventComboBox.ItemsSource = eventsName;
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
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                imageFileName = Path.GetFileName(openFileDialog.FileName); // ← Только имя
                PhotoImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                RemovePhotoButton.Visibility = Visibility.Visible;
            }
        }

        private void RemovePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            imageFileName = "foto1.jpg"; // ← Не null!
            PhotoImage.Source = new BitmapImage(new Uri("/Resources/foto1.jpg", UriKind.Relative));
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
            if (string.IsNullOrEmpty(input))
                return input;

            // Удаляем все нецифровые символы кроме +
            string digits = new string(input.Where(c => char.IsDigit(c) || c == '+').ToArray());

            // Обрабатываем начало номера
            if (digits.StartsWith("8") && digits.Length > 1)
            {
                digits = "+7" + digits.Substring(1);
            }
            else if (!digits.StartsWith("+7"))
            {
                // Если номер не начинается с +7, добавляем +7
                if (digits.StartsWith("7"))
                {
                    digits = "+" + digits;
                }
                else
                {
                    digits = "+7" + digits;
                }
            }

            // Ограничиваем длину (код страны + 10 цифр)
            if (digits.Length > 12)
                digits = digits.Substring(0, 12);

            // Форматируем номер
            StringBuilder formatted = new StringBuilder();

            if (digits.Length >= 2)
            {
                formatted.Append(digits.Substring(0, 2)); // +7

                if (digits.Length > 2)
                {
                    formatted.Append("(");
                    // Берем следующие 3 цифры (код оператора)
                    int operatorCodeLength = Math.Min(3, digits.Length - 2);
                    formatted.Append(digits.Substring(2, operatorCodeLength));

                    // Закрываем скобку только если набрано 3 цифры кода оператора
                    if (operatorCodeLength == 3)
                    {
                        formatted.Append(")");

                        if (digits.Length > 5)
                        {
                            formatted.Append("-");
                            // Первые 3 цифры номера
                            int firstPartLength = Math.Min(3, digits.Length - 5);
                            formatted.Append(digits.Substring(5, firstPartLength));

                            if (firstPartLength == 3 && digits.Length > 8)
                            {
                                formatted.Append("-");
                                // Следующие 2 цифры
                                int secondPartLength = Math.Min(2, digits.Length - 8);
                                formatted.Append(digits.Substring(8, secondPartLength));

                                if (secondPartLength == 2 && digits.Length > 10)
                                {
                                    formatted.Append("-");
                                    // Последние 2 цифры
                                    int thirdPartLength = Math.Min(2, digits.Length - 10);
                                    formatted.Append(digits.Substring(10, thirdPartLength));
                                }
                            }
                        }
                    }
                }
            }

            return formatted.ToString();
        }

        private void PhoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Сохраняем позицию курсора и текст до форматирования
            int cursorPosition = PhoneTextBox.SelectionStart;
            string originalText = PhoneTextBox.Text;

            string formatted = FormatPhoneNumber(PhoneTextBox.Text);

            if (formatted != PhoneTextBox.Text)
            {
                PhoneTextBox.Text = formatted;

                // Логика для корректного позиционирования курсора
                if (cursorPosition == originalText.Length)
                {
                    // Если курсор был в конце, ставим в конец
                    PhoneTextBox.SelectionStart = formatted.Length;
                }
                else
                {
                    // Пытаемся сохранить позицию относительно содержания
                    int newPosition = cursorPosition;

                    // Если добавились символы форматирования, сдвигаем курсор
                    if (formatted.Length > originalText.Length)
                    {
                        int addedChars = formatted.Length - originalText.Length;
                        if (cursorPosition >= formatted.Length - addedChars)
                        {
                            newPosition = cursorPosition + addedChars;
                        }
                    }

                    PhoneTextBox.SelectionStart = Math.Min(newPosition, formatted.Length);
                }
            }

            UpdateRegisterButton();
        }
        #endregion


        #region === Обновление кнопки регистрации ===

        private void Field_Changed(object sender, RoutedEventArgs e)
        {
            // Логика для обновления состояния кнопки
            UpdateRegisterButton();
        }
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
        #endregion

        #region === Ручной ввод направления ===
        private void CustomDirectionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            DirectionComboBox.IsEnabled = false; // Отключаем ComboBox
            CustomDirectionTextBox.Visibility = Visibility.Visible; // Показываем поле для ввода
            UpdateRegisterButton(); // Обновляем кнопку регистрации
        }

        private void CustomDirectionCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DirectionComboBox.IsEnabled = true; // Включаем ComboBox
            CustomDirectionTextBox.Visibility = Visibility.Collapsed; // Скрываем поле для ввода
            UpdateRegisterButton(); // Обновляем кнопку регистрации
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
                string password = ShowPasswordCheckBox.IsChecked == true ? PasswordTextBox.Text : PasswordBox.Password;
                string confirmPassword = ShowPasswordCheckBox.IsChecked == true ? ConfirmPasswordTextBox.Text : ConfirmPasswordBox.Password;

                if (password != confirmPassword)
                {
                    StatusText.Foreground = Brushes.Red;
                    StatusText.Text = "Пароли не совпадают!";
                    return;
                }

                string directionName = GetSelectedDirectionName();
                if (string.IsNullOrWhiteSpace(directionName))
                {
                    StatusText.Foreground = Brushes.Red;
                    StatusText.Text = "Направление не выбрано или не введено!";
                    return;
                }

                string gender = GenderComboBox.SelectedItem is ComboBoxItem genderItem ? genderItem.Content.ToString() : null;
                string role = RoleComboBox.SelectedItem is ComboBoxItem roleItem ? roleItem.Content.ToString() : null;

                if (string.IsNullOrEmpty(gender) || string.IsNullOrEmpty(role))
                {
                    StatusText.Foreground = Brushes.Red;
                    StatusText.Text = "Заполните все обязательные поля!";
                    return;
                }

                using (var context = new Praktika2Context())
                {
                    // Проверка email
                    if (context.User.Any(u => u.Email == EmailTextBox.Text.Trim()))
                    {
                        StatusText.Foreground = Brushes.Red;
                        StatusText.Text = "Пользователь с таким email уже существует!";
                        return;
                    }

                    // === КЛЮЧЕВОЕ ИСПРАВЛЕНИЕ: Загружаем в память ===
                    var genders = context.Gender.AsEnumerable(); // <-- AsEnumerable() — клиентская оценка
                    var roles = context.Role.AsEnumerable();
                    var directions = context.Direction.AsEnumerable();

                    var genderEntity = genders
                        .FirstOrDefault(g => g.GenderName.Trim().Equals(gender.Trim(), StringComparison.OrdinalIgnoreCase));

                    var roleEntity = roles
                        .FirstOrDefault(r => r.RoleName.Trim().Equals(role.Trim(), StringComparison.OrdinalIgnoreCase));

                    var directionEntity = directions
                        .FirstOrDefault(d => d.DirectionName.Trim().Equals(directionName.Trim(), StringComparison.OrdinalIgnoreCase));

                    if (genderEntity == null || roleEntity == null || directionEntity == null)
                    {
                        StatusText.Foreground = Brushes.Red;
                        StatusText.Text = "Не найдено: пол, роль или направление!";
                        return;
                    }

                    Event eventEntity = null;
                    if (AttachToEventCheckBox.IsChecked == true && EventComboBox.SelectedItem != null)
                    {
                        var eventName = EventComboBox.SelectedItem.ToString();
                        eventEntity = context.Event
                            .FirstOrDefault(e => e.NameEvent == eventName); // Можно оставить, если NameEvent уникально
                    }

                    var newUser = new User
                    {
                        Id = int.Parse(IdNumberText.Text),
                        Fio = FullNameTextBox.Text.Trim(),
                        Email = EmailTextBox.Text.Trim(),
                        Phone = PhoneTextBox.Text.Trim(),
                        Password = password,
                        Dob = DateOnly.FromDateTime(DateTime.Today),
                        Image = imageFileName,
                        Idgender = new List<Gender> { genderEntity },
                        Idrole = new List<Role> { roleEntity },
                        Iddirection = new List<Direction> { directionEntity },
                        Idevent = eventEntity != null ? new List<Event> { eventEntity } : new List<Event>()
                    };

                    try
                    {
                        context.User.Add(newUser);
                        context.SaveChanges();
                    }
                    catch (DbUpdateException dbEx)
                    {
                        // Логирование подробной ошибки
                        StatusText.Foreground = Brushes.Red;
                        StatusText.Text = $"Ошибка при сохранении: {dbEx.InnerException?.Message ?? dbEx.Message}";
                    }

                    MessageBox.Show("Регистрация успешно завершена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                }
            }
            catch (Exception ex)
            {
                StatusText.Foreground = Brushes.Red;
                StatusText.Text = $"Ошибка: {ex.Message}";
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