using System;
using System.Collections.Generic;
using HIV.Models;
using Microsoft.EntityFrameworkCore;

namespace HIV.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Arv> Arvs { get; set; }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<CustomizedArvProtocol> CustomizedArvProtocols { get; set; }

    public virtual DbSet<CustomizedArvProtocolDetail> CustomizedArvProtocolDetails { get; set; }

    public virtual DbSet<DoctorInfo> DoctorInfos { get; set; }

    public virtual DbSet<EducationalResource> EducationalResources { get; set; }

    public virtual DbSet<Examination> Examinations { get; set; }

    public virtual DbSet<FacilityInfo> FacilityInfos { get; set; }

    public virtual DbSet<Prescription> Prescriptions { get; set; }

    public virtual DbSet<Reminder> Reminders { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<User> UserTables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=HIV_System;Trusted_Connection=true;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__46A222CD82E29DFF");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Email, "UQ__Account__AB6E6164353CD3C1").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Account__F3DBC572CF8AE374").IsUnique();

            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__A50828FC82CDE625");

            entity.ToTable("Appointment");

            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.IsAnonymous)
                .HasDefaultValue(false)
                .HasColumnName("is_anonymous");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");

            entity.HasOne(d => d.Patient)
                   .WithMany(p => p.PatientAppointments)
                   .HasForeignKey(d => d.PatientId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .HasConstraintName("FK__Appointme__patie__2F10007B");

            entity.HasOne(d => d.Doctor)
                .WithMany(p => p.DoctorAppointments)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_appointment_doctor");

            entity.HasOne(d => d.Schedule).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ScheduleId)
                .HasConstraintName("FK__Appointme__sched__300424B4");
            entity.Property(e => e.Status)
           .HasMaxLength(50)
           .IsUnicode(true);
            entity.Property(e => e.AppointmentDate)
            .HasColumnName("appointment_date")
           .HasColumnType("datetime")
           .HasDefaultValueSql("GETDATE()"); // Tự động set thời gian hiện tại
            entity.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();
        });

        modelBuilder.Entity<Arv>(entity =>
        {
            entity.HasKey(e => e.ArvId).HasName("PK__ARV__4A3011CB33E11ADC");

            entity.ToTable("ARV");

            entity.Property(e => e.ArvId).HasColumnName("arv_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__Blog__2975AA28A9AF1AD0");

            entity.ToTable("Blog");

            entity.Property(e => e.BlogId).HasColumnName("blog_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Author).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__Blog__author_id__1CF15040");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comment__E7957687F7C7DD95");

            entity.ToTable("Comment");

            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.BlogId).HasColumnName("blog_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Blog).WithMany(p => p.Comments)
                .HasForeignKey(d => d.BlogId)
                .HasConstraintName("FK__Comment__blog_id__20C1E124");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Comment__user_id__21B6055D");
        });

        modelBuilder.Entity<CustomizedArvProtocol>(entity =>
        {
            entity.HasKey(e => e.CustomProtocolId).HasName("PK__Customiz__0D356AC88E190C35");

            entity.ToTable("CustomizedARV_Protocol");

            entity.Property(e => e.CustomProtocolId).HasColumnName("custom_protocol_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");

            entity.HasOne(d => d.Doctor).WithMany(p => p.CustomizedArvProtocolDoctors)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__Customize__docto__3B75D760");

            entity.HasOne(d => d.Patient).WithMany(p => p.CustomizedArvProtocolPatients)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__Customize__patie__3C69FB99");
        });

        modelBuilder.Entity<CustomizedArvProtocolDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customiz__3213E83FF6DAC44A");

            entity.ToTable("CustomizedARV_Protocol_Detail");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ArvId).HasColumnName("arv_id");
            entity.Property(e => e.CustomProtocolId).HasColumnName("custom_protocol_id");
            entity.Property(e => e.Dosage)
                .HasMaxLength(100)
                .HasColumnName("dosage");
            entity.Property(e => e.UsageInstruction).HasColumnName("usage_instruction");

            entity.HasOne(d => d.Arv).WithMany(p => p.CustomizedArvProtocolDetails)
                .HasForeignKey(d => d.ArvId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Customize__arv_i__49C3F6B7");

            entity.HasOne(d => d.CustomProtocol).WithMany(p => p.CustomizedArvProtocolDetails)
                .HasForeignKey(d => d.CustomProtocolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Customize__custo__48CFD27E");
        });

        modelBuilder.Entity<DoctorInfo>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__DoctorIn__F3993564AA01A7F3");

            entity.ToTable("DoctorInfo");

            entity.Property(e => e.DoctorId)
                .ValueGeneratedNever()
                .HasColumnName("doctor_id");
            entity.Property(e => e.Degree)
                .HasMaxLength(100)
                .HasColumnName("degree");
            entity.Property(e => e.ExperienceYears).HasColumnName("experience_years");
            entity.Property(e => e.Specialization)
                .HasMaxLength(100)
                .HasColumnName("specialization");

            entity.HasOne(d => d.Doctor).WithOne(p => p.DoctorInfo)
                .HasForeignKey<DoctorInfo>(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DoctorInf__docto__286302EC");
        });

        modelBuilder.Entity<EducationalResource>(entity =>
        {
            entity.HasKey(e => e.ResourceId).HasName("PK__Educatio__4985FC7394D73E60");

            entity.Property(e => e.ResourceId).HasColumnName("resource_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.EducationalResources)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Education__creat__25869641");
        });

        modelBuilder.Entity<Examination>(entity =>
        {
            entity.HasKey(e => e.ExamId).HasName("PK__Examinat__9C8C7BE9F5579390");

            entity.ToTable("Examination");

            entity.Property(e => e.ExamId).HasColumnName("exam_id");
            entity.Property(e => e.Cd4Count).HasColumnName("cd4_count");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.ExamDate).HasColumnName("exam_date");
            entity.Property(e => e.HivLoad).HasColumnName("hiv_load");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.Result).HasColumnName("result");

            entity.HasOne(d => d.Doctor).WithMany(p => p.ExaminationDoctors)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__Examinati__docto__33D4B598");

            entity.HasOne(d => d.Patient).WithMany(p => p.ExaminationPatients)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__Examinati__patie__32E0915F");
        });

        modelBuilder.Entity<FacilityInfo>(entity =>
        {
            entity.HasKey(e => e.FacilityId).HasName("PK__Facility__B2E8EAAEA86B7394");

            entity.ToTable("FacilityInfo");

            entity.Property(e => e.FacilityId).HasColumnName("facility_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(e => e.PrescriptionId).HasName("PK__Prescrip__3EE444F8F7470B4E");

            entity.ToTable("Prescription");

            entity.HasIndex(e => e.ExaminationId, "UQ__Prescrip__BCD8253117FF2B27").IsUnique();

            entity.Property(e => e.PrescriptionId).HasColumnName("prescription_id");
            entity.Property(e => e.CustomProtocolId).HasColumnName("custom_protocol_id");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.ExamDate).HasColumnName("exam_date");
            entity.Property(e => e.ExamTime).HasColumnName("exam_time");
            entity.Property(e => e.ExaminationId).HasColumnName("examination_id");
            entity.Property(e => e.IssuedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("issued_at");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");

            entity.HasOne(d => d.CustomProtocol).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.CustomProtocolId)
                .HasConstraintName("FK__Prescript__custo__45F365D3");

            entity.HasOne(d => d.Doctor).WithMany(p => p.PrescriptionDoctors)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Prescript__docto__440B1D61");

            entity.HasOne(d => d.Examination).WithOne(p => p.Prescription)
                .HasForeignKey<Prescription>(d => d.ExaminationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Prescript__exami__44FF419A");

            entity.HasOne(d => d.Patient).WithMany(p => p.PrescriptionPatients)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Prescript__patie__4316F928");
        });

        modelBuilder.Entity<Reminder>(entity =>
        {
            entity.HasKey(e => e.ReminderId).HasName("PK__Reminder__E27A36289093D052");

            entity.ToTable("Reminder");

            entity.Property(e => e.ReminderId).HasColumnName("reminder_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.RemindAt)
                .HasColumnType("datetime")
                .HasColumnName("remind_at");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Reminders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Reminder__user_i__37A5467C");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Schedule__C46A8A6F7368054B");

            entity.ToTable("Schedule");

            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.Room)
                .HasMaxLength(50)
                .HasColumnName("room");
            entity.Property(e => e.ScheduledTime)
                .HasColumnType("datetime")
                .HasColumnName("scheduled_time");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__Schedule__doctor__2B3F6F97");

            entity.Property(e => e.Status)
            .HasMaxLength(50)
            .HasColumnName("Status")
            .HasDefaultValue("Available");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserTabl__B9BE370F9A1C2C44");

            entity.ToTable("UserTable");

            entity.HasIndex(e => e.AccountId, "UQ__UserTabl__46A222CC66B4C8A6").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Birthdate).HasColumnName("birthdate");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");

            entity.HasOne(d => d.Account).WithOne(p => p.UserTable)
                .HasForeignKey<User>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserTable__accou__173876EA");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
