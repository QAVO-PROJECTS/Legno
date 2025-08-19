
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Abstracts.Services
{
    public interface ISubscriberService
    {
        Task AddSubscriberAsync(string email);
        Task<string?> GetSubscriberAsync(string subscriberId);
        Task<List<string>> GetAllSubscribersAsync();
        Task DeleteSubscriberAsync(string subscriberId);
    }
}
