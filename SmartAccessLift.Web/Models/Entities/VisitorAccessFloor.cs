using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAccessLift.Web.Models.Entities;

public class VisitorAccessFloor
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int VisitorAccessId { get; set; }

    [Required]
    public int FloorId { get; set; }

    // Navigation properties
    [ForeignKey("VisitorAccessId")]
    public virtual VisitorAccess VisitorAccess { get; set; } = null!;

    [ForeignKey("FloorId")]
    public virtual Floor Floor { get; set; } = null!;
}

