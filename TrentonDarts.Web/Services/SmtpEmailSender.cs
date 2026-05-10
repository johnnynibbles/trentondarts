using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace TrentonDarts.Web.Services;

public class SmtpEmailSender
{
    private readonly SmtpOptions _opts;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<SmtpOptions> opts, ILogger<SmtpEmailSender> logger)
    {
        _opts = opts.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        if (string.IsNullOrWhiteSpace(_opts.Host))
        {
            _logger.LogWarning("SMTP is not configured — email to {Email} with subject '{Subject}' was not sent.", toEmail, subject);
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_opts.FromName, _opts.FromAddress));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;
        message.Body = new TextPart(TextFormat.Html) { Text = htmlBody };

        using var smtp = new SmtpClient();
        var socketOptions = _opts.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable;
        await smtp.ConnectAsync(_opts.Host, _opts.Port, socketOptions);
        if (!string.IsNullOrEmpty(_opts.UserName))
            await smtp.AuthenticateAsync(_opts.UserName, _opts.Password);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(quit: true);
    }
}
