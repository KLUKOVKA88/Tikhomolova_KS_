using System;
using System.Net;
using System.Net.Mail;

namespace ConsoleTests
{
    class Program
    {
        static void Main(string[] args)
        {

            var to = new MailAddress("k3incho@inbox.ru", "Костя");           //кому отправляем
            var from = new MailAddress("8_klukovka_8@inbox.ru", "Костя");    //от кого отправляем

            var message = new MailMessage(from, to);    //создаем почтовое отправление
            //var msg = new MailAddress("user@server.ru", "qwe@ASD.ru");

            message.Subject = "Заголовок письма от " + DateTime.Now;
            message.Body = "Тело текстового письма + " + DateTime.Now;

            //создаем клиента SMTP почты, через который будет отправляться почта
            var client = new SmtpClient("smtp.mail.ru"/*, "smtp.yandex.ru", 587 */);
            client.EnableSsl = true;

            //указываем учетные данные почты клиента
            client.Credentials = new NetworkCredential
            {
                UserName = "user_name",
                Password = "PassWord!"
            };

            //отправляем сообщение
            client.Send(message);

            //Console.WriteLine("Hello World!");
        }
    }
}