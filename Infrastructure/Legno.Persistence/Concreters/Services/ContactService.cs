using Legno.Application.Absrtacts.Services;
using Legno.Application.Abstracts.Repositories.Contacts;
using Legno.Application.Dtos.Contact;
using Legno.Application.Dtos.ContactBranch;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using Legno.Domain.HelperEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Legno.Persistence.Concreters.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactReadRepository _contactReadRepository;
        private readonly IContactWriteRepository _contactWriteRepository;
        private readonly IMailService _mailService;
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
            if (userDto == null)
                throw new GlobalAppException("Məlumat göndərilməyib.");

            if (string.IsNullOrWhiteSpace(userDto.Name) ||
                string.IsNullOrWhiteSpace(userDto.Surname) ||
                string.IsNullOrWhiteSpace(userDto.Email) ||
                string.IsNullOrWhiteSpace(userDto.PhoneNumber) ||
                string.IsNullOrWhiteSpace(userDto.Description))
            {
                throw new GlobalAppException("Bütün inputlar doldurulmalıdır.");
            }

            Guid? branchId = null;

            // ContactBranchId optional gəlir
            if (!string.IsNullOrWhiteSpace(userDto.ContactBranchId))
            {
                if (!Guid.TryParse(userDto.ContactBranchId, out var parsed))
                    throw new GlobalAppException("ContactBranchId formatı yanlışdır!");

                branchId = parsed;
            }

            var newUser = new Contact
            {
                Id = Guid.NewGuid(),
                Name = userDto.Name,
                Surname = userDto.Surname,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                Description = userDto.Description,
                ContactBranchId = branchId, // Guid? olmalıdır entity-də
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            await _contactWriteRepository.AddAsync(newUser);
            await _contactWriteRepository.CommitAsync();

            // Admin email body
            string emailBody = $@"
<!doctype html>
<html lang='az'>
<head>
  <meta charset='UTF-8'>
  <title>Yeni Müraciət</title>
</head>
<body>
  <h2>Yeni müraciət daxil oldu</h2>
  <p><strong>Ad:</strong> {userDto.Name}</p>
  <p><strong>Soyad:</strong> {userDto.Surname}</p>
  <p><strong>Email:</strong> {userDto.Email}</p>
  <p><strong>Telefon:</strong> {userDto.PhoneNumber}</p>
  <p><strong>Müraciət:</strong> {userDto.Description}</p>
</body>
</html>
";

            var mailRequest = new MailRequest
            {
                ToEmail = "info@legno.az",
                Subject = "Yeni Əlaqə Məlumatları",
                Body = emailBody
            };

            await _mailService.SendFromInfoAsync(mailRequest);

            // Branch data da qaytarmaq üçün
            ContactBranchDto? branchDto = null;

            if (branchId.HasValue)
            {
                // Read repo Include edirsə ideal olar.
                // Yoxdursa GetById ilə branch repository yazıb çəkə bilərik.
                var saved = await _contactReadRepository.GetAsync(
                    x => x.Id == newUser.Id,
                    include: q => q.Include(x => x.ContactBranch)
                );

                if (saved?.ContactBranch != null)
                {
                    branchDto = new ContactBranchDto
                    {
                        Id = saved.ContactBranch.Id.ToString(),
                        Name = saved.ContactBranch.Name,
                        Address = saved.ContactBranch.Address,
                        Phone = saved.ContactBranch.Phone
                    };
                }
            }

            return new ContactDto
            {
                Id = newUser.Id.ToString(),
                Name = newUser.Name,
                Surname = newUser.Surname,
                Email = newUser.Email,
                PhoneNumber = newUser.PhoneNumber,
                Description = newUser.Description,
                ContactBranch = branchDto
            };
        }

        public async Task<ContactDto> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new GlobalAppException("Id boş ola bilməz.");

            if (!Guid.TryParse(id, out var guid))
                throw new GlobalAppException("Yanlış ID formatı!");

            // include branch
            var contact = await _contactReadRepository.GetAsync(
                x => x.Id == guid,
                include: q => q.Include(x => x.ContactBranch)
            );

            if (contact == null)
                throw new GlobalAppException("Məlumat tapılmadı.");

            return new ContactDto
            {
                Id = contact.Id.ToString(),
                Name = contact.Name,
                Surname = contact.Surname,
                Email = contact.Email,
                PhoneNumber = contact.PhoneNumber,
                Description = contact.Description,
                ContactBranch = contact.ContactBranch == null ? null : new ContactBranchDto
                {
                    Id = contact.ContactBranch.Id.ToString(),
                    Name = contact.ContactBranch.Name,
                    Address = contact.ContactBranch.Address,
                    Phone = contact.ContactBranch.Phone
                }
            };
        }

        public async Task<List<ContactDto>> GetAllUsersAsync()
        {
            var users = await _contactReadRepository.GetAllAsync(
                include: q => q.Include(x => x.ContactBranch)
            );

            return users.Select(u => new ContactDto
            {
                Id = u.Id.ToString(),
                Name = u.Name,
                Surname = u.Surname,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Description = u.Description,
                ContactBranch = u.ContactBranch == null ? null : new ContactBranchDto
                {
                    Id = u.ContactBranch.Id.ToString(),
                    Name = u.ContactBranch.Name,
                    Address = u.ContactBranch.Address,
                    Phone = u.ContactBranch.Phone
                }
            }).ToList();
        }
    }
}
