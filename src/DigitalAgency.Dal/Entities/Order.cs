using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalAgency.Dal.Entities.Enums;

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
        public int? ExecutorId { get; set; }
        [ForeignKey("ExecutorId")]
        public virtual Executor Executor { get; set; }
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public DateTimeOffset ScheduledTime { get; set; }
        public OrderStateEnum StateEnum { get; set; }
        
        public virtual ICollection<Card> Tasks { get; set; }
        
    }
}
