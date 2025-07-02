using mvc.dataaccess.Entities.Surveys;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.dataaccess.ViewModels
{
    public class SurveyDTO
    {
        public Survey Survey { get; set; }
        public List<SurveyQuestionDTO> QuestionDtos { get; set; } = new List<SurveyQuestionDTO>();

        public class SurveyListViewModel
        {
            public IEnumerable<Survey> Surveys { get; set; }
        }

        public class TakeSurveyViewModel
        {
            public Survey Survey { get; set; }
            public Guid ResponseId { get; set; }
            public Dictionary<Guid, Guid> UserAnswers { get; set; } = new Dictionary<Guid, Guid>();
        }

        public class SurveyResultViewModel
        {
            public SurveyResponse Response { get; set; }
            public string RiskLevelText { get; set; }
            public string RiskLevelColor { get; set; }
            public IEnumerable<RecommendedAction> Actions { get; set; }
        }

        public class SubmitAnswerModel
        {
            [Required]
            public Guid ResponseId { get; set; }

            [Required]
            public Guid QuestionId { get; set; }

            [Required]
            public Guid OptionId { get; set; }
        }
    }
}
