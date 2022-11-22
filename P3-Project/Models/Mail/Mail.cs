namespace P3_Project.Models.Mail;
using System.Net;
using System.Net.Mail;
using System.Configuration;

class AttachmentSpecification {
	public string filepath = "";
	public string? filename = null;
}

public class MailClient {
	SmtpClient client;
	MailMessage message = new();
	List<AttachmentSpecification> attachments = new();
	public MailClient(){
		const string ERROR_MESSAGE = "was not configured in App.Config";
		var userName = ConfigurationManager.AppSettings["email-username"] ?? 
			throw new Exception("\"email-username\" " + ERROR_MESSAGE);
		var password = ConfigurationManager.AppSettings["email-password"] ?? 
			throw new Exception("\"email-password\" " + ERROR_MESSAGE);
		var server   = ConfigurationManager.AppSettings["email-server"  ] ?? 
			throw new Exception("\"email-server\" "   + ERROR_MESSAGE);

		Console.WriteLine(server + " " + password + " " + client);

		client = new(server){
			Port = 587,
			Credentials = new NetworkCredential(userName, password),
			EnableSsl = true,
		};

		message.From = new MailAddress(userName);
	}

	public MailClient To(string to){
		this.message.To.Add(new MailAddress(to));
		return this;
	}

	public MailClient ToList(MailList list) {
		foreach(var address in list) this.To(address);	
		return this;
	}

	public MailClient Subject(string subject){
		this.message.Subject = subject;
		return this;
	}

	public MailClient Body(string body){
		this.message.Body = body;
		return this;
	}

	public MailClient SetHtml(bool value){
		this.message.IsBodyHtml = value;
		return this;
	}

	public MailClient Attachment(string filepath, string? filename = null){
		this.attachments.Add(new (){filepath = filepath, filename = filename});
		return this;
	}

	public void SendMail(){
		foreach(var file in this.attachments){
			this.message.Attachments.Add(
				new Attachment(File.OpenRead(file.filepath), file.filename ??
					file.filepath.Split("/").Last()));
		}
		client.Send(message);
		foreach(var attachment in this.message.Attachments){
			attachment.ContentStream.Close();
		}
	}
}

