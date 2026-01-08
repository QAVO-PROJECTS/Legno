using AutoMapper.Internal;
using Legno.Domain.HelperEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Absrtacts.Services
{
    public interface IMailService
    {
        Task SendFromInfoAsync(MailRequest mailRequest);
        Task SendFromHRAsync(MailRequest mailRequest);
    }

}
