using System.Collections.Generic;
using DigitalAgency.Dal.Entities;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.TelegramBot.Services.Interfaces
{
    public interface IButtons
    {
        Task ViewOrderButtons(IEnumerable<Order> orders, long chatId);
        Task ViewProjectButtons(IEnumerable<Project> projects, long chatId);
        Task AddOrderViewProjectButtons(IEnumerable<Project> projects, long chatId, string key = "project");
    }
}