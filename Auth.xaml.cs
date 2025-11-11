using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace uchebnayaPractica
{
    public partial class Auth : Window
    {
        private string currentCaptcha = "";
        private readonly Random rnd = new Random();

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
            currentCaptcha = GenerateCaptchaCode(4);
            CaptchaImage.Source = GenerateCaptchaImage(currentCaptcha, 160, 60);
        }

        private string GenerateCaptchaCode(int length)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            char[] buffer = new char[length];
            for (int i = 0; i < length; i++)
                buffer[i] = chars[rnd.Next(chars.Length)];
            return new string(buffer);
        }

        private ImageSource GenerateCaptchaImage(string text, int width, int height)
        {
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                // фон
                dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(235, 235, 245)), null, new Rect(0, 0, width, height));

                // линии помех
                for (int i = 0; i < rnd.Next(3, 6); i++)
                {
                    var pen = new Pen(new SolidColorBrush(Color.FromArgb((byte)rnd.Next(80, 150),
                        (byte)rnd.Next(50, 150), (byte)rnd.Next(50, 150), (byte)rnd.Next(50, 150))), 1);
                    dc.DrawLine(pen,
                        new Point(rnd.Next(width), rnd.Next(height)),
                        new Point(rnd.Next(width), rnd.Next(height)));
                }

                // символы
                int charCount = text.Length;
                double charWidth = width / (double)charCount;
                for (int i = 0; i < charCount; i++)
                {
                    string ch = text[i].ToString();
                    double fontSize = rnd.Next(22, 30);
                    var typeface = new Typeface("Arial Black");
                    var formatted = new FormattedText(
                        ch,
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        typeface,
                        fontSize,
                        new SolidColorBrush(Color.FromRgb(
                            (byte)rnd.Next(0, 120),
                            (byte)rnd.Next(0, 120),
                            (byte)rnd.Next(0, 120))),
                        1.25);

                    double x = i * charWidth + rnd.Next(-3, 3);
                    double y = rnd.Next(5, (int)(height - fontSize - 5));

                    dc.PushTransform(new RotateTransform(rnd.Next(-25, 25), x + fontSize / 2, y + fontSize / 2));
                    dc.DrawText(formatted, new Point(x, y));
                    dc.Pop();
                }

                // точки-«шум»
                for (int i = 0; i < 120; i++)
                {
                    var brush = new SolidColorBrush(Color.FromArgb((byte)rnd.Next(80, 200),
                        (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)));
                    dc.DrawEllipse(brush, null, new Point(rnd.Next(width), rnd.Next(height)), 1, 1);
                }
            }

            RenderTargetBitmap bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(visual);
            return bmp;
        }
    }
}
