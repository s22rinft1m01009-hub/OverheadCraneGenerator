using CraneGenerator.Application.DTOs;
using CraneGenerator.Domain.Entities;
using Mapster;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace OverheadCraneGenerator.Shared
{
    public class DesignMappingProfile : IRegister
    {
       

        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CraneDesign, DesignResponse>().TwoWays();
            config.NewConfig<CreateDesignRequest, CraneDesign>().TwoWays();
            config.NewConfig<UpdateDesignRequest, CraneDesign>().TwoWays();
            config.NewConfig<CreateDesignRequest, UpdateDesignRequest>().TwoWays();
        }
    }
}
