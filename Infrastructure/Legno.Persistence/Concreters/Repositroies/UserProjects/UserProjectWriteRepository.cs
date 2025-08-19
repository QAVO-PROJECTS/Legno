using Legno.Application.Abstracts.Repositories.UserProjects;
using Legno.Domain.Entities;
using Legno.Persistence.Context;

namespace Legno.Persistence.Concreters.Repositories.UserProjects
{
    public class UserProjectWriteRepository : WriteRepository<UserProject>, IUserProjectWriteRepository
    {
        public UserProjectWriteRepository(LegnoDbContext context) : base(context) { }
    }
}