using AutoMapper;
using DigitalAgency.Bll.Models;
using DigitalAgency.Dal.Entities;

namespace DigitalAgency.Bll.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Client,ClientModel>().ReverseMap();
            CreateMap<Executor,ExecutorModel>().ReverseMap();
            CreateMap<Order,OrderModel>().ReverseMap();
            CreateMap<Project,ProjectModel>().ReverseMap();
            CreateMap<Task,TaskModel>().ReverseMap();
        }
    }
}
