using DocAI.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DocAI.API.Data;

public class DocAIDbContext : DbContext
{
    public DocAIDbContext(DbContextOptions<DocAIDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<PatientCase> PatientCases => Set<PatientCase>();
    public DbSet<AuditReport> AuditReports => Set<AuditReport>();
    public DbSet<ApprovalRecord> ApprovalRecords => Set<ApprovalRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.Email).HasMaxLength(256);
            e.Property(u => u.Username).HasMaxLength(100);
            e.Property(u => u.FullName).HasMaxLength(200);
            e.Property(u => u.Role).HasMaxLength(50);
        });

        modelBuilder.Entity<PatientCase>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasOne(p => p.Physician)
             .WithMany(u => u.PatientCases)
             .HasForeignKey(p => p.PhysicianId)
             .OnDelete(DeleteBehavior.Restrict);
            e.Property(p => p.ChiefComplaint).HasMaxLength(2000);
            e.Property(p => p.ProvisionalDiagnosis).HasMaxLength(2000);
            e.Property(p => p.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<AuditReport>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasOne(a => a.PatientCase)
             .WithOne(p => p.AuditReport)
             .HasForeignKey<AuditReport>(a => a.PatientCaseId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ApprovalRecord>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasOne(a => a.PatientCase)
             .WithMany(p => p.ApprovalRecords)
             .HasForeignKey(a => a.PatientCaseId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(a => a.Physician)
             .WithMany()
             .HasForeignKey(a => a.PhysicianId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
