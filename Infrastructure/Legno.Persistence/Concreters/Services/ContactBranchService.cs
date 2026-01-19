using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Repositories.Contacts;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.ContactBranch;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;

namespace Legno.Persistence.Concreters.Services
{
    public class ContactBranchService : IContactBranchService
    {
        private readonly IContactBranchReadRepository _read;
        private readonly IContactBranchWriteRepository _write;

        public ContactBranchService(
            IContactBranchReadRepository read,
            IContactBranchWriteRepository write)
        {
            _read = read;
            _write = write;
        }

        // ================= CREATE =================
        public async Task<ContactBranchDto> CreateAsync(CreateContactBranchDto dto)
        {
            if (dto == null)
                throw new GlobalAppException("Məlumat göndərilməyib.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new GlobalAppException("Filial adı boş ola bilməz!");

            var entity = new ContactBranch
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Address = dto.Address,
                Phone = dto.Phone,
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return new ContactBranchDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                Address = entity.Address,
                Phone = entity.Phone
            };
        }

        // ================= GET ONE =================
        public async Task<ContactBranchDto?> GetAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                throw new GlobalAppException("Yanlış ID formatı!");

            var entity = await _read.GetAsync(x => x.Id == guid && !x.IsDeleted);

            return entity == null ? null : new ContactBranchDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                Address = entity.Address,
                Phone = entity.Phone
            };
        }

        // ================= GET ALL =================
        public async Task<List<ContactBranchDto>> GetAllAsync()
        {
            var list = await _read.GetAllAsync(x => !x.IsDeleted, EnableTraking: false);

            return list.Select(x => new ContactBranchDto
            {
                Id = x.Id.ToString(),
                Name = x.Name,
                Address = x.Address,
                Phone = x.Phone
            }).ToList();
        }

        // ================= UPDATE =================
        public async Task<ContactBranchDto> UpdateAsync(UpdateContactBranchDto dto)
        {
            if (dto == null)
                throw new GlobalAppException("Məlumat göndərilməyib.");

            if (!Guid.TryParse(dto.Id, out var guid))
                throw new GlobalAppException("Yanlış ID!");

            var entity = await _read.GetAsync(x => x.Id == guid && !x.IsDeleted, EnableTraking: true)
                ?? throw new GlobalAppException("Filial tapılmadı!");

            // field update (nullable update dto)
            if (dto.Name != null) entity.Name = dto.Name;
            if (dto.Address != null) entity.Address = dto.Address;
            if (dto.Phone != null) entity.Phone = dto.Phone;

            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();

            return new ContactBranchDto
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                Address = entity.Address,
                Phone = entity.Phone
            };
        }

        // ================= DELETE =================
        public async Task DeleteAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                throw new GlobalAppException("Yanlış ID!");

            var entity = await _read.GetAsync(x => x.Id == guid && !x.IsDeleted, EnableTraking: true)
                ?? throw new GlobalAppException("Filial tapılmadı!");

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
