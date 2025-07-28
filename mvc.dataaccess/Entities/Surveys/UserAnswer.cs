using System;
using System.ComponentModel.DataAnnotations;

namespace mvc.dataaccess.Entities.Surveys
{
    public class UserAnswer
    {
        [Key]
        public Guid AnswerId { get; set; }
        
        public Guid ResponseId { get; set; }
        
        public Guid QuestionId { get; set; }
        
        public Guid OptionId { get; set; }
        
        public int Score { get; set; }

        // Navigation properties
        public SurveyResponse Response { get; set; }
        public SurveyQuestion Question { get; set; }
        public QuestionOption Option { get; set; }
    }
}