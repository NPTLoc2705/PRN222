using mvc.dataaccess.Entities.Surveys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.repositories.Interfaces
{
    public interface ISurveyRepository
    {
        Task<IEnumerable<Survey>> GetAllSurveysAsync();
        Task<Survey> GetSurveyWithQuestionsAsync(Guid surveyId);
        Task<SurveyResponse> StartSurveyAsync(Guid surveyId, Guid memberId);
        Task<bool> SaveAnswerAsync(Guid responseId, Guid questionId, Guid optionId);
        Task<SurveyResponse> CompleteSurveyAsync(Guid responseId);
        Task<IEnumerable<SurveyResponse>> GetUserSurveyHistoryAsync(Guid memberId);
    }
}
