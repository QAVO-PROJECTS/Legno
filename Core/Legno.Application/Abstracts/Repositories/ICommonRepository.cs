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
    public interface IWorkPlanningWriteRepository : IWriteRepository<WorkPlanning> { }
}
