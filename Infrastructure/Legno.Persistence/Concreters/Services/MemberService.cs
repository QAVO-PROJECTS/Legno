using AutoMapper;
using Legno.Application.Absrtacts.Services;
using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Member;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using Legno.Infrastructure.Concreters.Services;
using Microsoft.EntityFrameworkCore;

namespace Legno.Persistence.Concreters.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberReadRepository _read;
        private readonly IMemberWriteRepository _write;
        private readonly IFileService _file;
        private readonly IMapper _mapper;

        public MemberService(
            IMemberReadRepository read,
            IMemberWriteRepository write,
            IFileService file,
            IMapper mapper)
        {
            _read = read;
            _write = write;
            _file = file;
            _mapper = mapper;
        }

        public async Task<MemberDto> AddMemberAsync(CreateMemberDto dto)
        {
            if (dto == null)
                throw new GlobalAppException("Məlumat göndərilməyib.");

            if (dto.Image == null)
                throw new GlobalAppException("Şəkil daxil edilməlidir!");

            // sonuncu order-i tapırıq
            var all = await _read.GetAllAsync(m => !m.IsDeleted);
            var maxOrder = all.Any() ? all.Max(m => m.DisplayOrderId) : 0;

            var entity = _mapper.Map<Member>(dto);
            entity.Id = Guid.NewGuid();
            entity.DisplayOrderId = maxOrder + 1;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;
            entity.IsDeleted = false;

            entity.Image = await _file.UploadFile(dto.Image, "members");

            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<MemberDto>(entity);
        }
//salam
        public async Task<MemberDto?> GetMemberAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                throw new GlobalAppException("Yanlış ID formatı!");

            var entity = await _read.GetAsync(x => x.Id == guid && !x.IsDeleted);

            return entity == null ? null : _mapper.Map<MemberDto>(entity);
        }

        public async Task<List<MemberDto>> GetAllMembersAsync()
        {
            var members = await _read.GetAllAsync(
                x => !x.IsDeleted,  // Filter deleted members
                orderBy: q => q.OrderBy(m => m.DisplayOrderId),  // Sort by DisplayOrderId
                EnableTraking: false
            );
            return _mapper.Map<List<MemberDto>>(members);
        }

        public async Task<MemberDto> UpdateMemberAsync(UpdateMemberDto dto)
        {
            if (!Guid.TryParse(dto.Id, out var guid))
                throw new GlobalAppException("Yanlış ID!");

            var entity = await _read.GetAsync(
                x => x.Id == guid && !x.IsDeleted,
                EnableTraking: true
            ) ?? throw new GlobalAppException("Üzv tapılmadı!");

            // ================= IMAGE UPDATE =================
            if (dto.Image != null && dto.Image.Length > 0)
            {
                // köhnə şəkli sil
                if (!string.IsNullOrWhiteSpace(entity.Image))
                    await _file.DeleteFile("members", entity.Image);

                // yeni şəkli yüklə
                entity.Image = await _file.UploadFile(dto.Image, "members");
            }

            // ================= TEXT FIELDS =================
            if (dto.Title != null) entity.Title = dto.Title;
            if (dto.TitleEng != null) entity.TitleEng = dto.TitleEng;
            if (dto.TitleRu != null) entity.TitleRu = dto.TitleRu;
            if (dto.JobTitle != null) entity.JobTitle = dto.JobTitle;
            if (dto.JobTitleEng != null) entity.JobTitleEng = dto.JobTitleEng;
            if (dto.JobTitleRu != null) entity.JobTitleRu = dto.JobTitleRu;

            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<MemberDto>(entity);
        }

        public async Task DeleteMemberAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                throw new GlobalAppException("Yanlış ID!");

            var entity = await _read.GetAsync(x => x.Id == guid && !x.IsDeleted)
                ?? throw new GlobalAppException("Üzv tapılmadı!");

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }

        public async Task ReorderMembersAsync(List<MemberOrderUpdateDto> orders)
        {
            if (orders == null || orders.Count == 0) return;

            var idMap = new Dictionary<Guid, int>();
            foreach (var o in orders)
            {
                if (!Guid.TryParse(o.MemberId, out var gid))
                    throw new GlobalAppException($"Yanlış ID: {o.MemberId}");
                idMap[gid] = o.DisplayOrderId;
            }

            var members = await _read.GetAllAsync(m => idMap.Keys.Contains(m.Id) && !m.IsDeleted, EnableTraking: true);
            foreach (var m in members)
            {
                m.DisplayOrderId = idMap[m.Id];
                m.LastUpdatedDate = DateTime.UtcNow;
                await _write.UpdateAsync(m);
            }

            await _write.CommitAsync();
        }
    }
}
