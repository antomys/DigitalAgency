using System.Collections.Generic;

namespace DigitalAgency.Bll.Models;

public class ClientModel
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public long TelegramId { get; set; }
    public long ChatId { get; set; }

    public List<OrderModel> Orders { get; set; }
    public List<ProjectModel> Projects { get; set; }
}