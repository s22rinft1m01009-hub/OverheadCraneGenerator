using AutoMapper;
using CraneGenerator.Application.DTOs;
using CraneGenerator.Domain.Entities;
using System.Xml;

namespace OverheadCraneGenerator.Shared
{
    public class DesignMappingProfile : Profile
    {
        protected DesignMappingProfile()
        {
            CreateMap<CraneDesign, DesignResponse>().ReverseMap();
            CreateMap<CreateDesignRequest, CraneDesign>().ReverseMap();
            CreateMap<UpdateDesignRequest, CraneDesign>().ReverseMap();
            CreateMap<CreateDesignRequest, UpdateDesignRequest>().ReverseMap();
        }

    }
}
