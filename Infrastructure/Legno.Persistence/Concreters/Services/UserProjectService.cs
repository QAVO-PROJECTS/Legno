using AutoMapper;
using Legno.Application.Absrtacts.Services;
using Legno.Application.Abstracts.Repositories.UserProjects;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Userproject;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using System.Net;

namespace Legno.Persistence.Concreters.Services
{
    public class UserProjectService : IUserProjectService
    {
        private readonly IUserProjectReadRepository _read;
        private readonly IUserProjectWriteRepository _write;
        private readonly IFileService _fileService;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;

        public UserProjectService(
            IUserProjectReadRepository read,
            IUserProjectWriteRepository write,
            IFileService fileService,
            IMailService mailService,
            IMapper mapper)
        {
            _read = read;
            _write = write;
            _fileService = fileService;
            _mailService = mailService;
            _mapper = mapper;
        }

        public async Task<UserProjectDto> AddUserProjectAsync(CreateUserProjectDto dto)
        {
            if (dto == null) throw new GlobalAppException("Məlumat göndərilməyib.");
            if (dto.ProjectFileName == null || dto.ProjectFileName.Length == 0)
                throw new GlobalAppException("Layihə faylı tələb olunur.");

            var storedFile = await _fileService.UploadFile(dto.ProjectFileName, "user-projects");

            var entity = _mapper.Map<UserProject>(dto);
            entity.Id = Guid.NewGuid();
            entity.ProjectFileName = storedFile;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;
            entity.IsDeleted = false;

            await _write.AddAsync(entity);
            await _write.CommitAsync();

            // Optional: mail göndərmək
            // await _mailService.SendEmailAsync(new MailRequest { ... });

            return _mapper.Map<UserProjectDto>(entity);
        }

        public async Task<UserProjectDto?> GetUserProjectAsync(string id)
        {
            var entity = await _read.GetByIdAsync(id, EnableTraking: false);
            if (entity == null || entity.IsDeleted)
                throw new GlobalAppException("Layihə tapılmadı.");
            return _mapper.Map<UserProjectDto>(entity);
        }

        public async Task<List<UserProjectDto>> GetAllUserProjectsAsync()
        {
            var list = await _read.GetAllAsync(
                func: x => !x.IsDeleted,
                orderBy: q => q.OrderByDescending(x => x.CreatedDate),
                EnableTraking: false);

            return _mapper.Map<List<UserProjectDto>>(list);
        }

        public async Task<UserProjectDto> UpdateUserProjectAsync(UpdateUserProjectDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
                throw new GlobalAppException("Id tələb olunur.");

            var entity = await _read.GetByIdAsync(dto.Id, EnableTraking: true)
                ?? throw new GlobalAppException("Məlumat tapılmadı.");

            _mapper.Map(dto, entity);

            if (dto.ProjectFileName != null)
            {
                if (!string.IsNullOrWhiteSpace(entity.ProjectFileName))
                    await _fileService.DeleteFile("user-projects", entity.ProjectFileName);

                entity.ProjectFileName = await _fileService.UploadFile(dto.ProjectFileName, "user-projects");
            }

            entity.LastUpdatedDate = DateTime.UtcNow;
            await _write.UpdateAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<UserProjectDto>(entity);
        }

        public async Task DeleteUserProjectAsync(string id)
        {
            var entity = await _read.GetByIdAsync(id, EnableTraking: true)
                ?? throw new GlobalAppException("Layihə tapılmadı.");

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
