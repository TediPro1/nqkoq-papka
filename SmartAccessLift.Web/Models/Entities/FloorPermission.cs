using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAccessLift.Web.Models.Entities;

public class FloorPermission
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int FloorId { get; set; }

    [Required]
    public bool IsAllowed { get; set; } = true;

    public int? GrantedBy { get; set; }

    [Required]
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("FloorId")]
    public virtual Floor Floor { get; set; } = null!;

    [ForeignKey("GrantedBy")]
    public virtual User? GrantedByUser { get; set; }
}

