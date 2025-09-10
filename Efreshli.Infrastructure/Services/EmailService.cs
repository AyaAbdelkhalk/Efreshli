<<<<<<< Updated upstream
=======
using Efreshli.Application.DTOs.ContactUs;
using Efreshli.Application.Helper.ResultPattern;
>>>>>>> Stashed changes
using Efreshli.Application.Services.EmailService;
using Efreshli.Domain.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net;

namespace Efreshli.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendConfirmationEmailAsync(string email, string token)
        {
            try
            {
                //var encodedToken = Uri.EscapeDataString(token);
                //var encodedEmail = Uri.EscapeDataString(email);
                var confirmationLink = Uri.EscapeDataString($"{_emailSettings.BaseUrl}/confirm-email?email={email}&token={token}");

                //var confirmationLink = $"{_emailSettings.BaseUrl}/confirm-email?email={email}&token={token}";
                var subject = "Confirm your email";
                var body = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='utf-8'>
                        <style>
                            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                            .button {{ 
                                display: inline-block; 
                                padding: 12px 24px; 
                                background-color: #007bff; 
                                color: white; 
                                text-decoration: none; 
                                border-radius: 4px; 
                                margin: 20px 0;
                            }}
                            .button:hover {{ background-color: #0056b3; }}
                        </style>
                    </head>
                    <body>
                        <h2>Email Confirmation</h2>
                        <p>Please confirm your email address by clicking the button below:</p>
                        <a href='{confirmationLink}' class='button'>Confirm Email</a>
                        <p>Or copy and paste this link in your browser:</p>
                        <p>{confirmationLink}</p>
                        <p>If you did not request this confirmation, please ignore this email.</p>
                    </body>
                    </html>";

                await SendEmailAsync(email, subject, body);
                _logger.LogInformation("Confirmation email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send confirmation email to {Email}", email);
                throw;
            }
        }

        public async Task SendPasswordResetEmailAsync(string email, string token)
        {
            try
            {
                var resetLink = $"{_emailSettings.BaseUrl}/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
                var subject = "Reset your password";
                var body = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='utf-8'>
                        <style>
                            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                            .button {{ 
                                display: inline-block; 
                                padding: 12px 24px; 
                                background-color: #dc3545; 
                                color: white; 
                                text-decoration: none; 
                                border-radius: 4px; 
                                margin: 20px 0;
                            }}
                            .button:hover {{ background-color: #c82333; }}
                        </style>
                    </head>
                    <body>
                        <h2>Password Reset Request</h2>
                        <p>You requested to reset your password. Click the button below to proceed:</p>
                        <a href='{resetLink}' class='button'>Reset Password</a>
                        <p>Or copy and paste this link in your browser:</p>
                        <p>{resetLink}</p>
                        <p>This link will expire in 1 hour for security reasons.</p>
                        <p>If you did not request a password reset, please ignore this email.</p>
                    </body>
                    </html>";

                await SendEmailAsync(email, subject, body);
                _logger.LogInformation("Password reset email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
                throw;
            }
        }

        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            if (string.IsNullOrEmpty(_emailSettings.SmtpServer))
                throw new InvalidOperationException("SMTP server is not configured");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Efreshli", _emailSettings.FromEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            try
            {
                _logger.LogInformation("Connecting to SMTP server {Server}:{Port}",
                    _emailSettings.SmtpServer, _emailSettings.Port);

                await client.ConnectAsync(
                    _emailSettings.SmtpServer,
                    _emailSettings.Port,
                    GetSecureSocketOptions());

                if (!string.IsNullOrEmpty(_emailSettings.Username))
                {
                    _logger.LogInformation("Authenticating with username: {Username}", _emailSettings.Username);
                    await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                }

                _logger.LogInformation("Sending email to {ToEmail} with subject: {Subject}", toEmail, subject);
                await client.SendAsync(message);
                _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);
            }
            catch (AuthenticationException authEx)
            {
                _logger.LogError(authEx, "SMTP authentication failed for {Username}", _emailSettings.Username);
                throw new InvalidOperationException("Email authentication failed", authEx);
            }
            catch (SmtpCommandException smtpEx)
            {
                _logger.LogError(smtpEx, "SMTP command error: {Status} - {Message}",
                    smtpEx.StatusCode, smtpEx.Message);
                throw;
            }
            catch (SmtpProtocolException protoEx)
            {
                _logger.LogError(protoEx, "SMTP protocol error");
                throw;
            }
            finally
            {
                if (client.IsConnected)
                {
                    await client.DisconnectAsync(true);
                }
            }
        }

        private SecureSocketOptions GetSecureSocketOptions()
        {
            // Determine SSL/TLS settings based on port
            return _emailSettings.Port switch
            {
                587 => SecureSocketOptions.StartTls, // Typically uses STARTTLS
                465 => SecureSocketOptions.SslOnConnect, // Typically uses SSL
                25 => SecureSocketOptions.StartTlsWhenAvailable, // Opportunistic TLS
                _ => SecureSocketOptions.Auto // Let MailKit figure it out
            };
        }


        #region Email confirmation
        // src/Core/Application/Services/EmailService.cs
        public async Task SendConfirmationEmailAsync(string email, string token, string firstName)
        {
            try
            {
                // Use proper URL encoding
                var encodedEmail = WebUtility.UrlEncode(email);
                var encodedToken = WebUtility.UrlEncode(token);

                // Create the confirmation link with properly encoded parameters
                var confirmationLink = $"{_emailSettings.BaseUrl}/confirm-email?email={encodedEmail}&token={encodedToken}";

                var subject = "Confirm your email";
                var body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .button {{ 
            display: inline-block; 
            padding: 12px 24px; 
            background-color: #007bff; 
            color: white; 
            text-decoration: none; 
            border-radius: 4px; 
            margin: 20px 0;
        }}
        .button:hover {{ background-color: #0056b3; }}
        .plain-link {{ 
            color: #007bff; 
            word-break: break-all; 
            font-size: 14px;
            font-family: monospace;
            background: #f8f9fa;
            padding: 10px;
            border-radius: 4px;
            border: 1px solid #e9ecef;
        }}
    </style>
</head>
<body>
    <h2>Welcome, {firstName}!</h2>
    <p>Thank you for registering. Please confirm your email address by clicking the button below:</p>
    <a href='{confirmationLink}' class='button'>Confirm Email</a>
    <p>Or copy and paste this link in your browser:</p>
    <p class='plain-link'>{confirmationLink}</p>
    <p><strong>Note:</strong> If the button doesn't work, make sure to copy the entire link including both email and token parameters.</p>
    <p>If you did not request this confirmation, please ignore this email.</p>
</body>
</html>";

                await SendEmailAsync(email, subject, body);
                _logger.LogInformation("Confirmation email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send confirmation email to {Email}", email);
                throw;
            }
        }

        public async Task SendPasswordResetEmailAsync(string email, string token, string firstName)
        {
            try
            {
                var resetLink = $"{_emailSettings.BaseUrl}/reset-password?email={WebUtility.UrlEncode(email)}&token={token}";

                var subject = "Reset Your Password";
                var body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background: #dc3545; color: white; padding: 20px; text-align: center; }}
                    .content {{ padding: 30px; background: #f9f9f9; }}
                    .button {{ display: inline-block; padding: 12px 24px; background: #dc3545; 
                             color: white; text-decoration: none; border-radius: 5px; }}
                    .footer {{ padding: 20px; text-align: center; color: #666; font-size: 12px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Password Reset Request</h1>
                    </div>
                    <div class='content'>
                        <h2>Hi {firstName},</h2>
                        <p>We received a request to reset your password. Click the button below to proceed.</p>
                        <p style='text-align: center;'>
                            <a href='{resetLink}' class='button'>Reset Password</a>
                        </p>
                        <p>Or copy and paste this link in your browser:</p>
                        <p style='word-break: break-all; color: #dc3545;'>{resetLink}</p>
                        <p>This link will expire in 1 hour for security reasons.</p>
                        <p>If you didn't request a password reset, please ignore this email.</p>
                    </div>
                    <div class='footer'>
                        <p>For security reasons, do not share this email with anyone.</p>
                    </div>
                </div>
            </body>
            </html>";

                await SendEmailAsync(email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
                throw;
            }
        }
        #endregion

        public async Task<bool> TestConnectionAsync()
        {
            if (string.IsNullOrEmpty(_emailSettings.SmtpServer))
                return false;

            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(
                    _emailSettings.SmtpServer,
                    _emailSettings.Port,
                    GetSecureSocketOptions());

                if (!string.IsNullOrEmpty(_emailSettings.Username))
                {
                    await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                }

                var canConnect = client.IsConnected;
                await client.DisconnectAsync(true);

                return canConnect;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SMTP connection test failed");
                return false;
            }
        }


    }
}