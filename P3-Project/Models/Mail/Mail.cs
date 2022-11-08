namespace P3_Project.Models.Mail;
using System.Net;
using System.Net.Mail;
using System.Configuration;


public class MailClient {
	SmtpClient client;
	public MailClient(){
		const string ERROR_CODE = "was not configured in App.Config";
		var userName = ConfigurationManager.AppSettings["email-username"] ?? 
			throw new Exception("\"email-username\" " + ERROR_CODE);
		var password = ConfigurationManager.AppSettings["email-username"] ?? 
			throw new Exception("\"email-password\" " + ERROR_CODE);
		var server   = ConfigurationManager.AppSettings["email-server"  ] ?? 
			throw new Exception("\"email-server\" "   + ERROR_CODE);

		client = new(server){
			Port = 587,
			Credentials = new NetworkCredential(userName, password),
			EnableSsl = true,
		};
	}

	public void SendMail(MailMessage m){
		client.Send(m);
	}

}

