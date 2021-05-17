using System.Threading.Tasks;

namespace DigitalAgency.Bll.Services
{
    public interface IEnumService
    {
        Task<string[]> GetStateEnum();
        Task<string[]> GetPositionEnum();
    }
}