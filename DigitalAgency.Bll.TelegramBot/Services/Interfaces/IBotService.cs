using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace DigitalAgency.Bll.TelegramBot.Services.Interfaces
{
    public interface IBotService
    {
        public Task ProcessMessageAsync(Update receivedMessage);
    }
}
