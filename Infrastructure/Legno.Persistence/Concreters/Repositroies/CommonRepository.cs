// Concrete
using Legno.Application.Abstracts.Repositories;
using Legno.Domain.Entities;
using Legno.Persistence.Concreters.Repositories;
using Legno.Persistence.Context;
using System;

namespace Legno.Persistence.Repositories
{
    public class B2BServiceReadRepository(LegnoDbContext c) : ReadRepository<B2BService>(c), IB2BServiceReadRepository { }
    public class B2BServiceWriteRepository(LegnoDbContext c) : WriteRepository<B2BService>(c), IB2BServiceWriteRepository { }
    public class BusinessServiceReadRepository(LegnoDbContext c) : ReadRepository<BusinessService>(c), IBusinessServiceReadRepository { }
    public class BusinessServiceWriteRepository(LegnoDbContext c) : WriteRepository<BusinessService>(c), IBusinessServiceWriteRepository { }
    public class CommonServiceReadRepository(LegnoDbContext c) : ReadRepository<CommonService>(c), ICommonServiceReadRepository { }
    public class CommonServiceWriteRepository(LegnoDbContext c) : WriteRepository<CommonService>(c), ICommonServiceWriteRepository { }
    public class DesignerCommonServiceReadRepository(LegnoDbContext c) : ReadRepository<DesignerCommonService>(c), IDesignerCommonServiceReadRepository { }
    public class DesignerCommonServiceWriteRepository(LegnoDbContext c) : WriteRepository<DesignerCommonService>(c), IDesignerCommonServiceWriteRepository { }
    public class DesignerServiceReadRepository(LegnoDbContext c) : ReadRepository<DesignerService>(c), IDesignerServiceReadRepository { }
    public class DesignerServiceWriteRepository(LegnoDbContext c) : WriteRepository<DesignerService>(c), IDesignerServiceWriteRepository { }
    public class FabricReadRepository(LegnoDbContext c) : ReadRepository<Fabric>(c), IFabricReadRepository { }
    public class FabricWriteRepository(LegnoDbContext c) : WriteRepository<Fabric>(c), IFabricWriteRepository { }
    public class LocationReadRepository(LegnoDbContext c) : ReadRepository<Location>(c), ILocationReadRepository { }
    public class LocationWriteRepository(LegnoDbContext c) : WriteRepository<Location>(c), ILocationWriteRepository { }
    public class PartnerReadRepository(LegnoDbContext c) : ReadRepository<Partner>(c), IPartnerReadRepository { }
    public class PartnerWriteRepository(LegnoDbContext c) : WriteRepository<Partner>(c), IPartnerWriteRepository { }
    public class ServiceSliderReadRepository(LegnoDbContext c) : ReadRepository<ServiceSlider>(c), IServiceSliderReadRepository { }
    public class ServiceSliderWriteRepository(LegnoDbContext c) : WriteRepository<ServiceSlider>(c), IServiceSliderWriteRepository { }
    public class ProjectSliderImageWriteRepository(LegnoDbContext c) : WriteRepository<ProjectSliderImage>(c), IProjectSliderImageWriteRepository { }
    public class ProjectSliderImageReadRepository(LegnoDbContext c) : ReadRepository<ProjectSliderImage>(c), IProjectSliderImageReadRepository { }
    public class ProjectSliderReadWriteRepository(LegnoDbContext c) : ReadRepository<ProjectFabric>(c), IProjectFabricReadRepository { }
    public class ProjectFabricWriteRepository(LegnoDbContext c) : WriteRepository<ProjectFabric>(c), IProjectFabricWriteRepository { }
    public class ProjectFabricReadRepository(LegnoDbContext c) : ReadRepository<ProjectFabric>(c), IProjectFabricReadRepository { }

    public class CategoryImageReadRepository(LegnoDbContext c) : ReadRepository<CategoryImage>(c), ICategoryImageReadRepository { }
    public class CategoryImageWriteRepository(LegnoDbContext c) : WriteRepository<CategoryImage>(c), ICategoryImageWriteRepository { }
    public class WorkPlanningReadRepository(LegnoDbContext c) : ReadRepository<WorkPlanning>(c), IWorkPlanningReadRepository { }
    public class WorkPlanningWriteRepository(LegnoDbContext c) : WriteRepository<WorkPlanning>(c), IWorkPlanningWriteRepository { }
    public class PurchaseReadRepository(LegnoDbContext c) : ReadRepository<Purchase>(c), IPurchaseReadRepository { }
    public class PurchaseWriteRepository(LegnoDbContext c) : WriteRepository<Purchase>(c), IPurchaseWriteRepository { }


    public class CareerReadRepository(LegnoDbContext c): ReadRepository<Career>(c), ICareerReadRepository { }

    public class CareerWriteRepository(LegnoDbContext c): WriteRepository<Career>(c), ICareerWriteRepository{ }


    public class ArticleReadRepository(LegnoDbContext c): ReadRepository<Article>(c), IArticleReadRepository{ }

    public class ArticleWriteRepository(LegnoDbContext c): WriteRepository<Article>(c), IArticleWriteRepository{ }


    public class ArticleImageReadRepository(LegnoDbContext c): ReadRepository<ArticleImage>(c), IArticleImageReadRepository{ }

    public class ArticleImageWriteRepository(LegnoDbContext c): WriteRepository<ArticleImage>(c), IArticleImageWriteRepository{ }


    public class AnnouncementReadRepository(LegnoDbContext c): ReadRepository<Announcement>(c), IAnnouncementReadRepository{ }

    public class AnnouncementWriteRepository(LegnoDbContext c): WriteRepository<Announcement>(c), IAnnouncementWriteRepository{ }
    public class MemberReadRepository(LegnoDbContext c): ReadRepository<Member>(c), IMemberReadRepository{ }
    public class MemberWriteRepository(LegnoDbContext c):WriteRepository<Member>(c),IMemberWriteRepository{};
    public class AboutReadRepository(LegnoDbContext c) : ReadRepository<About>(c),IAboutReadRepository { }
    public class AboutWriteRepository(LegnoDbContext c):WriteRepository<About>(c),IAboutWriteRepository{};
    public class DirectorWriteRepository(LegnoDbContext c):WriteRepository<Director>(c),IDirectorWriteRepository{};
    public class DirectorReadRepository(LegnoDbContext c):ReadRepository<Director>(c),IDirectorReadRepository{};
    public class SettingReadRepository(LegnoDbContext c):ReadRepository<Setting>(c),ISettingReadRepository{};
    public class SettingWriteRepository(LegnoDbContext c):WriteRepository<Setting>(c),ISettingWriteRepository{};
    public class ContactBranchWriteRepository(LegnoDbContext c):WriteRepository<ContactBranch>(c),IContactBranchWriteRepository{};
    public class ContactBranchReadRepository(LegnoDbContext c):ReadRepository<ContactBranch>(c),IContactBranchReadRepository{};




}
