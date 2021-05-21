using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace DigitalAgency.Bll.TelegramBot.Services.Interfaces
{
    public interface IRegistrationService
    {
        Task<bool> StartRegistration(Update update);
    }
}