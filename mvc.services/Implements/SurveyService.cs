using mvc.dataaccess.Entities.Surveys;
using mvc.dataaccess.ViewModels;
using mvc.repositories.Interfaces;
using mvc.services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.services.Implements
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _surveyRepository;

        public SurveyService(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        #region Survey Management

        public async Task<IEnumerable<Survey>> GetAllSurveysAsync()
        {
            return await _surveyRepository.GetAllSurveysAsync();
        }

        public async Task<Survey> GetSurveyByIdAsync(Guid surveyId)
        {
            return await _surveyRepository.GetByIdAsync(surveyId);
        }

        public async Task<Survey> GetSurveyWithQuestionsAndOptionsAsync(Guid surveyId)
        {
            return await _surveyRepository.GetSurveyWithQuestionsAsync(surveyId);
        }

        public async Task<Survey> CreateSurveyAsync(Survey survey)
        {
            return await _surveyRepository.AddAsync(survey);
        }

        public async Task<Survey> UpdateSurveyAsync(Survey survey)
        {
            return await _surveyRepository.UpdateAsync(survey);
        }

        public async Task<bool> DeleteSurveyAsync(Guid surveyId)
        {
            return await _surveyRepository.DeleteAsync(surveyId);
        }

        #endregion

        #region Question Management

        public async Task<SurveyQuestion> GetQuestionWithOptionsAsync(Guid questionId)
        {
            return await _surveyRepository.GetQuestionWithOptionsAsync(questionId);
        }

        public async Task<SurveyQuestion> AddQuestionToSurveyAsync(Guid surveyId, SurveyQuestion question)
        {
            question.SurveyId = surveyId;
            return await _surveyRepository.AddQuestionAsync(question);
        }

        public async Task<SurveyQuestion> UpdateQuestionAsync(SurveyQuestion question)
        {
            return await _surveyRepository.UpdateQuestionAsync(question);
        }

        public async Task<bool> DeleteQuestionAsync(Guid questionId)
        {
            return await _surveyRepository.DeleteQuestionAsync(questionId);
        }

        public async Task<bool> ReorderQuestionsAsync(Guid surveyId, List<Guid> questionIds)
        {
            try
            {
                var survey = await _surveyRepository.GetByIdAsync(surveyId);
                if (survey == null) return false;

                // Get all questions for this survey
                var questions = (await _surveyRepository.GetSurveyWithQuestionsAsync(surveyId)).Questions.ToList();

                // Update order index for each question based on the provided order
                for (int i = 0; i < questionIds.Count; i++)
                {
                    var question = questions.FirstOrDefault(q => q.QuestionId == questionIds[i]);
                    if (question != null)
                    {
                        question.OrderIndex = i;
                        await _surveyRepository.UpdateQuestionAsync(question);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Option Management

        public async Task<QuestionOption> GetOptionByIdAsync(Guid optionId)
        {
            // You might need to add this method to the repository as well
            return await _surveyRepository.GetOptionByIdAsync(optionId);
        }

        public async Task<QuestionOption> AddOptionToQuestionAsync(Guid questionId, QuestionOption option)
        {
            option.QuestionId = questionId;
            return await _surveyRepository.AddOptionAsync(option);
        }

        public async Task<QuestionOption> UpdateOptionAsync(QuestionOption option)
        {
            return await _surveyRepository.UpdateOptionAsync(option);
        }

        public async Task<bool> DeleteOptionAsync(Guid optionId)
        {
            return await _surveyRepository.DeleteOptionAsync(optionId);
        }

        public async Task<bool> ReorderOptionsAsync(Guid questionId, List<Guid> optionIds)
        {
            try
            {
                var question = await _surveyRepository.GetQuestionWithOptionsAsync(questionId);
                if (question == null) return false;

                // Update order index for each option based on the provided order
                for (int i = 0; i < optionIds.Count; i++)
                {
                    var option = question.Options.FirstOrDefault(o => o.OptionId == optionIds[i]);
                    if (option != null)
                    {
                        option.OrderIndex = i;
                        await _surveyRepository.UpdateOptionAsync(option);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Comprehensive Operations

        public async Task<Survey> CreateFullSurveyAsync(Survey survey, List<SurveyQuestionDTO> questionDtos)
        {
            // Create the survey first
            var createdSurvey = await _surveyRepository.AddAsync(survey);

            // Add each question with its options
            foreach (var questionDto in questionDtos)
            {
                questionDto.Question.SurveyId = createdSurvey.SurveyId;
                var createdQuestion = await _surveyRepository.AddQuestionAsync(questionDto.Question);

                // Add options for this question
                foreach (var option in questionDto.Options)
                {
                    option.QuestionId = createdQuestion.QuestionId;
                    await _surveyRepository.AddOptionAsync(option);
                }
            }

            // Return the full survey with questions and options
            return await _surveyRepository.GetSurveyWithQuestionsAsync(createdSurvey.SurveyId);
        }

        public async Task<bool> DuplicateSurveyAsync(Guid surveyId, string newTitle)
        {
            try
            {
                // Get the original survey with all questions and options
                var originalSurvey = await _surveyRepository.GetSurveyWithQuestionsAsync(surveyId);
                if (originalSurvey == null) return false;

                // Create a new survey with the same properties but a new title
                var newSurvey = new Survey
                {
                    Title = newTitle,
                    Description = originalSurvey.Description,
                    Type = originalSurvey.Type,
                    IsActive = originalSurvey.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                // Add the new survey
                var createdSurvey = await _surveyRepository.AddAsync(newSurvey);

                // Copy all questions and their options
                foreach (var originalQuestion in originalSurvey.Questions)
                {
                    var newQuestion = new SurveyQuestion
                    {
                        SurveyId = createdSurvey.SurveyId,
                        QuestionText = originalQuestion.QuestionText,
                        OrderIndex = originalQuestion.OrderIndex
                    };

                    var createdQuestion = await _surveyRepository.AddQuestionAsync(newQuestion);

                    // Copy all options for this question
                    foreach (var originalOption in originalQuestion.Options)
                    {
                        var newOption = new QuestionOption
                        {
                            QuestionId = createdQuestion.QuestionId,
                            OptionText = originalOption.OptionText,
                            Score = originalOption.Score,
                            OrderIndex = originalOption.OrderIndex
                        };

                        await _surveyRepository.AddOptionAsync(newOption);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Survey Taking

        public async Task<SurveyResponse> StartSurveyAsync(Guid surveyId, Guid memberId)
        {
            return await _surveyRepository.StartSurveyAsync(surveyId, memberId);
        }

        public async Task<bool> SaveAnswerAsync(Guid responseId, Guid questionId, Guid optionId)
        {
            return await _surveyRepository.SaveAnswerAsync(responseId, questionId, optionId);
        }

        public async Task<SurveyResponse> CompleteSurveyAsync(Guid responseId)
        {
            return await _surveyRepository.CompleteSurveyAsync(responseId);
        }

        public async Task<IEnumerable<SurveyResponse>> GetUserSurveyHistoryAsync(Guid memberId)
        {
            return await _surveyRepository.GetUserSurveyHistoryAsync(memberId);
        }

        #endregion
    }
}