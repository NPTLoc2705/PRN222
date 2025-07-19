using System;
using System.ComponentModel.DataAnnotations;

namespace mvc.dataaccess.Entities.Surveys
{
    public enum ActionType
    {
        SelfHelp,
        Training,
        Consultation,
        Emergency
    }

    public class RecommendedAction
    {
        [Key]
        public Guid ActionId { get; set; }
        
        public Guid ResponseId { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        
        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }
        
        public ActionType Type { get; set; }
        
        public RiskLevel RequiredRiskLevel { get; set; }
        
        public bool IsCompleted { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual SurveyResponse Response { get; set; }
    }
}