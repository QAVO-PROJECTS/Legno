using Legno.Application.Absrtacts.Services;
using Legno.Domain.HelperEntities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

public class MailService : IMailService
{
    private readonly IConfiguration _config;

    public MailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(MailRequest mailRequest, string sectionName)
    {
        var settings = _config.GetSection(sectionName).Get<MailSettings>();

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(settings.DisplayName, settings.Mail));
        email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
        email.Subject = mailRequest.Subject;

        var builder = new BodyBuilder
        {
            HtmlBody = mailRequest.Body
        };

        // 🔥 Faylları əlavə et
        if (mailRequest.Attachments != null)
        {
            foreach (var file in mailRequest.Attachments)
            {
                if (file.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);

                    builder.Attachments.Add(
                        file.FileName,
                        ms.ToArray(),
                        ContentType.Parse(file.ContentType)
                    );
                }
            }
        }

        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

        await smtp.ConnectAsync(settings.Host, settings.Port, SecureSocketOptions.SslOnConnect);
        await smtp.AuthenticateAsync(settings.Mail, settings.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }


    public Task SendFromInfoAsync(MailRequest req)
        => SendEmailAsync(req, "MailSettingsInfo");

    public Task SendFromHRAsync(MailRequest req)
        => SendEmailAsync(req, "MailSettingsHR");
}
