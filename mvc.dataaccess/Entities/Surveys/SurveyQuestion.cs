using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mvc.dataaccess.Entities.Surveys
{
    public class SurveyQuestion
    {
        [Key]
        public Guid QuestionId { get; set; }
        
        public Guid SurveyId { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string QuestionText { get; set; }
        
        public int OrderIndex { get; set; }

        // Navigation properties
        public virtual Survey Survey { get; set; }
        public ICollection<QuestionOption> Options { get; set; }
    }
}