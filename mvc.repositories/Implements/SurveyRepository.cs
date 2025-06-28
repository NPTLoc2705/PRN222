using Microsoft.EntityFrameworkCore;
using mvc.dataaccess.Entities;
using mvc.dataaccess.Entities.Surveys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.repositories.Implements
{
    public class SurveyRepository
    {
        private readonly AppDbContext _context;

        public SurveyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Survey>> GetAllSurveysAsync()
        {
            return await _context.Surveys
                .Where(s => s.IsActive)
                .OrderBy(s => s.Title)
                .ToListAsync();
        }

        public async Task<Survey> GetSurveyWithQuestionsAsync(Guid surveyId)
        {
            return await _context.Surveys
                .Include(s => s.Questions.OrderBy(q => q.OrderIndex))
                    .ThenInclude(q => q.Options.OrderBy(o => o.OrderIndex))
                .FirstOrDefaultAsync(s => s.SurveyId == surveyId && s.IsActive);
        }

        public async Task<SurveyResponse> StartSurveyAsync(Guid surveyId, Guid memberId)
        {
            var response = new SurveyResponse
            {
                SurveyId = surveyId,
                MemberId = memberId
            };

            _context.SurveyResponses.Add(response);
            await _context.SaveChangesAsync();
            return response;
        }

        public async Task<bool> SaveAnswerAsync(Guid responseId, Guid questionId, Guid optionId)
        {
            // Remove existing answer if any
            var existingAnswer = await _context.UserAnswers
                .FirstOrDefaultAsync(ua => ua.ResponseId == responseId && ua.QuestionId == questionId);

            if (existingAnswer != null)
            {
                _context.UserAnswers.Remove(existingAnswer);
            }

            // Get option score
            var option = await _context.QuestionOptions
                .FirstOrDefaultAsync(qo => qo.OptionId == optionId);

            if (option == null) return false;

            // Add new answer
            var answer = new UserAnswer
            {
                ResponseId = responseId,
                QuestionId = questionId,
                OptionId = optionId,
                Score = option.Score
            };

            _context.UserAnswers.Add(answer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SurveyResponse> CompleteSurveyAsync(Guid responseId)
        {
            var response = await _context.SurveyResponses
                .Include(sr => sr.Answers)
                .Include(sr => sr.Survey)
                .FirstOrDefaultAsync(sr => sr.ResponseId == responseId);

            if (response == null) return null;

            // Calculate total score
            response.TotalScore = response.Answers.Sum(a => a.Score);

            // Determine risk level based on survey type and score
            response.RiskLevel = DetermineRiskLevel(response.Survey.Type, response.TotalScore);

            // Create recommended actions
            await CreateRecommendedActions(responseId, response.RiskLevel);

            response.IsCompleted = true;
            response.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return response;
        }

        public async Task<IEnumerable<SurveyResponse>> GetUserSurveyHistoryAsync(Guid memberId)
        {
            return await _context.SurveyResponses
                .Include(sr => sr.Survey)
                .Where(sr => sr.MemberId == memberId && sr.IsCompleted)
                .OrderByDescending(sr => sr.CompletedAt)
                .ToListAsync();
        }

        private RiskLevel DetermineRiskLevel(SurveyType surveyType, int totalScore)
        {
            return surveyType switch
            {
                SurveyType.ASSIST => totalScore switch
                {
                    <= 10 => RiskLevel.Low,
                    <= 26 => RiskLevel.Moderate,
                    _ => RiskLevel.High
                },
                SurveyType.CRAFFT => totalScore switch
                {
                    <= 1 => RiskLevel.Low,
                    _ => RiskLevel.High
                },
                SurveyType.DAST => totalScore switch
                {
                    0 => RiskLevel.Low,
                    <= 2 => RiskLevel.Low,
                    <= 5 => RiskLevel.Moderate,
                    <= 8 => RiskLevel.High,
                    _ => RiskLevel.Severe
                },
                _ => RiskLevel.Low
            };
        }

        private async Task CreateRecommendedActions(Guid responseId, RiskLevel riskLevel)
        {
            var actions = new List<RecommendedAction>();

            switch (riskLevel)
            {
                case RiskLevel.Low:
                    actions.Add(new RecommendedAction
                    {
                        ResponseId = responseId,
                        Title = "Educational Resources",
                        Description = "Access self-help materials and educational content about substance use.",
                        Type = ActionType.SelfHelp,
                        RequiredRiskLevel = RiskLevel.Low
                    });
                    break;

                case RiskLevel.Moderate:
                    actions.Add(new RecommendedAction
                    {
                        ResponseId = responseId,
                        Title = "Attend Training Course",
                        Description = "Participate in our substance abuse awareness training program.",
                        Type = ActionType.Training,
                        RequiredRiskLevel = RiskLevel.Moderate
                    });
                    actions.Add(new RecommendedAction
                    {
                        ResponseId = responseId,
                        Title = "Schedule Consultation",
                        Description = "Meet with a qualified counselor to discuss your results.",
                        Type = ActionType.Consultation,
                        RequiredRiskLevel = RiskLevel.Moderate
                    });
                    break;

                case RiskLevel.High:
                case RiskLevel.Severe:
                    actions.Add(new RecommendedAction
                    {
                        ResponseId = responseId,
                        Title = "Professional Assessment",
                        Description = "Schedule an immediate consultation with a specialist.",
                        Type = ActionType.Consultation,
                        RequiredRiskLevel = RiskLevel.High
                    });
                    actions.Add(new RecommendedAction
                    {
                        ResponseId = responseId,
                        Title = "Crisis Support",
                        Description = "Access emergency support resources and hotlines.",
                        Type = ActionType.Emergency,
                        RequiredRiskLevel = RiskLevel.High
                    });
                    break;
            }

            _context.RecommendedActions.AddRange(actions);
        }
    }
}
