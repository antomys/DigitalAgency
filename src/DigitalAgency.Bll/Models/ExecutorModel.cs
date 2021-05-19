using System.Collections.Generic;
using DigitalAgency.Dal.Entities.Enums;
using PositionsEnum = DigitalAgency.Bll.Models.Enums.PositionsEnum;

namespace DigitalAgency.Bll.Models
{
    public class ExecutorModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public PositionsEnum Position { get; set; }
        public long TelegramId { get; set; }
        public long ChatId { get; set; }
        public List<OrderModel> Orders { get; set; }
    }
}