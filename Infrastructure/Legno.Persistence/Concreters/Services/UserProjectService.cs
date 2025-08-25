using AutoMapper;
using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Userproject;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using Legno.Application.Absrtacts.Services;
using Legno.Domain.HelperEntities;
using Legno.Application.Abstracts.Repositories.UserProjects;

namespace Legno.Persistence.Concreters.Services
{
    public class UserProjectService : IUserProjectService
    {
        private readonly IUserProjectReadRepository _readRepo;
        private readonly IUserProjectWriteRepository _writeRepo;
        private readonly IFileService _fileService;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly CloudinaryService _cloudinaryService;



        public UserProjectService(
            IUserProjectReadRepository readRepo,
        IUserProjectWriteRepository writeRepo,
            IFileService fileService,
            IMailService mailService,
            IMapper mapper,
            IConfiguration config,
            CloudinaryService cloudinaryService)
        {
            _readRepo = readRepo;
            _writeRepo = writeRepo;
            _fileService = fileService;
            _mailService = mailService;
            _mapper = mapper;
            _config = config;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<UserProjectDto> AddUserProjectAsync(CreateUserProjectDto dto)
        {
            if (dto == null) throw new GlobalAppException("Məlumat göndərilməyib.");
            if (dto.ProjectFileName == null || dto.ProjectFileName.Length == 0)
                throw new GlobalAppException("Layihə faylı tələb olunur.");

            // 1) Faylı saxla
            var storedFileName = await _cloudinaryService.UploadFileAsync(dto.ProjectFileName);

            // 2) Entity yarat
            var entity = _mapper.Map<UserProject>(dto);
            entity.Id = Guid.NewGuid();
            entity.ProjectFileName = storedFileName;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;
            entity.IsDeleted = false;

            await _writeRepo.AddAsync(entity);
            await _writeRepo.CommitAsync();
            var mailRequest = new MailRequest
            {
                ToEmail = "bd7bl34lt@code.edu.az",
                Subject = "Yeni sifarişiniz var",
                Body = BuildAdminEmailBody(entity)
            };
   
            await _mailService.SendEmailAsync(mailRequest);

            return _mapper.Map<UserProjectDto>(entity);
        }

        public async Task<UserProjectDto?> GetUserProjectAsync(string id)
        {
            var entity = await _readRepo.GetByIdAsync(id, EnableTraking: false);
            if (entity == null || entity.IsDeleted)
                throw new GlobalAppException("Məlumat tapılmadı.");
            return _mapper.Map<UserProjectDto>(entity);
        }

        public async Task<List<UserProjectDto>> GetAllUserProjectsAsync()
        {
            var list = await _readRepo.GetAllAsync(
                func: x => !x.IsDeleted,
                include: null,
                orderBy: q => q.OrderByDescending(x => x.CreatedDate),
                EnableTraking: false
            );
            return _mapper.Map<List<UserProjectDto>>(list);
        }

        public async Task<UserProjectDto> UpdateUserProjectAsync(UpdateUserProjectDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
                throw new GlobalAppException("Id tələb olunur.");

            var entity = await _readRepo.GetByIdAsync(dto.Id, EnableTraking: true);
            if (entity == null || entity.IsDeleted)
                throw new GlobalAppException("Məlumat tapılmadı.");

            // Sahələri (null olmayanları) kopyala
            _mapper.Map(dto, entity);

            // Fayl yenilənirsə – köhnəni sil, yenisini yaz
            if (dto.ProjectFileName != null && dto.ProjectFileName.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(entity.ProjectFileName))
                    await _cloudinaryService.DeleteFileAsync(entity.ProjectFileName);

                entity.ProjectFileName = await _cloudinaryService.UploadFileAsync(dto.ProjectFileName);
            }

            entity.LastUpdatedDate = DateTime.UtcNow;

            await _writeRepo.UpdateAsync(entity);
            await _writeRepo.CommitAsync();

            return _mapper.Map<UserProjectDto>(entity);
        }

        public async Task DeleteUserProjectAsync(string id)
        {
            var entity = await _readRepo.GetByIdAsync(id, EnableTraking: true);
            if (entity == null || entity.IsDeleted)
                throw new GlobalAppException("Məlumat tapılmadı.");

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;

            await _writeRepo.UpdateAsync(entity);
            await _writeRepo.CommitAsync();
        }

        private static string BuildAdminEmailBody(UserProject entity)
        {
            var phone = WebUtility.HtmlEncode(entity.PhoneNumber ?? "");
            var desc = WebUtility.HtmlEncode(entity.Description ?? "");
            var fileName = WebUtility.HtmlEncode(entity.ProjectFileName ?? "");

            // Fayl URL-i artıq avtomatik
            var fileUrl = WebUtility.HtmlEncode(entity.ProjectFileName ?? "#");

            return $@"
<!doctype html>
<html lang='az'>
<head>
  <meta charset='UTF-8'>
  <title>Yeni Sifariş</title>
  <style>
    body {{
      font-family: Arial, sans-serif;
      background-color: #f8f9fa;
      margin: 0; padding: 0;
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
    .header img {{ height: 60px; }}
    .content {{
      padding: 30px;
      text-align: left;
      color: #333;
    }}
    .content h2 {{ text-align:center; margin-top: 0; }}
    .info {{ margin: 12px 0; font-size: 16px; color: #555; }}
    .info strong {{ color: #000; }}
    .cta {{ text-align:center; margin-top: 24px; }}
    .btn {{
      display:inline-block; padding:12px 20px; background:#0D60FE; color:#fff;
      border-radius:6px; text-decoration:none;
    }}
    .footer {{
      padding: 16px; text-align: center; background-color: #f0f0f0;
      font-size: 14px; color: #777;
    }}
    a {{ color:#0D60FE; text-decoration:none; }}
  </style>
</head>
<body>
  <div class='email-container'>
    <div class='header'>
      <img src='https://i.postimg.cc/XXMpxN2z/Frame-290.png' alt='Logo'>
    </div>

    <div class='content'>
      <h2>Yeni sifarişiniz var</h2>
      <div class='info'><strong>Telefon:</strong> {phone}</div>
      <div class='info'><strong>Qeyd:</strong> {desc}</div>
      <div class='info'><strong>Layihə faylı:</strong> <a href='{fileUrl}' target='_blank'>{fileName}</a></div>

      <div class='cta'>
        <a class='btn' href='{fileUrl}' target='_blank'>Faylı aç</a>
      </div>
    </div>

    <div class='footer'>
      Bu mesaj Legno tərəfindən avtomatik göndərilmişdir.
    </div>
  </div>
</body>
</html>";
        }

    }
}
