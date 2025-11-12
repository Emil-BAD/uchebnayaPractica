using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using uchebnayaPractica.Models;

namespace uchebnayaPractica
{
    public partial class Organizator : Window
    {
        private readonly User currentUser;
        private readonly string role;

        public Organizator(string firstName, string middleName, User user, string role)
        {
            InitializeComponent();

            this.currentUser = user;
            this.role = role;

            ShowGreeting(firstName, middleName);
            LoadUserPhoto();
            UpdateTitlesByRole();
        }

        private void ShowGreeting(string firstName, string middleName)
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            string greeting = now switch
            {
                var t when t >= new TimeSpan(9, 0, 0) && t <= new TimeSpan(11, 0, 0) => "Доброе утро",
                var t when t > new TimeSpan(11, 0, 0) && t <= new TimeSpan(18, 0, 0) => "Добрый день",
                _ => "Добрый вечер"
            };

            GreetingText.Text = $"{greeting}!";
            UserNameText.Text = $"{firstName} {middleName}";
        }

        private void LoadUserPhoto()
        {
            try
            {
                string imagePath = currentUser.Image;

                string resourcePath = $"pack://application:,,,/Resources/{imagePath}";

              
                Uri uri = new Uri(resourcePath, UriKind.Absolute);
                Photo.Source = new BitmapImage(uri);
            }
            catch
            {
                Photo.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/no_photo.png"));
            }
        }

        private void UpdateTitlesByRole()
        {
            switch (role.ToLower())
            {
                case "организатор":
                    Header.Text = "Окно организатора";
                    this.Title = "Организатор";
                    break;
                case "участник":
                    Header.Text = "Окно участника";
                    this.Title = "Участник";
                    break;
                case "модератор":
                    Header.Text = "Окно модератора";
                    this.Title = "Модератор";
                    break;
                case "жюри":
                    Header.Text = "Окно жюри";
                    this.Title = "Жюри";
                    break;
                default:
                    Header.Text = "Окно пользователя";
                    this.Title = "Пользователь";
                    break;
            }
        }
    }
}
