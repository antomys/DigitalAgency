using System;
using DigitalAgency.Dal.Entities.Enums;

namespace DigitalAgency.Bll.TelegramBot.Models;

public class BotShortOrderModel
{
    public int Id { get; set; }
    public string ClientName { get; set; }
    public string ClientPhone { get; set; }
    public string ProjectName { get; set; }
    public string ExecutorName { get; set; }
    public string ExecutorPhone { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public DateTimeOffset ScheduledTime { get; set; }
    public OrderStateEnum StateEnum { get; set; }
    public int TasksCount { get; set; }
}