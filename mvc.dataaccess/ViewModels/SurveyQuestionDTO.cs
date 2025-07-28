using mvc.dataaccess.Entities.Surveys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.dataaccess.ViewModels
{
    public class SurveyQuestionDTO
    {
        public SurveyQuestionDTO()
        {
            // Initialize objects to prevent null reference exceptions
            Question = new SurveyQuestion();
            Options = new List<QuestionOption>();
        }

        public SurveyQuestion Question { get; set; }
        public List<QuestionOption> Options { get; set; }
    }
}
