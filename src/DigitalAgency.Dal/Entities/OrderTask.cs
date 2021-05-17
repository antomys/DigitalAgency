using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalAgency.Dal.Entities
{
    public class OrderTask
    {
        public int OrderId {get; set;}
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        public int TaskId { get; set; }
        [ForeignKey("TaskId")]
        public virtual Task Task { get; set; }
    }
}