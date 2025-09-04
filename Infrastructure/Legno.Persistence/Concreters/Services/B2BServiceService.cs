using AutoMapper;
using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.CommonService;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Persistence.Concreters.Services
{
    public class B2BServiceService : IB2BServiceService
    {
        private readonly IB2BServiceReadRepository _read;
        private readonly IB2BServiceWriteRepository _write;
        private readonly IMapper _mapper;

        public B2BServiceService(
            IB2BServiceReadRepository read,
            IB2BServiceWriteRepository write,
            IMapper mapper)
        {
            _read = read;
            _write = write;
            _mapper = mapper;
        }

        public async Task<CommonServiceDto> AddCommonServiceAsync(CreateCommonServiceDto createDto)
        {
            if (createDto == null) throw new GlobalAppException("Məlumat göndərilməyib.");

            var entity = _mapper.Map<B2BService>(createDto);
            entity.Id = Guid.NewGuid();
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<CommonServiceDto>(entity);
        }

        public async Task<CommonServiceDto?> GetCommonServiceAsync(string id)
        {
            if (!Guid.TryParse(id, out var gid))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == gid && !x.IsDeleted, EnableTraking: false);
            if (entity == null) throw new GlobalAppException("Xidmət tapılmadı.");

            return _mapper.Map<CommonServiceDto>(entity);
        }

        public async Task<List<CommonServiceDto>> GetAllCommonServicesAsync()
        {
            var list = await _read.GetAllAsync(x => !x.IsDeleted, EnableTraking: false,
                orderBy: q => q.OrderBy(x => x.CreatedDate));
            return list.Select(_mapper.Map<CommonServiceDto>).ToList();
        }

        public async Task<CommonServiceDto> UpdateCommonServiceAsync(UpdateCommonServiceDto updateDto)
        {
            if (updateDto == null || !Guid.TryParse(updateDto.Id, out var gid))
                throw new GlobalAppException("Yanlış ID.");

            var entity = await _read.GetAsync(x => x.Id == gid && !x.IsDeleted, EnableTraking: true)
                        ?? throw new GlobalAppException("Xidmət tapılmadı.");

            // yalnız göndərilən sahələr (Profile-də null-ignore olmalıdır)
            _mapper.Map(updateDto, entity);
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<CommonServiceDto>(entity);
        }

        public async Task DeleteCommonServiceAsync(string id)
        {
            if (!Guid.TryParse(id, out var gid))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == gid && !x.IsDeleted, EnableTraking: true)
                        ?? throw new GlobalAppException("Xidmət tapılmadı.");

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
