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
using System.Windows.Shapes;

namespace uchebnayaPractica
{
    /// <summary>
    /// Логика взаимодействия для SignIn.xaml
    /// </summary>
    public partial class SignIn : Window
    {
        private string captchaCode = "";
        private void BtnRefreshCaptcha_Click(object sender, RoutedEventArgs e)
        {
            generateAndShowCaptcha();
            CaptchaInput.Text = "";
            StatusText.Text = "";
        }
        public SignIn()
        {
            InitializeComponent();
            generateAndShowCaptcha();
        }

        private void generateAndShowCaptcha()
        {
            captchaCode = CaptchaGenerator.generateCaptchaCode(4);
            CaptchaImage.Source = CaptchaGenerator.generateCaptchaImage(captchaCode, 165,60);
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoginInput.Text != "" && PasswordInput.Password != "")
            {
                if (captchaCode == CaptchaInput.Text)
                {
                    StatusText.Text = "CAPTCHA right";
                    StatusText.Foreground = Brushes.Green;
                }
                else
                {
                    StatusText.Text = "CAPTCHA incorrect";
                    StatusText.Foreground = Brushes.Red;
                }
            } else
            {
                StatusText.Text = "You wrote incorrect info";
                StatusText.Foreground = Brushes.Red;
            }
        }
    }
}
