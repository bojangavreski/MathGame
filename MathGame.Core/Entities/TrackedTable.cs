using System.ComponentModel.DataAnnotations;

namespace MathGame.Core.Entities;
public class TrackedTable 
{
    [Key]
    public int Id { get; set; }

    public Guid Uid { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? DeletedOn { get; set; }
}
