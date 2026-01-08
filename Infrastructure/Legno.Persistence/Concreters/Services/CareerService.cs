using AutoMapper;
using Legno.Application.Absrtacts.Services;
using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Career;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using Legno.Domain.HelperEntities;
using Microsoft.AspNetCore.Http;

namespace Legno.Persistence.Concreters.Services
{
    public class CareerService : ICareerService
    {
        private readonly ICareerReadRepository _read;
        private readonly ICareerWriteRepository _write;
        private readonly IFileService _file;
        private readonly IMailService _mail;
        private readonly IMapper _mapper;

        public CareerService(
            ICareerReadRepository read,
            ICareerWriteRepository write,
            IFileService file,
            IMailService mail,
            IMapper mapper)
        {
            _read = read;
            _write = write;
            _file = file;
            _mail = mail;
            _mapper = mapper;
        }
        public async Task<CareerDto> AddCareerAsync(CreateCareerDto dto)
        {
            if (dto == null)
                throw new GlobalAppException("Məlumat göndərilməyib.");

            var entity = _mapper.Map<Career>(dto);
            entity.Id = Guid.NewGuid();
            entity.CreatedDate = DateTime.UtcNow;
            entity.IsDeleted = false;

            // 📌 CV faylını serverə yüklə
            if (dto.FileName != null)
                entity.FileName = await _file.UploadFile(dto.FileName, "career/cv");

            await _write.AddAsync(entity);
            await _write.CommitAsync();


            // ========== EMAIL UI ==========
            string emailBody = $@"
<!doctype html>
<html lang='az'>
<head>
  <meta charset='UTF-8'>
  <title>Yeni Karyera Müraciəti</title>
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
     background: #707070;
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
      <img src='https://legnoback.online/files/assets/legno.webp' alt='Logo'>
    </div>
    <div class='content'>
      <img src='https://cdn-icons-png.flaticon.com/512/1827/1827392.png' alt='Notification'>
      <h2>Yeni Karyera Müraciəti</h2>

      <div class='info'><strong>Ad:</strong> {dto.Name}</div>
      <div class='info'><strong>Soyad:</strong> {dto.Surname}</div>
      <div class='info'><strong>Email:</strong> <a href='mailto:{dto.Email}'>{dto.Email}</a></div>
      <div class='info'><strong>Telefon:</strong> <a href='tel:{dto.PhoneNumber}'>{dto.PhoneNumber}</a></div>
      <div class='info'><strong>Doğum tarixi:</strong> {dto.BirthDate}</div>
      <div class='info'><strong>Təcrübə:</strong> {dto.WorkExperience}</div>
    </div>

    <div class='footer'>
      Bu mesaj Legno tərəfindən avtomatik göndərilmişdir.
    </div>
  </div>
</body>
</html>
";


            // 📎 CV-ni mailə attachment kimi əlavə et
            var mail = new MailRequest
            {
                ToEmail = "hr@legno.az",
                Subject = "Yeni Karyera Müraciəti",
                Body = emailBody,
                Attachments = dto.FileName != null
                    ? new List<IFormFile> { dto.FileName }
                    : null
            };

            await _mail.SendFromHRAsync(mail);

            return _mapper.Map<CareerDto>(entity);
        }



        public async Task<CareerDto?> GetCareerAsync(string id)
        {
            if (!Guid.TryParse(id, out var gid))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == gid && !x.IsDeleted)
                ?? throw new GlobalAppException("Müraciət tapılmadı.");

            return _mapper.Map<CareerDto>(entity);
        }

        public async Task<List<CareerDto>> GetAllCareersAsync()
        {
            var list = await _read.GetAllAsync(x => !x.IsDeleted);
            return _mapper.Map<List<CareerDto>>(list);
        }

        public async Task<CareerDto> UpdateCareerAsync(UpdateCareerDto dto)
        {
            if (!Guid.TryParse(dto.Id, out var gid))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == gid && !x.IsDeleted)
                ?? throw new GlobalAppException("Müraciət tapılmadı.");

            _mapper.Map(dto, entity);

            if (dto.FileName != null)
            {
                if (!string.IsNullOrEmpty(entity.FileName))
                    await _file.DeleteFile("career/cv", entity.FileName);

                entity.FileName = await _file.UploadFile(dto.FileName, "career/cv");
            }

            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<CareerDto>(entity);
        }

        public async Task DeleteCareerAsync(string id)
        {
            if (!Guid.TryParse(id, out var gid))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == gid && !x.IsDeleted)
                ?? throw new GlobalAppException("Müraciət tapılmadı.");

            if (!string.IsNullOrEmpty(entity.FileName))
                await _file.DeleteFile("career/cv", entity.FileName);

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
