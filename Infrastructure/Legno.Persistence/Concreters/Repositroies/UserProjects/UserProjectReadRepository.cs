using Legno.Application.Abstracts.Repositories.UserProjects;
using Legno.Domain.Entities;
using Legno.Persistence.Context;

namespace Legno.Persistence.Concreters.Repositories.UserProjects
{
    public class UserProjectReadRepository : ReadRepository<UserProject>, IUserProjectReadRepository
    {
        public UserProjectReadRepository(LegnoDbContext context) : base(context) { }
    }
}