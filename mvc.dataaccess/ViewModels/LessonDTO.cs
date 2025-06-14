using Microsoft.AspNetCore.Http;
using mvc.dataaccess.Entities.Courses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mvc.dataaccess.ViewModels
{
    public class LessonDTO
    {
        // Common properties
        public Guid LessonId { get; set; }

        [Required(ErrorMessage = "Module ID is required")]
        public Guid ModuleId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content type is required")]
        [Display(Name = "Content Type")]
        public string ContentType { get; set; } // Video, Article, Quiz, etc.

        [Display(Name = "Content URL")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string? ContentUrl { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute")]
        public int Duration { get; set; } // in minutes

        [Display(Name = "Order Number")]
        public int OrderNumber { get; set; }

        [Display(Name = "Free Preview")]
        public bool IsFreePreview { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        // For file upload during creation
        public IFormFile? ContentFile { get; set; }

        // For displaying additional information
        public string? ModuleTitle { get; set; }
        public string? ModuleDescription { get; set; }
        public string? CourseTitle { get; set; }
        public Guid CourseId { get; set; }

        // For error handling
        public bool Error { get; set; }
        public string? Message { get; set; }

        // For reordering
        public List<Guid>? OrderedLessonIds { get; set; }

        // Constructor for easy conversion
        public LessonDTO() { }

        public LessonDTO(Lesson lesson)
        {
            if (lesson != null)
            {
                LessonId = lesson.LessonId;
                ModuleId = lesson.ModuleId;
                Title = lesson.Title;
                ContentType = lesson.ContentType;
                ContentUrl = lesson.ContentUrl;
                Duration = lesson.Duration;
                OrderNumber = lesson.OrderNumber;
                IsFreePreview = lesson.IsFreePreview;
                CreatedAt = lesson.CreatedAt;
            }
        }
    }
}