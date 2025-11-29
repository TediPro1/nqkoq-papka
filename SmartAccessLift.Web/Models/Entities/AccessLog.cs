using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAccessLift.Web.Models.Entities;

public class AccessLog
{
    [Key]
    public long Id { get; set; }

    public int? UserId { get; set; }

    public int? VisitorAccessId { get; set; }

    [Required]
    public int FloorId { get; set; }

    [Required]
    [MaxLength(20)]
    public string AccessMethod { get; set; } = string.Empty; // "NFC", "Fingerprint", "QR", "AdminOverride"

    [Required]
    [MaxLength(20)]
    public string Outcome { get; set; } = string.Empty; // "Successful", "Denied"

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Reason { get; set; }

    [MaxLength(45)]
    public string? IPAddress { get; set; }

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    [ForeignKey("VisitorAccessId")]
    public virtual VisitorAccess? VisitorAccess { get; set; }

    [ForeignKey("FloorId")]
    public virtual Floor Floor { get; set; } = null!;
}

