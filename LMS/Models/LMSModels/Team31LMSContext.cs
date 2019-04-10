using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class Team31LMSContext : DbContext
    {
        public Team31LMSContext()
        {
        }

        public Team31LMSContext(DbContextOptions<Team31LMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrators> Administrators { get; set; }
        public virtual DbSet<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual DbSet<Assignments> Assignments { get; set; }
        public virtual DbSet<Classes> Classes { get; set; }
        public virtual DbSet<Courses> Courses { get; set; }
        public virtual DbSet<Departments> Departments { get; set; }
        public virtual DbSet<Enrollment> Enrollment { get; set; }
        public virtual DbSet<Professors> Professors { get; set; }
        public virtual DbSet<Students> Students { get; set; }
        public virtual DbSet<Submissions> Submissions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("Server=atr.eng.utah.edu;User Id=u1204589;Password=changeme;Database=Team31LMS");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrators>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("varchar(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .HasColumnName("first_name")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LastName)
                    .HasColumnName("last_name")
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<AssignmentCategories>(entity =>
            {
                entity.ToTable("Assignment_Categories");

                entity.HasIndex(e => e.Class)
                    .HasName("fk_category_of");

                entity.HasIndex(e => new { e.Name, e.Class })
                    .HasName("unique_name_class")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Class).HasColumnName("class");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Weight).HasColumnName("weight");

                entity.HasOne(d => d.ClassNavigation)
                    .WithMany(p => p.AssignmentCategories)
                    .HasForeignKey(d => d.Class)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_category_of");
            });

            modelBuilder.Entity<Assignments>(entity =>
            {
                entity.HasIndex(e => e.Category)
                    .HasName("fk_category");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Category).HasColumnName("category");

                entity.Property(e => e.Contents)
                    .HasColumnName("contents")
                    .HasColumnType("varchar(8192)");

                entity.Property(e => e.Due)
                    .HasColumnName("due")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Points)
                    .HasColumnName("points")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.Category)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_category");
            });

            modelBuilder.Entity<Classes>(entity =>
            {
                entity.HasIndex(e => e.OfferingOf)
                    .HasName("fk_offereing_of");

                entity.HasIndex(e => e.Teacher)
                    .HasName("fk_teacher");

                entity.HasIndex(e => new { e.SemesterSeason, e.SemesterYear, e.OfferingOf })
                    .HasName("classes_constraint")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.End)
                    .HasColumnName("end")
                    .HasColumnType("datetime");

                entity.Property(e => e.Location)
                    .HasColumnName("location")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.OfferingOf).HasColumnName("offering_of");

                entity.Property(e => e.SemesterSeason)
                    .HasColumnName("semester_season")
                    .HasColumnType("varchar(6)");

                entity.Property(e => e.SemesterYear)
                    .HasColumnName("semester_year")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Start)
                    .HasColumnName("start")
                    .HasColumnType("datetime");

                entity.Property(e => e.Teacher)
                    .IsRequired()
                    .HasColumnName("teacher")
                    .HasColumnType("varchar(8)");

                entity.HasOne(d => d.OfferingOfNavigation)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.OfferingOf)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_offereing_of");

                entity.HasOne(d => d.TeacherNavigation)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.Teacher)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_teacher");
            });

            modelBuilder.Entity<Courses>(entity =>
            {
                entity.HasIndex(e => e.Subject)
                    .HasName("subject");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Crn)
                    .HasColumnName("CRN")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Subject)
                    .HasColumnName("subject")
                    .HasColumnType("varchar(4)");

                entity.HasOne(d => d.SubjectNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.Subject)
                    .HasConstraintName("Courses_ibfk_1");
            });

            modelBuilder.Entity<Departments>(entity =>
            {
                entity.HasKey(e => e.Subject)
                    .HasName("PRIMARY");

                entity.Property(e => e.Subject)
                    .HasColumnName("subject")
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasIndex(e => new { e.UId, e.ClassId })
                    .HasName("uID")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ClassId).HasColumnName("classID");

                entity.Property(e => e.Grade)
                    .HasColumnName("grade")
                    .HasColumnType("varchar(2)");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("varchar(8)");
            });

            modelBuilder.Entity<Professors>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Works)
                    .HasName("fk_works");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("varchar(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .HasColumnName("first_name")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LastName)
                    .HasColumnName("last_name")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Works)
                    .IsRequired()
                    .HasColumnName("works")
                    .HasColumnType("varchar(4)");

                entity.HasOne(d => d.WorksNavigation)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.Works)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_works");
            });

            modelBuilder.Entity<Students>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Major)
                    .HasName("fk_major");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("varchar(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .HasColumnName("first_name")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LastName)
                    .HasColumnName("last_name")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Major)
                    .IsRequired()
                    .HasColumnName("major")
                    .HasColumnType("varchar(4)");

                entity.HasOne(d => d.MajorNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.Major)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_major");
            });

            modelBuilder.Entity<Submissions>(entity =>
            {
                entity.HasIndex(e => e.AId)
                    .HasName("fk_aID");

                entity.HasIndex(e => new { e.UId, e.AId })
                    .HasName("unique_uID_aID")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AId).HasColumnName("aID");

                entity.Property(e => e.Contents)
                    .HasColumnName("contents")
                    .HasColumnType("varchar(8192)");

                entity.Property(e => e.Score)
                    .HasColumnName("score")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Time)
                    .HasColumnName("time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("varchar(8)");

                entity.HasOne(d => d.A)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.AId)
                    .HasConstraintName("fk_aID");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.UId)
                    .HasConstraintName("fk_uID");
            });
        }
    }
}
