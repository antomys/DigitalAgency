using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace DigitalAgency.Bll.Services.Interfaces
{
    public interface IBotService
    {
        public Task ProcessMessageAsync(Update receivedMessage);
    }
}
