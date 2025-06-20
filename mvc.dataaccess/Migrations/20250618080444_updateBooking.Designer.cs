﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using mvc.dataaccess.Entities;

#nullable disable

namespace mvc.dataaccess.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250618080444_updateBooking")]
    partial class updateBooking
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("BookingDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ConsultantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Bookings", (string)null);
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.Blog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<byte[]>("ImageData")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("blog_content")
                        .IsRequired()
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Blogs", (string)null);
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.Courses.Course", b =>
                {
                    b.Property<Guid>("CourseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<string>("DifficultyLevel")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<byte[]>("ImageBytes")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("ImageContentType")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("CourseId");

                    b.ToTable("Courses", (string)null);
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.Courses.CourseCategory", b =>
                {
                    b.Property<Guid>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CategoryId");

                    b.ToTable("CourseCategories", (string)null);
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.Courses.CourseCategoryMapping", b =>
                {
                    b.Property<Guid>("CourseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CourseId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("CourseCategoryMappings");
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.Courses.CoursePrerequisite", b =>
                {
                    b.Property<Guid>("CourseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PrerequisiteCourseId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CourseId", "PrerequisiteCourseId");

                    b.HasIndex("PrerequisiteCourseId");

                    b.ToTable("CoursePrerequisites");
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.Courses.Lesson", b =>
                {
                    b.Property<Guid>("LessonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContentUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<bool>("IsFreePreview")
                        .HasColumnType("bit");

                    b.Property<Guid>("ModuleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("OrderNumber")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LessonId");

                    b.HasIndex("ModuleId");

                    b.ToTable("Lessons", (string)null);
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.Courses.Module", b =>
                {
                    b.Property<Guid>("ModuleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrderNumber")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ModuleId");

                    b.HasIndex("CourseId");

                    b.ToTable("Modules", (string)null);
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.Courses.UserCourseProgress", b =>
                {
                    b.Property<Guid>("ProgressId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<DateTime?>("CompletedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastAccessed")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LessonId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("ProgressPercentage")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ProgressId");

                    b.HasIndex("CourseId");

                    b.HasIndex("LessonId");

                    b.HasIndex("UserId");

                    b.ToTable("UserCourseProgresses");
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsPublished")
                        .HasColumnType("bit");

                    b.Property<Guid>("StaffId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("StaffId");

                    b.ToTable("Posts", (string)null);
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.Courses.CourseCategoryMapping", b =>
                {
                    b.HasOne("mvc.dataaccess.Entities.Courses.CourseCategory", "Category")
                        .WithMany("CourseMappings")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("mvc.dataaccess.Entities.Courses.Course", "Course")
                        .WithMany("CategoryMappings")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Course");
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.Courses.CoursePrerequisite", b =>
                {
                    b.HasOne("mvc.dataaccess.Entities.Courses.Course", "Course")
                        .WithMany("Prerequisites")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("mvc.dataaccess.Entities.Courses.Course", "PrerequisiteCourse")
                        .WithMany("RequiredForCourses")
                        .HasForeignKey("PrerequisiteCourseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("PrerequisiteCourse");
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.Courses.Lesson", b =>
                {
                    b.HasOne("mvc.dataaccess.Entities.Courses.Module", "Module")
                        .WithMany("Lessons")
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Module");
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.Courses.Module", b =>
                {
                    b.HasOne("mvc.dataaccess.Entities.Courses.Course", "Course")
                        .WithMany("Modules")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.Courses.UserCourseProgress", b =>
                {
                    b.HasOne("mvc.dataaccess.Entities.Courses.Course", "Course")
                        .WithMany("UserProgresses")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("mvc.dataaccess.Entities.Courses.Lesson", "Lesson")
                        .WithMany("UserProgresses")
                        .HasForeignKey("LessonId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("mvc.dataaccess.Entities.User", "User")
                        .WithMany("CourseProgresses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Lesson");

                    b.Navigation("User");
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.Post", b =>
                {
                    b.HasOne("mvc.dataaccess.Entities.User", "Staff")
                        .WithMany()
                        .HasForeignKey("StaffId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Staff");
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.Courses.Course", b =>
                {
                    b.Navigation("CategoryMappings");

                    b.Navigation("Modules");

                    b.Navigation("Prerequisites");

                    b.Navigation("RequiredForCourses");

                    b.Navigation("UserProgresses");
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.Courses.CourseCategory", b =>
                {
                    b.Navigation("CourseMappings");
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.Courses.Lesson", b =>
                {
                    b.Navigation("UserProgresses");
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.Courses.Module", b =>
                {
                    b.Navigation("Lessons");
                });

            modelBuilder.Entity("mvc.dataaccess.Entities.User", b =>
                {
                    b.Navigation("CourseProgresses");
                });
#pragma warning restore 612, 618
        }
    }
}
