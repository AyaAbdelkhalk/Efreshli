<<<<<<< Updated upstream
=======
using Efreshli.Application.DTOs.ContactUs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Domain.Models;
>>>>>>> Stashed changes
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Efreshli.Application.Resources.SharedResourcesKeys;

namespace Efreshli.Application.Services.EmailService
{
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(string email, string token, string UserFirstName);
        Task SendPasswordResetEmailAsync(string email, string token);
    }
}
