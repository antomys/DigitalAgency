using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalAgency.Dal.Entities;

[Table("Clients")]
public class Client
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public long TelegramId { get; set; }
    public long ChatId { get; set; }

    public virtual ICollection<Order> Orders { get; set; }

    public virtual ICollection<Project> Projects { get; set; }

    public virtual ICollection<Action> Actions { get; set; }
}