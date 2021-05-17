using System.Collections.Generic;
using DigitalAgency.Dal.Entities;
using Task = System.Threading.Tasks.Task;

namespace DigitalAgency.Bll.Services.Bot.Interfaces
{
    public interface IButtons
    {
        Task ViewOrderButtons(IEnumerable<Order> orders, long chatId);
        Task ViewProjectButtons(IEnumerable<Project> projects, long chatId);
        Task AddOrderViewProjectButtons(IEnumerable<Project> projects, long chatId, string key = "project");
    }
}