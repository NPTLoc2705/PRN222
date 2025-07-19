using System;
using System.ComponentModel.DataAnnotations;

namespace mvc.dataaccess.Entities.Surveys
{
    public class QuestionOption
    {
        [Key]
        public Guid OptionId { get; set; }
        
        public Guid QuestionId { get; set; }
        
        [Required]
        [MaxLength(300)]
        public string OptionText { get; set; }
        
        public int Score { get; set; }
        
        public int OrderIndex { get; set; }

        // Navigation properties
        public virtual SurveyQuestion Question { get; set; }
    }
}