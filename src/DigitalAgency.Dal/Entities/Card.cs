using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalAgency.Dal.Entities.Enums;

namespace DigitalAgency.Dal.Entities;

[Table("Cards")]
public class Card
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; }
    public int DaysDeadline { get; set; }
    public OrderStateEnum StateEnum { get; set; }
    public int OrderId { get; set; }

    [ForeignKey("OrderId")] public virtual Order Order { get; set; }
}