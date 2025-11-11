using System;
using System.Windows;

namespace uchebnayaPractica
{
    public partial class Organizator : Window
    {
        private string userFirstName;
        private string userMiddleName;

        public Organizator(string firstName, string middleName)
        {
            InitializeComponent();

            userFirstName = firstName;
            userMiddleName = middleName;

            ShowGreeting();
        }

        private void ShowGreeting()
        {

            TimeSpan now = DateTime.Now.TimeOfDay;

            string greeting;

            if (now >= new TimeSpan(9, 0, 0) && now <= new TimeSpan(11, 0, 0))
                greeting = "Доброе утро";
            else if (now > new TimeSpan(11, 0, 0) && now <= new TimeSpan(18, 0, 0))
                greeting = "Добрый день";
            else
                greeting = "Добрый вечер";

            GreetingText.Text = $"{greeting}!";
            UserNameText.Text = $"{userFirstName} {userMiddleName}";
        }
    }
}
