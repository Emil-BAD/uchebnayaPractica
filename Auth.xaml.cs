using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace uchebnayaPractica
{
    public partial class Auth : Window
    {
        private string currentCaptcha = "";

        public Auth()
        {
            InitializeComponent();
            GenerateAndShowCaptcha();
        }

        private void BtnRefreshCaptcha_Click(object sender, RoutedEventArgs e)
        {
            GenerateAndShowCaptcha();
            CaptchaInput.Text = "";
            StatusText.Text = "";
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.Equals(CaptchaInput.Text?.Trim(), currentCaptcha, StringComparison.OrdinalIgnoreCase))
            {
                StatusText.Foreground = Brushes.Green;
                StatusText.Text = "CAPTCHA верна — вход выполнен";
            }
            else
            {
                StatusText.Foreground = Brushes.Red;
                StatusText.Text = "Неверная CAPTCHA, попробуйте снова";
                GenerateAndShowCaptcha();
                CaptchaInput.Text = "";
            }
        }

        private void GenerateAndShowCaptcha()
        {
            currentCaptcha = CaptchaGenerator.generateCaptchaCode(4);
            CaptchaImage.Source = CaptchaGenerator.generateCaptchaImage(currentCaptcha, 160, 60);
        }
    }
}
