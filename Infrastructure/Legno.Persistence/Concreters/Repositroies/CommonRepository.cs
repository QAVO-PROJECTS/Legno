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

    public class WorkPlanningReadRepository(LegnoDbContext c) : ReadRepository<WorkPlanning>(c), IWorkPlanningReadRepository { }
    public class WorkPlanningWriteRepository(LegnoDbContext c) : WriteRepository<WorkPlanning>(c), IWorkPlanningWriteRepository { }
}
