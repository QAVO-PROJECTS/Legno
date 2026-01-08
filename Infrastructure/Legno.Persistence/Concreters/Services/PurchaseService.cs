using AutoMapper;
using Legno.Application.Absrtacts.Services;
using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Purchase;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using Legno.Domain.HelperEntities;
using Microsoft.AspNetCore.Http;

namespace Legno.Persistence.Concreters.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseReadRepository _read;
        private readonly IPurchaseWriteRepository _write;
        private readonly IFileService _file;
        private readonly IMailService _mail;
        private readonly IMapper _mapper;

        public PurchaseService(
            IPurchaseReadRepository read,
            IPurchaseWriteRepository write,
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

        public async Task<PurchaseDto> AddPurchaseAsync(CreatePurchaseDto dto)
        {
            if (dto == null)
                throw new GlobalAppException("Məlumat göndərilməyib.");

            var entity = _mapper.Map<Purchase>(dto);
            entity.Id = Guid.NewGuid();
            entity.CreatedDate = DateTime.UtcNow;
            entity.IsDeleted = false;

            // 🔥 Faylı serverə save edirik
            if (dto.FileName != null)
                entity.FileName = await _file.UploadFile(dto.FileName, "purchase/files");

            await _write.AddAsync(entity);
            await _write.CommitAsync();


            // ================= EMAIL UI =================
            string emailBody = $@"
<!doctype html>
<html lang='az'>
<head>
<meta charset='UTF-8'>
<title>Yeni Satınalma Sorğusu</title>
<style>
 body {{font-family: Arial;background:#f8f9fa}}
 .card {{max-width:650px;margin:30px auto;background:#fff;border:1px solid #ddd;border-radius:8px}}
 .head {{background:#707070;padding:22px;text-align:center}}
 .head img {{height:60px}}
 .body{{padding:28px}}
 .body h2{{text-align:center}}
 .footer{{background:#f0f0f0;padding:16px;text-align:center;color:#777}}
 .row{{margin:10px 0}}
 .row b{{color:#000}}
</style>
</head>
<body>
<div class='card'>
<div class='head'>
<img src='https://legnoback.online/files/assets/legno.webp'>
</div>

<div class='body'>
<h2>Yeni Satınalma Sorğusu</h2>

<div class='row'><b>Şirkət:</b> {dto.CompanyName}</div>
<div class='row'><b>Başlıq:</b> {dto.Subtitle}</div>
<div class='row'><b>Məhsul/Xidmət:</b> {dto.ProductOrService}</div>
<div class='row'><b>Email:</b> <a href='mailto:{dto.Email}'>{dto.Email}</div>
<div class='row'><b>Telefon:</b> <a href='tel:{dto.PhoneNumber}'>{dto.PhoneNumber}</div>

<p style='margin-top:15px;color:#777'>
Fayl mailə əlavə olunmuşdur.
</p>
</div>

<div class='footer'>
Bu mesaj Legno tərəfindən avtomatik göndərilmişdir.
</div>
</div>
</body>
</html>
";


            // 🔥🔥🔥 FAYLI MAILƏ ATTACH EDİRİK 🔥🔥🔥
            await _mail.SendFromHRAsync(new MailRequest
            {
                ToEmail = "hr@legno.az",
                Subject = "Yeni Satınalma Sorğusu",
                Body = emailBody,
                Attachments = dto.FileName != null
                    ? new List<IFormFile> { dto.FileName }
                    : null
            });

            return _mapper.Map<PurchaseDto>(entity);
        }


        public async Task<PurchaseDto?> GetPurchaseAsync(string id)
        {
            if (!Guid.TryParse(id, out var gid))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == gid && !x.IsDeleted)
                ?? throw new GlobalAppException("Sorğu tapılmadı.");

            return _mapper.Map<PurchaseDto>(entity);
        }

        public async Task<List<PurchaseDto>> GetAllPurchasesAsync()
        {
            var list = await _read.GetAllAsync(x => !x.IsDeleted);
            return _mapper.Map<List<PurchaseDto>>(list);
        }

        public async Task<PurchaseDto> UpdatePurchaseAsync(UpdatePurchaseDtos dto)
        {
            if (!Guid.TryParse(dto.Id, out var gid))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == gid && !x.IsDeleted)
                ?? throw new GlobalAppException("Sorğu tapılmadı.");

            _mapper.Map(dto, entity);

            if (dto.FileName != null)
            {
                if (!string.IsNullOrEmpty(entity.FileName))
                    await _file.DeleteFile("purchase/files", entity.FileName);

                entity.FileName = await _file.UploadFile(dto.FileName, "purchase/files");
            }

            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<PurchaseDto>(entity);
        }

        public async Task DeletePurchaseAsync(string id)
        {
            if (!Guid.TryParse(id, out var gid))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == gid && !x.IsDeleted)
                ?? throw new GlobalAppException("Sorğu tapılmadı.");

            if (!string.IsNullOrEmpty(entity.FileName))
                await _file.DeleteFile("purchase/files", entity.FileName);

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
