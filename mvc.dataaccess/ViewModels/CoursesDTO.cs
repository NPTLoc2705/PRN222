using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.dataaccess.ViewModels
{
    public class CoursesDTO
    {
        public bool Error { get; set; }
        public string? Message { get; set; }

        // Course data
        public Guid CourseId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [Display(Name = "Course Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Course Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0")]
        [Display(Name = "Duration (minutes)")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Difficulty level is required")]
        [Display(Name = "Difficulty Level")]
        public string DifficultyLevel { get; set; }

        public byte[]? ImageBytes { get; set; }
        public string? ImageContentType { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
       
        public List<LessonDTO>? Lessons { get; set; }  // Changed from LessonIds to full LessonDTO objects
        [MinLength(1, ErrorMessage = "Please select at least one category")]

        public List<Guid> SelectedCategoryIds { get; set; } = new List<Guid>();
        public List<CategoryDTO> AvailableCategories { get; set; } = new List<CategoryDTO>();

    }
}