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
        Task<Survey> GetByIdAsync(Guid id);
        Task<Survey> AddAsync(Survey survey);
        Task<Survey> UpdateAsync(Survey survey);
        Task<bool> DeleteAsync(Guid id);

        // Add these method signatures to your interface
        Task<SurveyQuestion> AddQuestionAsync(SurveyQuestion question);
        Task<SurveyQuestion> UpdateQuestionAsync(SurveyQuestion question);
        Task<bool> DeleteQuestionAsync(Guid questionId);
        Task<QuestionOption> AddOptionAsync(QuestionOption option);
        Task<QuestionOption> UpdateOptionAsync(QuestionOption option);
        Task<bool> DeleteOptionAsync(Guid optionId);
        Task<SurveyQuestion> GetQuestionWithOptionsAsync(Guid questionId);
        Task<QuestionOption> GetOptionByIdAsync(Guid optionId);
    }
}
