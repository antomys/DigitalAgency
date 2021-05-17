using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalAgency.Dal.Entities
{
    [Table("Tasks")]
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int DaysDeadline { get; set; }
        public string State { get; set; }
        public virtual ICollection<OrderTask> OrderTasks { get; set; }
    }
}
