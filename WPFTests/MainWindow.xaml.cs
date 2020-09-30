using System;
using System.Net;
using System.Net.Mail;

namespace WPFTests
{
    public partial class MainWindow
    {
        public MainWindow() => InitializeComponent();

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var to = new MailAddress("k3incho@inbox.ru", "Костя");           //кому отправляем
            var from = new MailAddress("konstantin.tikhomolov@yandex.ru", "Костя");    //от кого отправляем

            var message = new MailMessage(from, to);    //создаем почтовое отправление
            //var msg = new MailAddress("user@server.ru", "qwe@ASD.ru");

            message.Subject = "Заголовок письма от " + DateTime.Now;
            message.Body = "Тело текстового письма + " + DateTime.Now;

            //создаем клиента SMTP почты, через который будет отправляться почта
            //var client = new SmtpClient("smtp.mail.ru", 465);
            var client = new SmtpClient("smtp.yandex.ru", 25);
            client.EnableSsl = true;

            //указываем учетные данные почты клиента
            client.Credentials = new NetworkCredential
            {
                UserName = LoginEdit.Text,
                SecurePassword = PasswordEdit.SecurePassword
            };

            //отправляем сообщение
            client.Send(message);
        }
    }
}
