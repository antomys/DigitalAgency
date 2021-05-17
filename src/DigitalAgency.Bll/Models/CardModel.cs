using DigitalAgency.Dal.Entities.Enums;
using OrderStateEnum = DigitalAgency.Bll.Models.Enums.OrderStateEnum;

namespace DigitalAgency.Bll.Models
{
    public class CardModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DaysDeadline { get; set; }
        public OrderStateEnum StateEnum { get; set; }
        public int OrderId { get; set; }
    }
}