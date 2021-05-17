using DigitalAgency.Dal.Entities;
using Telegram.Bot.Types;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.Services.Bot.Interfaces
{
    public interface IProcessReply
    {
        Task ProcessClientReplies(Message message, Client thisClient, Update update);
    }
}