using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAccessLift.Web.Models.Entities;

public class VisitorAccess
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CreatedByUserId { get; set; }

    [Required]
    [MaxLength(200)]
    public string VisitorName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string QRCode { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? QRCodeImageUrl { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // "Pending", "Active", "Expired"

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? FirstUsedAt { get; set; }

    public DateTime? LastUsedAt { get; set; }

    [Required]
    public int UseCount { get; set; } = 0;

    // Navigation properties
    [ForeignKey("CreatedByUserId")]
    public virtual User CreatedByUser { get; set; } = null!;

    public virtual ICollection<VisitorAccessFloor> VisitorAccessFloors { get; set; } = new List<VisitorAccessFloor>();
    public virtual ICollection<AccessLog> AccessLogs { get; set; } = new List<AccessLog>();
}

