using Legno.Application.Abstracts.Repositories.Subscribers;
using Legno.Application.Abstracts.Services;
using Legno.Application.GlobalExceptionn;   // Səndə bu namespace işlənirdi; fərqlidirsə dəyiş
using Legno.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Legno.Persistence.Concreters.Services
{
    public class SubscriberService : ISubscriberService
    {
        private readonly ISubscriberReadRepository _subscriberReadRepository;
        private readonly ISubscriberWriteRepository _subscriberWriteRepository;

        public SubscriberService(
            ISubscriberReadRepository subscriberReadRepository,
            ISubscriberWriteRepository subscriberWriteRepository)
        {
            _subscriberReadRepository = subscriberReadRepository;
            _subscriberWriteRepository = subscriberWriteRepository;
        }

        public async Task AddSubscriberAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new GlobalAppException("Email boş ola bilməz.");

            email = email.Trim().ToLowerInvariant();

            // format yoxlaması
            try { _ = new MailAddress(email); }
            catch { throw new GlobalAppException("Email formatı yanlışdır."); }

            // mövcud və aktiv (IsDeleted = false) abunəçi varsa -> xəta
            var existActive = await _subscriberReadRepository.GetAllAsync(
                func: s => s.Email.ToLower() == email && !s.IsDeleted,
                include: null, orderBy: null, EnableTraking: false
            );
            if (existActive.Any())
                throw new GlobalAppException("Bu email artıq abunədir.");

            else
            {
                var entity = new Subscriber
                {
                    Email = email,
                    IsDeleted = false,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdatedDate = DateTime.UtcNow
                };

                await _subscriberWriteRepository.AddAsync(entity);
                await _subscriberWriteRepository.CommitAsync();

            }

                // yeni əlavə et
              
        }


        public async Task<string?> GetSubscriberAsync(string subscriberId)
        {
            var sub = await _subscriberReadRepository.GetByIdAsync(subscriberId, EnableTraking: false);
            if (sub == null || sub.IsDeleted) return null;
            return sub.Email;
        }

        public async Task<List<string>> GetAllSubscribersAsync()
        {
            var subs = await _subscriberReadRepository.GetAllAsync(
                func: s => !s.IsDeleted,
                include: null,
                orderBy: q => q.OrderByDescending(s => s.CreatedDate),
                EnableTraking: false
            );

            return subs.Select(s => s.Email).ToList();
        }

        public async Task DeleteSubscriberAsync(string subscriberId)
        {
            var sub = await _subscriberReadRepository.GetAsync(x=>x.Email==subscriberId);
            if (sub == null || sub.IsDeleted)
                throw new GlobalAppException("Abunəçi tapılmadı.");

            sub.IsDeleted = true;
            sub.DeletedDate = DateTime.UtcNow;
            sub.LastUpdatedDate = DateTime.UtcNow;
            
            await _subscriberWriteRepository.UpdateAsync(sub);
            await _subscriberWriteRepository.CommitAsync();
        }
    }
}
