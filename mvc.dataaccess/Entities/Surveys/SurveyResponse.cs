using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mvc.dataaccess.Entities.Surveys
{
    public enum RiskLevel
    {
        Low = 0,
        Moderate = 1,
        High = 2,
        Severe = 3
    }

    public class SurveyResponse
    {
        [Key]
        public Guid ResponseId { get; set; } 
        
        public Guid SurveyId { get; set; }
        
        public Guid MemberId { get; set; }
        
        public int TotalScore { get; set; }
        
        public RiskLevel RiskLevel { get; set; }
        
        public bool IsCompleted { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        public virtual Survey Survey { get; set; }
        public virtual User Member { get; set; }
        public ICollection<UserAnswer> Answers { get; set; }
    }
}