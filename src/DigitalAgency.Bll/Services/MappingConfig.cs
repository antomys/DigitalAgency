using AutoMapper;
using DigitalAgency.Bll.Models;
using DigitalAgency.Dal.Entities;

namespace DigitalAgency.Bll.Services
{
    public static class MappingConfig
    {
        public static MapperConfiguration GetMapper()
        {
            return new MapperConfiguration(expression =>
            {
                expression.CreateMap<Order, ServiceOrderModel>();
                expression.CreateMap<Project, CarModel>();
                expression.CreateMap<Client, ClientModel>();
            });
        }
    }
}