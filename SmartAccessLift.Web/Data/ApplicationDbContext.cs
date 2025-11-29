using Microsoft.EntityFrameworkCore;
using SmartAccessLift.Web.Models.Entities;

namespace SmartAccessLift.Web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Floor> Floors { get; set; }
    public DbSet<FloorPermission> FloorPermissions { get; set; }
    public DbSet<VisitorAccess> VisitorAccesses { get; set; }
    public DbSet<VisitorAccessFloor> VisitorAccessFloors { get; set; }
    public DbSet<AccessLog> AccessLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Role);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.HasCheckConstraint("CK_User_Role", "[Role] IN ('Resident', 'Admin')");
        });

        // Floor configuration
        modelBuilder.Entity<Floor>(entity =>
        {
            entity.HasIndex(e => e.FloorNumber).IsUnique();
        });

        // FloorPermission configuration
        modelBuilder.Entity<FloorPermission>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.FloorId }).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.FloorId);
            entity.HasIndex(e => e.IsAllowed);
            
            // Configure GrantedBy relationship (optional, can be null)
            entity.HasOne(e => e.GrantedByUser)
                .WithMany()
                .HasForeignKey(e => e.GrantedBy)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // VisitorAccess configuration
        modelBuilder.Entity<VisitorAccess>(entity =>
        {
            entity.HasIndex(e => e.QRCode).IsUnique();
            entity.HasIndex(e => e.CreatedByUserId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.StartTime, e.EndTime });
            entity.HasIndex(e => e.CreatedAt);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.HasCheckConstraint("CK_VisitorAccess_EndAfterStart", "[EndTime] > [StartTime]");
            entity.HasCheckConstraint("CK_VisitorAccess_Status", "[Status] IN ('Pending', 'Active', 'Expired')");
        });

        // VisitorAccessFloor configuration
        modelBuilder.Entity<VisitorAccessFloor>(entity =>
        {
            entity.HasIndex(e => new { e.VisitorAccessId, e.FloorId }).IsUnique();
            entity.HasIndex(e => e.VisitorAccessId);
            entity.HasIndex(e => e.FloorId);
        });

        // AccessLog configuration
        modelBuilder.Entity<AccessLog>(entity =>
        {
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.VisitorAccessId);
            entity.HasIndex(e => e.FloorId);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => new { e.FloorId, e.Timestamp });
            entity.HasIndex(e => new { e.UserId, e.Timestamp });
            entity.HasIndex(e => e.Outcome);
            entity.Property(e => e.AccessMethod).HasMaxLength(20);
            entity.Property(e => e.Outcome).HasMaxLength(20);
            entity.HasCheckConstraint("CK_AccessLog_UserOrVisitor", "([UserId] IS NOT NULL) OR ([VisitorAccessId] IS NOT NULL)");
            entity.HasCheckConstraint("CK_AccessLog_AccessMethod", "[AccessMethod] IN ('NFC', 'Fingerprint', 'QR', 'AdminOverride')");
            entity.HasCheckConstraint("CK_AccessLog_Outcome", "[Outcome] IN ('Successful', 'Denied')");
        });
    }
}

