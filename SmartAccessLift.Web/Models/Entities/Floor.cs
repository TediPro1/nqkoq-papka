using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAccessLift.Web.Models.Entities;

public class Floor
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int FloorNumber { get; set; }

    [MaxLength(100)]
    public string? Name { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<FloorPermission> FloorPermissions { get; set; } = new List<FloorPermission>();
    public virtual ICollection<VisitorAccessFloor> VisitorAccessFloors { get; set; } = new List<VisitorAccessFloor>();
    public virtual ICollection<AccessLog> AccessLogs { get; set; } = new List<AccessLog>();
}

