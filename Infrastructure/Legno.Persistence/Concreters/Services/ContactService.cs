using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using Legno.Application.Absrtacts.Services;
using Legno.Application.Dtos.Contact;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using Legno.Domain.HelperEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Legno.Application.Abstracts.Repositories.Contacts;

namespace Legno.Persistence.Concreters.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactReadRepository _contactReadRepository;
        private readonly IContactWriteRepository _contactWriteRepository;
        private readonly IMailService _mailService;
        private string _adminEmail = "";
        private readonly IConfiguration _configuration;

        public ContactService(
            IContactReadRepository contactReadRepository,
            IContactWriteRepository contactWriteRepository,
            IMailService mailService,
            IOptions<MailSettings> mailSettings,
            IConfiguration configuration)
        {
            _contactReadRepository = contactReadRepository;
            _contactWriteRepository = contactWriteRepository;
            _mailService = mailService;
            
            _configuration = configuration;
        }

        public async Task<ContactDto> CreateUserInfoAsync(CreateContactDto userDto)
        {
            
            if (string.IsNullOrWhiteSpace(userDto.Name) ||
                string.IsNullOrWhiteSpace(userDto.Surname) ||
             string.IsNullOrWhiteSpace(userDto.Email) ||
            string.IsNullOrWhiteSpace(userDto.PhoneNumber) ||
                 string.IsNullOrWhiteSpace(userDto.Description))
            {
                throw new GlobalAppException("Bütün inputlar doldurulmalıdır.");
            }

            // **Artıq "mövcud email" yoxlamasını tamamilə çıxarırıq.**

            // 1) Yeni Contact obyekti yaradıb bazaya əlavə edirik:
            var newUser = new Contact
            {
                Name = userDto.Name,
                Surname = userDto.Surname,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                Description = userDto.Description,
                CreatedDate = DateTime.UtcNow
            };

            await _contactWriteRepository.AddAsync(newUser);
            await _contactWriteRepository.CommitAsync();

            // 2) Adminə göndəriləcək email məzmunu (birinci emailBody):
            string emailBody = $@"
<!doctype html>
<html lang='az'>
<head>
  <meta charset='UTF-8'>
  <title>Yeni Müraciət</title>
  <style>
    body {{
      font-family: Arial, sans-serif;
      background-color: #f8f9fa;
      margin: 0;
      padding: 0;
    }}
    .email-container {{
      max-width: 600px;
      margin: 40px auto;
      background-color: #ffffff;
      border: 1px solid #ddd;
      border-radius: 8px;
      overflow: hidden;
    }}
    .header {{
      background-color: #f0f0f0;
      padding: 20px;
      text-align: center;
    }}
    .header img {{
      height: 60px;
    }}
    .content {{
      padding: 30px;
      text-align: center;
    }}
    .content img {{
      width: 100px;
      margin-bottom: 20px;
    }}
    .content h2 {{
      color: #333;
      margin-bottom: 20px;
    }}
    .info {{
      text-align: left;
      font-size: 16px;
      color: #555;
      margin-bottom: 15px;
    }}
    .info strong {{
      color: #000;
    }}
    .footer {{
      padding: 20px;
      text-align: center;
      background-color: #f0f0f0;
      font-size: 14px;
      color: #777;
    }}
  </style>
</head>
<body>
  <div class='email-container'>
    <div class='header'>
      <img src='https://i.postimg.cc/XXMpxN2z/Frame-290.png' alt='Logo'>
    </div>
    <div class='content'>
      <img src='https://cdn-icons-png.flaticon.com/512/1827/1827392.png
' alt='Notification'>
      <h2>Yeni müraciət daxil olub</h2>
      <div class='info'><strong>Ad:</strong> {userDto.Name}</div>
      <div class='info'><strong>Soyad:</strong> {userDto.Surname}</div>
      <div class='info'><strong>Email:</strong> <a href='mailto:{userDto.Email}'>{userDto.Email}</a></div>
      <div class='info'><strong>Telefon:</strong> <a href='tel:{userDto.PhoneNumber}'>{userDto.PhoneNumber}</a></div>
      <div class='info'><strong>Müraciət:</strong> {userDto.Description}</div>
    </div>
    <div class='footer'>
      Bu mesaj Legno tərəfindən avtomatik göndərilmişdir.
    </div>
  </div>
</body>
</html>
";



            // 3) Adminə email göndər:
            //var mailRequest = new MailRequest
            //{
            //    ToEmail = "bd7bl34lt@code.edu.az",
            //    Subject = "Yeni İstifadəçi Məlumatları",
            //    Body = emailBody
            //};
            //await _mailService.SendEmailAsync(mailRequest);

            // 4) Cavab olaraq yeni yaradılmış `ContactDto` qaytarırıq:
            return new ContactDto
            {
                Id = newUser.Id.ToString(),
                Name = newUser.Name,
                Surname = newUser.Surname,
                Email = newUser.Email,
                PhoneNumber = newUser.PhoneNumber,
                Description = newUser.Description
            };
        }

        public async Task<List<ContactDto>> GetAllUsersAsync()
        {
            var users = await _contactReadRepository.GetAllAsync();

            var userDtos = users.Select(u => new ContactDto
            {
                Id = u.Id.ToString(),
                Name = u.Name,
                Surname = u.Surname,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Description = u.Description
            }).ToList();

            return userDtos;
        }
    }
}
