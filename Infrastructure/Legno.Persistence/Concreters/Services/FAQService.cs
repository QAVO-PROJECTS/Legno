using AutoMapper;
using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Repositories.FAQs;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.FAQ;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Legno.Persistence.Concreters.Services
{
    public class FAQService : IFAQService
    {
        private readonly IFAQReadRepository _faqReadRepository;
        private readonly IFAQWriteRepository _faqWriteRepository;
        private readonly IMapper _mapper;

        public FAQService(
            IFAQReadRepository faqReadRepository,
           IFAQWriteRepository faqWriteRepository,
            IMapper mapper)
        {
            _faqReadRepository = faqReadRepository;
            _faqWriteRepository = faqWriteRepository;
            _mapper = mapper;
        }

        public async Task<FAQDto> AddFAQAsync(CreateFAQDto createFAQDto)
        {
            if (createFAQDto == null)
                throw new GlobalAppException("Məlumat göndərilməyib.");

            var entity = _mapper.Map<FAQ>(createFAQDto);
            entity.Id = Guid.NewGuid();
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _faqWriteRepository.AddAsync(entity);
            await _faqWriteRepository.CommitAsync();

            return _mapper.Map<FAQDto>(entity);
        }

        public async Task<FAQDto?> GetFAQAsync(string faqId)
        {
            if (string.IsNullOrWhiteSpace(faqId))
                throw new GlobalAppException("Id tələb olunur.");

            var entity = await _faqReadRepository.GetByIdAsync(faqId, EnableTraking: false);
            if (entity == null || entity.IsDeleted)
                throw new GlobalAppException("FAQ tapılmadı.");

            return _mapper.Map<FAQDto>(entity);
        }

        public async Task<List<FAQDto>> GetAllFAQsAsync()
        {
            var list = await _faqReadRepository.GetAllAsync(
                func: x => !x.IsDeleted,
                include: null,
                orderBy: q => q.OrderByDescending(x => x.CreatedDate), // ən son əvvəl
                EnableTraking: false
            );

            return _mapper.Map<List<FAQDto>>(list);
        }

        public async Task<FAQDto> UpdateFAQAsync(UpdateFAQDto updateFAQDto)
        {
            if (updateFAQDto == null || string.IsNullOrWhiteSpace(updateFAQDto.Id))
                throw new GlobalAppException("Id tələb olunur.");

            var entity = await _faqReadRepository.GetByIdAsync(updateFAQDto.Id, EnableTraking: true);
            if (entity == null || entity.IsDeleted)
                throw new GlobalAppException("FAQ tapılmadı.");

            // Manual update: null gələnləri toxunmuruq
            if (!string.IsNullOrWhiteSpace(updateFAQDto.Question))
                entity.Question = updateFAQDto.Question;

            if (!string.IsNullOrWhiteSpace(updateFAQDto.QuestionEng))
                entity.QuestionEng = updateFAQDto.QuestionEng;

            if (!string.IsNullOrWhiteSpace(updateFAQDto.QuestionRu))
                entity.QuestionRu = updateFAQDto.QuestionRu;

            if (!string.IsNullOrWhiteSpace(updateFAQDto.Answer))
                entity.Answer = updateFAQDto.Answer;

            if (!string.IsNullOrWhiteSpace(updateFAQDto.AnswerEng))
                entity.AnswerEng = updateFAQDto.AnswerEng;

            if (!string.IsNullOrWhiteSpace(updateFAQDto.AnswerRu))
                entity.AnswerRu = updateFAQDto.AnswerRu;

            entity.LastUpdatedDate = DateTime.UtcNow;

            await _faqWriteRepository.UpdateAsync(entity);
            await _faqWriteRepository.CommitAsync();

            return _mapper.Map<FAQDto>(entity);
        }


        public async Task DeleteFAQAsync(string faqId)
        {
            if (string.IsNullOrWhiteSpace(faqId))
                throw new GlobalAppException("Id tələb olunur.");

            var entity = await _faqReadRepository.GetByIdAsync(faqId, EnableTraking: true);
            if (entity == null || entity.IsDeleted)
                throw new GlobalAppException("FAQ tapılmadı.");

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;

            await _faqWriteRepository.UpdateAsync(entity);
            await _faqWriteRepository.CommitAsync();
        }
    }
}
