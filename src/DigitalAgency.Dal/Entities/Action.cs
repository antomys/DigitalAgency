using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalAgency.Dal.Entities
{
    [Table("Actions")]
    public class Action
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ActionName { get; set; }
        public string EntityType { get; set; } 
        public int EntityId { get; set; }
        public int ClientId { get; set; }
        
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        
        public DateTimeOffset ActionDate { get; set; }
        public bool IsDone { get; set; }
    }
}