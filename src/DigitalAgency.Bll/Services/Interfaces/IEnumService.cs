using System.Threading.Tasks;

namespace DigitalAgency.Bll.Services.Interfaces;

public interface IEnumService
{
    Task<string[]> GetStateEnum();
    Task<string[]> GetPositionEnum();
}