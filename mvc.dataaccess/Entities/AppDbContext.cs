using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using mvc.dataaccess.Entities.Courses;
using System;
using System.IO;

namespace mvc.dataaccess.Entities
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<CourseCategory> CourseCategories { get; set; }
        public DbSet<CourseCategoryMapping> CourseCategoryMappings { get; set; }
        public DbSet<UserCourseProgress> UserCourseProgresses { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);

                entity.Property(u => u.FullName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(u => u.Password)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(u => u.PhoneNumber)
                    .HasMaxLength(20);

                entity.Property(u => u.Address)
                    .HasMaxLength(500);

                entity.Property(u => u.Role)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(u => u.IsActive)
                    .HasDefaultValue(true);

                entity.Property(u => u.Id)
                    .HasDefaultValueSql("NEWID()");
            });

            // Configure Course entity
            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Courses");
                entity.HasKey(e => e.CourseId);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.DifficultyLevel)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Duration)
                    .IsRequired();

                entity.Property(e => e.ImageBytes)
                    .IsRequired(false);

                entity.Property(e => e.ImageContentType)
                    .IsRequired(false)
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                entity.Property(e => e.UpdatedAt)
                    .IsRequired();

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(e => e.CourseId)
                    .HasDefaultValueSql("NEWID()");

                entity.HasQueryFilter(c => c.IsActive);
            });

            // Configure Lesson entity
            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.ToTable("Lessons");
                entity.HasKey(l => l.LessonId);

                entity.Property(l => l.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(l => l.ContentType)
                    .HasMaxLength(100);

                entity.Property(l => l.ContentUrl)
                    .HasMaxLength(500);

                entity.Property(l => l.Duration)
                    .IsRequired();

                entity.Property(l => l.OrderNumber)
                    .IsRequired();

                entity.Property(l => l.IsFreePreview)
                    .HasDefaultValue(false);

                entity.Property(l => l.CreatedAt)
                    .IsRequired();

                entity.Property(l => l.LessonId)
                    .HasDefaultValueSql("NEWID()");

                entity.HasOne(l => l.Course)
                    .WithMany(c => c.Lessons)
                    .HasForeignKey(l => l.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure CourseCategory entity
            modelBuilder.Entity<CourseCategory>(entity =>
            {
                entity.ToTable("CourseCategories");
                entity.HasKey(cc => cc.CategoryId);

                entity.Property(cc => cc.Name)
                    .IsRequired()
                    .HasMaxLength(100);

               

                entity.Property(cc => cc.CategoryId)
                    .HasDefaultValueSql("NEWID()");
            });

            // Configure CourseCategoryMapping entity
            modelBuilder.Entity<CourseCategoryMapping>(entity =>
            {
                entity.ToTable("CourseCategoryMappings");
                entity.HasKey(cc => new { cc.CourseId, cc.CategoryId });

                entity.HasOne(cc => cc.Course)
                    .WithMany(c => c.CategoryMappings)
                    .HasForeignKey(cc => cc.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cc => cc.Category)
                    .WithMany(c => c.CourseMappings)
                    .HasForeignKey(cc => cc.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure UserCourseProgress entity
            modelBuilder.Entity<UserCourseProgress>(entity =>
            {
                entity.ToTable("UserCourseProgresses");
                entity.HasKey(ucp => ucp.ProgressId);

                entity.Property(ucp => ucp.IsCompleted)
                    .HasDefaultValue(false);

                entity.Property(ucp => ucp.ProgressPercentage)
                    .HasColumnType("decimal(5,2)")
                    .HasDefaultValue(0.00m);

                entity.Property(ucp => ucp.LastAccessed)
                    .IsRequired();

                entity.Property(ucp => ucp.CompletedAt)
                    .IsRequired(false);

                entity.Property(ucp => ucp.ProgressId)
                    .HasDefaultValueSql("NEWID()");

                entity.HasOne(ucp => ucp.User)
                    .WithMany(u => u.CourseProgresses)
                    .HasForeignKey(ucp => ucp.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ucp => ucp.Course)
                    .WithMany(c => c.UserProgresses)
                    .HasForeignKey(ucp => ucp.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ucp => ucp.Lesson)
                    .WithMany(l => l.UserProgresses)
                    .HasForeignKey(ucp => ucp.LessonId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(u => new { u.UserId, u.CourseId });
                entity.HasIndex(u => u.LessonId);
            });

            // Configure other entities (Blog, Post, Booking) similarly...

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnectionDB");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("The connection string 'DefaultConnectionDB' was not found in appsettings.json");
                }

                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}