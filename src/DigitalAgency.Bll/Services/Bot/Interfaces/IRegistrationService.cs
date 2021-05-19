
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace DigitalAgency.Bll.Services.Bot.Interfaces
{
    public interface IRegistrationService
    {
        Task StartRegistration(Update update);
    }
}