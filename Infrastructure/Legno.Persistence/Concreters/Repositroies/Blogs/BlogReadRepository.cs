
using Legno.Application.Abstracts.Repositories.Blogs;
using Legno.Domain.Entities;
using Legno.Persistence.Context;

namespace Legno.Persistence.Concreters.Repositories.Blogs
{
    public class BlogReadRepository : ReadRepository<Blog>, IBlogReadRepository
    {
        public BlogReadRepository(LegnoDbContext context) : base(context) { }
    }
}