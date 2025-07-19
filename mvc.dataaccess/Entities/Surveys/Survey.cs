using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mvc.dataaccess.Entities.Surveys
{
    public enum SurveyType
    {
        ASSIST,
        CRAFFT,
        DAST,
        Custom
    }

    public class Survey
    {
        [Key]
        public Guid SurveyId { get; set; } 
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        
        [MaxLength(1000)]
        public string Description { get; set; }
        
        public SurveyType Type { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<SurveyQuestion> Questions { get; set; }
        public ICollection<SurveyResponse> Responses { get; set; }
    }
}