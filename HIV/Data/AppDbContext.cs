using HIV.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    // DbSet declarations
    public DbSet<Account> Accounts { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<EducationalResource> EducationalResources { get; set; }
    public DbSet<DoctorInfo> DoctorInfos { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Examination> Examinations { get; set; }
    public DbSet<Arv> ARVs { get; set; }
    public DbSet<ARVProtocol> ARVProtocols { get; set; }
    public DbSet<ARVProtocolDetail> ARVProtocolDetails { get; set; }
    public DbSet<CustomizedArvProtocol> CustomizedARVProtocols { get; set; }
    public DbSet<CustomizedArvProtocolDetail> CustomizedARVProtocolDetails { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
    public DbSet<Notification> Notification { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Table names (if needed, match exact names)
        modelBuilder.Entity<Account>().ToTable("Account");
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<Blog>().ToTable("Blog");
        modelBuilder.Entity<Comment>().ToTable("Comment");
        modelBuilder.Entity<EducationalResource>().ToTable("EducationalResources");
        modelBuilder.Entity<DoctorInfo>().ToTable("DoctorInfo");
        modelBuilder.Entity<Schedule>().ToTable("Schedule");
        modelBuilder.Entity<Appointment>().ToTable("Appointment");
        modelBuilder.Entity<Examination>().ToTable("Examination");
        modelBuilder.Entity<Arv>().ToTable("ARV");
        modelBuilder.Entity<ARVProtocol>().ToTable("ARV_Protocol");
        modelBuilder.Entity<ARVProtocolDetail>().ToTable("ARV_Protocol_Detail");
        modelBuilder.Entity<CustomizedArvProtocol>().ToTable("CustomizedARV_Protocol");
        modelBuilder.Entity<CustomizedArvProtocolDetail>().ToTable("CustomizedARV_Protocol_Detail");
        modelBuilder.Entity<MedicalRecord>().ToTable("MedicalRecord");


        // Account - User (1:1)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Account)
            .WithOne(a => a.User)
            .HasForeignKey<User>(u => u.AccountId);

        // User - DoctorInfo (1:1)
        modelBuilder.Entity<DoctorInfo>()
            .HasOne(d => d.Doctor)
            .WithOne(u => u.DoctorInfo)
            .HasForeignKey<DoctorInfo>(d => d.DoctorId);

        // Blog - Comment (1:N)
        modelBuilder.Entity<Blog>()
            .HasMany(b => b.Comments)
            .WithOne(c => c.Blog)
            .HasForeignKey(c => c.BlogId);

        // User - Blog (1:N)
        modelBuilder.Entity<Blog>()
            .HasOne(b => b.Author)
            .WithMany(u => u.Blogs)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // User - Comment (1:N)
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // User - EducationalResources (1:N)
        modelBuilder.Entity<EducationalResource>()
            .HasOne(e => e.CreatedByNavigation)
            .WithMany(u => u.EducationalResources)
            .HasForeignKey(e => e.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // User - Schedule (1:N)
        modelBuilder.Entity<Schedule>()
            .HasOne(s => s.Doctor)
            .WithMany(u => u.Schedules)
            .HasForeignKey(s => s.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Appointment
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(u => u.AppointmentsAsPatient)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(u => u.AppointmentsAsDoctor)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Schedule)
            .WithMany(s => s.Appointments)
            .HasForeignKey(a => a.ScheduleId);

        // Examination
        modelBuilder.Entity<Examination>()
            .HasOne(e => e.Patient)
            .WithMany(u => u.ExaminationsAsPatient)
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Examination>()
            .HasOne(e => e.Doctor)
            .WithMany(u => u.ExaminationsAsDoctor)
            .HasForeignKey(e => e.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Examination - MedicalRecord (1:1)
        modelBuilder.Entity<Examination>()
            .HasOne(e => e.MedicalRecord)
            .WithOne(m => m.Examination)
            .HasForeignKey<MedicalRecord>(m => m.ExamId);

        // ARV - ARVProtocolDetail (1:N)
        modelBuilder.Entity<ARVProtocolDetail>()
            .HasOne(p => p.Arv)
            .WithMany(a => a.ProtocolDetails)
            .HasForeignKey(p => p.ArvId);

        modelBuilder.Entity<ARVProtocolDetail>()
            .HasOne(p => p.Protocol)
            .WithMany(p => p.Details)
            .HasForeignKey(p => p.ProtocolId);

        // Customized ARV Protocol
        modelBuilder.Entity<CustomizedArvProtocol>()
            .HasOne(c => c.Doctor)
            .WithMany(u => u.CustomProtocolsAsDoctor)
            .HasForeignKey(c => c.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CustomizedArvProtocol>()
            .HasOne(c => c.Patient)
            .WithMany(u => u.CustomProtocolsAsPatient)
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CustomizedArvProtocol>()
            .HasOne(c => c.BaseProtocol)
            .WithMany(p => p.CustomizedProtocols)
            .HasForeignKey(c => c.BaseProtocolId);

        modelBuilder.Entity<CustomizedArvProtocolDetail>()
            .HasOne(c => c.CustomProtocol)
            .WithMany(p => p.Details)
            .HasForeignKey(c => c.CustomProtocolId);

        modelBuilder.Entity<CustomizedArvProtocolDetail>()
            .HasOne(c => c.Arv)
            .WithMany(a => a.CustomProtocolDetails)
            .HasForeignKey(c => c.ArvId);

        // MedicalRecord
        modelBuilder.Entity<MedicalRecord>()
            .HasOne(m => m.Patient)
            .WithMany(u => u.MedicalRecordsAsPatient)
            .HasForeignKey(m => m.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MedicalRecord>()
            .HasOne(m => m.Doctor)
            .WithMany(u => u.MedicalRecordsAsDoctor)
            .HasForeignKey(m => m.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MedicalRecord>()
            .HasOne(m => m.CustomProtocol)
            .WithMany(c => c.MedicalRecords)
            .HasForeignKey(m => m.CustomProtocolId);
    }
}
