using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class LMSContext : DbContext
    {
        public LMSContext()
        {
        }

        public LMSContext(DbContextOptions<LMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrator> Administrators { get; set; } = null!;
        public virtual DbSet<Assignment> Assignments { get; set; } = null!;
        public virtual DbSet<AssignmentCategory> AssignmentCategories { get; set; } = null!;
        public virtual DbSet<Class> Classes { get; set; } = null!;
        public virtual DbSet<Course> Courses { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Enrolled> Enrolleds { get; set; } = null!;
        public virtual DbSet<Professor> Professors { get; set; } = null!;
        public virtual DbSet<Sshkey> Sshkeys { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<Submission> Submissions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("name=LMS:LMSConnectionString", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.8-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("latin1_swedish_ci")
                .HasCharSet("latin1");

            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.HasKey(e => e.AId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.UId, "uID")
                    .IsUnique();

                entity.Property(e => e.AId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("aID");

                entity.Property(e => e.FName)
                    .HasMaxLength(100)
                    .HasColumnName("fName");

                entity.Property(e => e.LName)
                    .HasMaxLength(100)
                    .HasColumnName("lName");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasIndex(e => new { e.AName, e.CategoryId }, "aName_categoryID")
                    .IsUnique();

                entity.HasIndex(e => e.CategoryId, "categoryID");

                entity.Property(e => e.AssignmentId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("assignmentID");

                entity.Property(e => e.AContents)
                    .HasMaxLength(8192)
                    .HasColumnName("aContents");

                entity.Property(e => e.AName)
                    .HasMaxLength(100)
                    .HasColumnName("aName");

                entity.Property(e => e.CategoryId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("categoryID");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.Points)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("points");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Assignments_ibfk_1");
            });

            modelBuilder.Entity<AssignmentCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => new { e.CatName, e.ClassId }, "catName_classID")
                    .IsUnique();

                entity.HasIndex(e => e.ClassId, "classID");

                entity.Property(e => e.CategoryId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("categoryID");

                entity.Property(e => e.CatName)
                    .HasMaxLength(100)
                    .HasColumnName("catName");

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("classID");

                entity.Property(e => e.Weight)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("weight");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.AssignmentCategories)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AssignmentCategories_ibfk_1");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasIndex(e => new { e.Semester, e.Year, e.CId }, "Semester")
                    .IsUnique();

                entity.HasIndex(e => e.CId, "cID");

                entity.HasIndex(e => e.PId, "pID");

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("classID");

                entity.Property(e => e.CId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("cID");

                entity.Property(e => e.EndTime)
                    .HasColumnType("time")
                    .HasColumnName("endTime");

                entity.Property(e => e.Location).HasMaxLength(100);

                entity.Property(e => e.PId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("pID");

                entity.Property(e => e.Semester).HasMaxLength(6);

                entity.Property(e => e.StartTime)
                    .HasColumnType("time")
                    .HasColumnName("startTime");

                entity.Property(e => e.Year).HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.CIdNavigation)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.CId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Classes_ibfk_1");

                entity.HasOne(d => d.PIdNavigation)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.PId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Classes_ibfk_2");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.DId, "Courses_ibfk_1");

                entity.HasIndex(e => new { e.CNum, e.DId }, "cNum")
                    .IsUnique();

                entity.Property(e => e.CId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("cID");

                entity.Property(e => e.CName)
                    .HasMaxLength(100)
                    .HasColumnName("cName");

                entity.Property(e => e.CNum)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("cNum");

                entity.Property(e => e.DId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("dID");

                entity.HasOne(d => d.DIdNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.DId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Courses_ibfk_1");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.DId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Subject, "Subject")
                    .IsUnique();

                entity.Property(e => e.DId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("dID");

                entity.Property(e => e.DName)
                    .HasMaxLength(100)
                    .HasColumnName("dName");

                entity.Property(e => e.Subject).HasMaxLength(4);
            });

            modelBuilder.Entity<Enrolled>(entity =>
            {
                entity.HasKey(e => new { e.SId, e.ClassId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("Enrolled");

                entity.HasIndex(e => e.ClassId, "classID");

                entity.Property(e => e.SId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("sID");

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("classID");

                entity.Property(e => e.Grade)
                    .HasMaxLength(2)
                    .HasColumnName("grade");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Enrolleds)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enrolled_ibfk_1");

                entity.HasOne(d => d.SIdNavigation)
                    .WithMany(p => p.Enrolleds)
                    .HasForeignKey(d => d.SId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enrolled_ibfk_2");
            });

            modelBuilder.Entity<Professor>(entity =>
            {
                entity.HasKey(e => e.PId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.WorksIn, "Professors_ibfk_1");

                entity.HasIndex(e => e.UId, "uID")
                    .IsUnique();

                entity.Property(e => e.PId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("pID");

                entity.Property(e => e.FName)
                    .HasMaxLength(100)
                    .HasColumnName("fName");

                entity.Property(e => e.LName)
                    .HasMaxLength(100)
                    .HasColumnName("lName");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.WorksIn).HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.WorksInNavigation)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.WorksIn)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Professors_ibfk_1");
            });

            modelBuilder.Entity<Sshkey>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("sshkey");

                entity.Property(e => e.Sshkey1)
                    .HasColumnType("text")
                    .HasColumnName("sshkey");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.SId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Major, "Students_ibfk_1");

                entity.HasIndex(e => e.UId, "uID")
                    .IsUnique();

                entity.Property(e => e.SId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("sID");

                entity.Property(e => e.FName)
                    .HasMaxLength(100)
                    .HasColumnName("fName");

                entity.Property(e => e.LName)
                    .HasMaxLength(100)
                    .HasColumnName("lName");

                entity.Property(e => e.Major).HasColumnType("int(10) unsigned");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.HasOne(d => d.MajorNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.Major)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Students_ibfk_1");
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasKey(e => new { e.AssignmentId, e.SId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.HasIndex(e => e.SId, "sID");

                entity.Property(e => e.AssignmentId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("assignmentID");

                entity.Property(e => e.SId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("sID");

                entity.Property(e => e.SContent)
                    .HasMaxLength(8192)
                    .HasColumnName("sContent");

                entity.Property(e => e.SDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("sDateTime");

                entity.Property(e => e.Score).HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Assignment)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.AssignmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submissions_ibfk_1");

                entity.HasOne(d => d.SIdNavigation)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.SId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submissions_ibfk_2");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
