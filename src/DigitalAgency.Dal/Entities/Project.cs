using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalAgency.Dal.Entities
{
    [Table("Projects")]
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string ProjectLink { get; set; }
        public int OwnerId { get; set; }
        
        [ForeignKey("OwnerId")]
        public virtual Client Client { get; set; }
        public virtual ICollection<Order> ServiceOrders { get; set; }
        
    }
}
