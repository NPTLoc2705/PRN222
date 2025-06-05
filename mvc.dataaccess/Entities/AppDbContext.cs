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
        public DbSet<Module> Modules { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<CourseCategory> CourseCategories { get; set; }
        public DbSet<CourseCategoryMapping> CourseCategoryMappings { get; set; }
        public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }
        public DbSet<UserCourseProgress> UserCourseProgresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure table names
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Post>().ToTable("Posts");
            modelBuilder.Entity<Blog>().ToTable("Blogs");
            modelBuilder.Entity<Course>().ToTable("Courses");
            modelBuilder.Entity<Module>().ToTable("Modules");
            modelBuilder.Entity<Lesson>().ToTable("Lessons");
            modelBuilder.Entity<CourseCategory>().ToTable("CourseCategories");

            // Configure primary keys
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<Course>(entity =>
            {
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

                // Image properties are optional
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
            });
            modelBuilder.Entity<Module>().HasKey(m => m.ModuleId);
            modelBuilder.Entity<Lesson>().HasKey(l => l.LessonId);
            modelBuilder.Entity<CourseCategory>().HasKey(cc => cc.CategoryId);
            modelBuilder.Entity<UserCourseProgress>().HasKey(ucp => ucp.ProgressId);

            // Configure many-to-many relationships
            modelBuilder.Entity<CourseCategoryMapping>()
                .HasKey(cc => new { cc.CourseId, cc.CategoryId });

            modelBuilder.Entity<CoursePrerequisite>()
                .HasKey(cp => new { cp.CourseId, cp.PrerequisiteCourseId });

            // Configure relationships with proper delete behavior
            modelBuilder.Entity<Module>()
                .HasOne(m => m.Course)
                .WithMany(c => c.Modules)
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Module)
                .WithMany(m => m.Lessons)
                .HasForeignKey(l => l.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CoursePrerequisite>()
                .HasOne(cp => cp.Course)
                .WithMany(c => c.Prerequisites)
                .HasForeignKey(cp => cp.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CoursePrerequisite>()
                .HasOne(cp => cp.PrerequisiteCourse)
                .WithMany(c => c.RequiredForCourses)
                .HasForeignKey(cp => cp.PrerequisiteCourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure UserCourseProgress relationships
            modelBuilder.Entity<UserCourseProgress>()
                .HasOne(ucp => ucp.User)
                .WithMany(u => u.CourseProgresses)
                .HasForeignKey(ucp => ucp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserCourseProgress>()
                .HasOne(ucp => ucp.Course)
                .WithMany(c => c.UserProgresses)
                .HasForeignKey(ucp => ucp.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserCourseProgress>()
                .HasOne(ucp => ucp.Lesson)
                .WithMany(l => l.UserProgresses)
                .HasForeignKey(ucp => ucp.LessonId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure CourseCategoryMapping relationships
            modelBuilder.Entity<CourseCategoryMapping>()
                .HasOne(cc => cc.Course)
                .WithMany(c => c.CategoryMappings)
                .HasForeignKey(cc => cc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourseCategoryMapping>()
                .HasOne(cc => cc.Category)
                .WithMany(c => c.CourseMappings)
                .HasForeignKey(cc => cc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure GUID default values for SQL Server
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Course>()
                .Property(c => c.CourseId)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Module>()
                .Property(m => m.ModuleId)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Lesson>()
                .Property(l => l.LessonId)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<CourseCategory>()
                .Property(cc => cc.CategoryId)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<UserCourseProgress>()
                .Property(ucp => ucp.ProgressId)
                .HasDefaultValueSql("NEWID()");

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Only configure if not already configured (this helps with DI scenarios)
            if (!optionsBuilder.IsConfigured)
            {
                // Get the configuration from appsettings.json
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