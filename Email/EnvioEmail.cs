
using System.Configuration;
using System.Net.Mail;

namespace GestaoFornecedores.Infra.CrossCutting.Email
{
	public static class EnvioEmail
	{
		private static readonly string _de = ConfigurationManager.AppSettings["De"];
		private static readonly string _senha = ConfigurationManager.AppSettings["Senha"];
		private static readonly string _host = ConfigurationManager.AppSettings["Host"];
		private static readonly string _port = ConfigurationManager.AppSettings["Porta"];

		public static void EnviarEmail(string para, string assunto, string body)
		{
			EnviarEmail(para, assunto, body, string.Empty);
		}

		public static void EnviarEmail(string para, string assunto, string body, string cc)
		{
			MailMessage mail = new MailMessage();
			mail.From = new MailAddress(_de);
			mail.To.Add(para);
			mail.Subject = assunto;
			mail.IsBodyHtml = true;
			mail.Body = body;

			if (!string.IsNullOrEmpty(cc))
				mail.CC.Add(cc);

			SmtpClient StmpServer = CriarServidorSmtp();
			StmpServer.Send(mail);
		}

		private static SmtpClient CriarServidorSmtp()
		{
			SmtpClient SmtpServer = new SmtpClient(_host);
			SmtpServer.Port = int.Parse(_port);
			SmtpServer.UseDefaultCredentials = false;
			SmtpServer.Credentials = new System.Net.NetworkCredential(_de, _senha);
			SmtpServer.EnableSsl = false;

			return SmtpServer;
		}
	}
}
