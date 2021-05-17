using System.Threading.Tasks;

namespace DigitalAgency.Bll.Services.Interfaces
{
    public interface IOrderService
    {
        Task<bool> ChangeOrderState(int id, string state);
    }
}