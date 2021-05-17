using AutoMapper;
using DigitalAgency.Api.Models;
using DigitalAgency.Bll.DTOs;

namespace DigitalAgency.Api.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ScheaduleViewModel,OrderDTO>();
        }
    }
}
