using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAccessLift.Web.Models.Entities;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(256)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(512)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = "Resident"; // "Resident" or "Admin"

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }

    [Required]
    public bool EmailConfirmed { get; set; } = false;

    // Navigation properties
    public virtual ICollection<FloorPermission> FloorPermissions { get; set; } = new List<FloorPermission>();
    public virtual ICollection<VisitorAccess> CreatedVisitorAccesses { get; set; } = new List<VisitorAccess>();
    public virtual ICollection<AccessLog> AccessLogs { get; set; } = new List<AccessLog>();
}

