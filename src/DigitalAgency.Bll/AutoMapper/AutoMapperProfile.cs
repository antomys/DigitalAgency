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
            CreateMap<OrderModel, Order>()
                .ForMember(x => x.ClientId, y => y.MapFrom(z => z.Client.Id))
                .ForMember(x => x.ProjectId, y => y.MapFrom(z => z.Project.Id))
                .ForMember(x => x.Client, y => y.Ignore())
                .ForMember(x=> x.Executor,y=>y.Ignore())
                .ForMember(x => x.Project, y => y.Ignore());
            CreateMap<Order, OrderModel>()
                .ForMember(x => x.Client, y => y.MapFrom(z => z.Client))
                .ForMember(x => x.Project, y => y.MapFrom(z => z.Project))
                .ForMember(x => x.Executor, y => y.MapFrom(z => z.Executor));
            CreateMap<Project, ProjectModel>();
            CreateMap<ProjectModel, Project>()
                .ForMember(x => x.OwnerId, y => y.MapFrom(z => z.Client.Id))
                .ForMember(x => x.Client, y => y.Ignore());
            CreateMap<Card, CardModel>()
                .ForMember(x => x.OrderId, y => y.MapFrom(z => z.OrderId));
            CreateMap<CardModel, Card>()
                .ForMember(x => x.OrderId, y => y.MapFrom(z => z.OrderId))
                .ForMember(x => x.Order, y => y.Ignore());
        }
    }
}
