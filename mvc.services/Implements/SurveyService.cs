using mvc.dataaccess.Entities.Surveys;
using mvc.repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.services.Implements
{
    public class SurveyService
    {
        private readonly ISurveyRepository _surveyRepository;

        public SurveyService(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        public async Task<IEnumerable<Survey>> GetAvailableSurveysAsync()
        {
            return await _surveyRepository.GetAllSurveysAsync();
        }

        public async Task<Survey> GetSurveyAsync(Guid surveyId)
        {
            return await _surveyRepository.GetSurveyWithQuestionsAsync(surveyId);
        }

        public async Task<SurveyResponse> StartSurveyAsync(Guid surveyId, Guid memberId)
        {
            return await _surveyRepository.StartSurveyAsync(surveyId, memberId);
        }

        public async Task<bool> SubmitAnswerAsync(Guid responseId, Guid questionId, Guid optionId)
        {
            return await _surveyRepository.SaveAnswerAsync(responseId, questionId, optionId);
        }

        public async Task<SurveyResponse> CompleteSurveyAsync(Guid responseId)
        {
            return await _surveyRepository.CompleteSurveyAsync(responseId);
        }

        public async Task<IEnumerable<SurveyResponse>> GetUserHistoryAsync(Guid memberId)
        {
            return await _surveyRepository.GetUserSurveyHistoryAsync(memberId);
        }
    }
}
