using System;
using System.Threading.Tasks;
using DigitalAgency.Bll.Models.Enums;

namespace DigitalAgency.Bll.Services
{
    public class EnumService : IEnumService
    {
        public async Task<string[]> GetStateEnum()
        {
            return await Task.FromResult(Enum.GetNames(typeof(OrderStateEnum)));
        }

        public async Task<string[]> GetPositionEnum()
        {
            return await Task.FromResult(Enum.GetNames(typeof(OrderStateEnum)));
        }
    }
}