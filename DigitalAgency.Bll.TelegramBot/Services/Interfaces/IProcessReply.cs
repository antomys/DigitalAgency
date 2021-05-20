using DigitalAgency.Dal.Entities;
using Telegram.Bot.Types;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.TelegramBot.Services.Interfaces
{
    public interface IProcessReply
    {
        Task ProcessClientReplies(Message message, Client thisClient, Update update);
    }
}