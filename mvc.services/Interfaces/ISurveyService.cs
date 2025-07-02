using mvc.dataaccess.Entities.Surveys;
using mvc.dataaccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.services.Interfaces
{
    public interface ISurveyService
    {
        // Survey Management
        Task<IEnumerable<Survey>> GetAllSurveysAsync();
        Task<Survey> GetSurveyByIdAsync(Guid surveyId);
        Task<Survey> GetSurveyWithQuestionsAndOptionsAsync(Guid surveyId);
        Task<Survey> CreateSurveyAsync(Survey survey);
        Task<Survey> UpdateSurveyAsync(Survey survey);
        Task<bool> DeleteSurveyAsync(Guid surveyId);

        // Question Management
        Task<SurveyQuestion> GetQuestionWithOptionsAsync(Guid questionId);
        Task<SurveyQuestion> AddQuestionToSurveyAsync(Guid surveyId, SurveyQuestion question);
        Task<SurveyQuestion> UpdateQuestionAsync(SurveyQuestion question);
        Task<bool> DeleteQuestionAsync(Guid questionId);
        Task<bool> ReorderQuestionsAsync(Guid surveyId, List<Guid> questionIds); // For drag-and-drop reordering

        // Option Management
        Task<QuestionOption> GetOptionByIdAsync(Guid optionId);
        Task<QuestionOption> AddOptionToQuestionAsync(Guid questionId, QuestionOption option);
        Task<QuestionOption> UpdateOptionAsync(QuestionOption option);
        Task<bool> DeleteOptionAsync(Guid optionId);
        Task<bool> ReorderOptionsAsync(Guid questionId, List<Guid> optionIds);

        // Comprehensive Operations
        Task<Survey> CreateFullSurveyAsync(Survey survey, List<SurveyQuestionDTO> questionDtos);
        Task<bool> DuplicateSurveyAsync(Guid surveyId, string newTitle);

        // Survey Taking
        Task<SurveyResponse> StartSurveyAsync(Guid surveyId, Guid memberId);
        Task<bool> SaveAnswerAsync(Guid responseId, Guid questionId, Guid optionId);
        Task<SurveyResponse> CompleteSurveyAsync(Guid responseId);
        Task<IEnumerable<SurveyResponse>> GetUserSurveyHistoryAsync(Guid memberId);
    }
}
