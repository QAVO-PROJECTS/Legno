
using Legno.Application.Abstracts.Repositories.Blogs;
using Legno.Domain.Entities;
using Legno.Persistence.Context;

namespace Legno.Persistence.Concreters.Repositories.Blogs
{
    public class BlogWriteRepository : WriteRepository<Blog>, IBlogWriteRepository
    {
        public BlogWriteRepository(LegnoDbContext context) : base(context) { }
    }
}