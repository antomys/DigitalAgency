using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalAgency.Dal.Entities
{
    [Table("ServiceOrders")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int  ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        public int  ExecutorId { get; set; }
        [ForeignKey("ExecutorId")]
        public virtual Executor Executor { get; set; }
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string State { get; set; }
        public virtual ICollection<OrderTask> OrderParts { get; set; }
        
    }
}
