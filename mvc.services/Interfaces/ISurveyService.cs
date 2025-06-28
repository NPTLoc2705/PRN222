using mvc.dataaccess.Entities.Surveys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.services.Interfaces
{
    public interface ISurveyService
    {
        Task<IEnumerable<Survey>> GetAvailableSurveysAsync();
        Task<Survey> GetSurveyAsync(Guid surveyId);
        Task<SurveyResponse> StartSurveyAsync(Guid surveyId, Guid memberId);
        Task<bool> SubmitAnswerAsync(Guid responseId, Guid questionId, Guid optionId);
        Task<SurveyResponse> CompleteSurveyAsync(Guid responseId);
        Task<IEnumerable<SurveyResponse>> GetUserHistoryAsync(Guid memberId);
    }
}
