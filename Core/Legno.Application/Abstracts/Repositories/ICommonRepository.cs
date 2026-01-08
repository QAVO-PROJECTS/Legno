// Abstract
using Legno.Domain.Entities;

namespace Legno.Application.Abstracts.Repositories
{
    public interface IB2BServiceReadRepository : IReadRepository<B2BService> { }
    public interface IB2BServiceWriteRepository : IWriteRepository<B2BService> { }
    public interface IBusinessServiceReadRepository : IReadRepository<BusinessService> { }
    public interface IBusinessServiceWriteRepository : IWriteRepository<BusinessService> { }
    public interface ICommonServiceReadRepository : IReadRepository<CommonService> { }
    public interface ICommonServiceWriteRepository : IWriteRepository<CommonService> { }
    public interface IDesignerCommonServiceReadRepository : IReadRepository<DesignerCommonService> { }
    public interface IDesignerCommonServiceWriteRepository : IWriteRepository<DesignerCommonService> { }
    public interface IDesignerServiceReadRepository : IReadRepository<DesignerService> { }
    public interface IDesignerServiceWriteRepository : IWriteRepository<DesignerService> { }
    public interface IFabricReadRepository : IReadRepository<Fabric> { }
    public interface IProjectSliderImageReadRepository : IReadRepository<ProjectSliderImage> { }
    public interface IProjectFabricReadRepository : IReadRepository<ProjectFabric> { }


    public interface IFabricWriteRepository : IWriteRepository<Fabric> { }
    public interface ILocationReadRepository : IReadRepository<Location> { }
    public interface ILocationWriteRepository : IWriteRepository<Location> { }
    public interface IPartnerReadRepository : IReadRepository<Partner> { }
    public interface IPartnerWriteRepository : IWriteRepository<Partner> { }
    public interface IServiceSliderReadRepository : IReadRepository<ServiceSlider> { }
    public interface IServiceSliderWriteRepository : IWriteRepository<ServiceSlider> { }
    public interface IProjectSliderImageWriteRepository : IWriteRepository<ProjectSliderImage> { }
    public interface IProjectFabricWriteRepository : IWriteRepository<ProjectFabric> { }

    public interface IWorkPlanningReadRepository : IReadRepository<WorkPlanning> { }
    public interface ICategoryImageReadRepository : IReadRepository<CategoryImage> { }
    public interface ICategoryImageWriteRepository : IWriteRepository<CategoryImage> { }

    public interface IWorkPlanningWriteRepository : IWriteRepository<WorkPlanning> { }
    public interface IPurchaseReadRepository : IReadRepository<Purchase> { }
    public interface IPurchaseWriteRepository : IWriteRepository<Purchase> { }

    public interface ICareerReadRepository : IReadRepository<Career> { }
    public interface ICareerWriteRepository : IWriteRepository<Career> { }

    public interface IArticleReadRepository : IReadRepository<Article> { }
    public interface IArticleWriteRepository : IWriteRepository<Article> { }

    public interface IArticleImageReadRepository : IReadRepository<ArticleImage> { }
    public interface IArticleImageWriteRepository : IWriteRepository<ArticleImage> { }

    public interface IAnnouncementReadRepository : IReadRepository<Announcement> { }
    public interface IAnnouncementWriteRepository : IWriteRepository<Announcement> { }
    public interface IMemberReadRepository : IReadRepository<Member> { }
    public interface IMemberWriteRepository : IWriteRepository<Member> { };
}
