using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvc.services.Interfaces;
using mvc.dataaccess.ViewModels;
using mvc.dataaccess.Entities.Surveys;

namespace mvc.app.Controllers
{
    public class SurveysController : BaseController
    {
        private readonly ISurveyService _surveyService;

        public SurveysController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        // GET: Surveys
        public async Task<IActionResult> Index()
        {
            var surveys = await _surveyService.GetAllSurveysAsync();
            var model = new SurveyDTO.SurveyListViewModel { Surveys = surveys };
            return View(model.Surveys);
        }

        // GET: Surveys/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            // Use GetSurveyWithQuestionsAndOptionsAsync instead of GetSurveyByIdAsync
            var survey = await _surveyService.GetSurveyWithQuestionsAndOptionsAsync(id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }

        // GET: Surveys/Create
        public IActionResult Create()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Index");
            return View();
        }

        // POST: Surveys/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SurveyDTO model)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Index");
            model.Survey.SurveyId = Guid.NewGuid();
            model.Survey.CreatedAt = DateTime.UtcNow;

            // Use the comprehensive method to create survey with questions and options
            var survey = await _surveyService.CreateFullSurveyAsync(model.Survey, model.QuestionDtos);

            return RedirectToAction(nameof(Details), new { id = survey.SurveyId });
        }

        // GET: Surveys/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Index");

            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var survey = await _surveyService.GetSurveyWithQuestionsAndOptionsAsync(id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }

        // POST: Surveys/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Survey survey)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Index");

            if (survey == null || survey.SurveyId == Guid.Empty)
                return NotFound();

            await _surveyService.UpdateSurveyAsync(survey);

            // Optionally, show a success message or redirect to details
            return RedirectToAction("Details", new { id = survey.SurveyId });
        }

        // POST: Surveys/UpdateQuestion/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuestion(Guid id, [Bind("QuestionId,QuestionText,OrderIndex")] SurveyQuestion question)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Index");

            if (id != question.QuestionId)
            {
                return BadRequest();
            }

            // Get the existing question to preserve the SurveyId
            var existingQuestion = await _surveyService.GetQuestionWithOptionsAsync(id);
            if (existingQuestion == null)
            {
                return NotFound();
            }

            // Update only the allowed fields
            existingQuestion.QuestionText = question.QuestionText;
            existingQuestion.OrderIndex = question.OrderIndex;

            await _surveyService.UpdateQuestionAsync(existingQuestion);
            return Ok();
        }

        // POST: Surveys/DeleteQuestion/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuestion(Guid id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Index");

            var result = await _surveyService.DeleteQuestionAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return Ok();
        }

        // POST: Surveys/UpdateOption/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOption(Guid id, [Bind("OptionId,OptionText,Score,OrderIndex")] QuestionOption option)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Index");

            if (id != option.OptionId)
            {
                return BadRequest();
            }

            // Get the existing option to preserve the QuestionId
            var existingOption = await _surveyService.GetOptionByIdAsync(id);
            if (existingOption == null)
            {
                return NotFound();
            }

            // Update only the allowed fields
            existingOption.OptionText = option.OptionText;
            existingOption.Score = option.Score;
            existingOption.OrderIndex = option.OrderIndex;

            await _surveyService.UpdateOptionAsync(existingOption);
            return Ok();
        }

        // POST: Surveys/DeleteOption/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOption(Guid id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Index");

            var result = await _surveyService.DeleteOptionAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return Ok();
        }

        // POST: Surveys/AddQuestion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuestion([Bind("SurveyId,QuestionText,OrderIndex")] SurveyQuestion question)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Index");

            if (question.SurveyId == Guid.Empty)
            {
                return BadRequest();
            }

            question.QuestionId = Guid.NewGuid();
            var createdQuestion = await _surveyService.AddQuestionToSurveyAsync(question.SurveyId, question);
            return Json(createdQuestion);
        }

        // POST: Surveys/AddOption
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOption([Bind("QuestionId,OptionText,Score,OrderIndex")] QuestionOption option)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Index");

            if (option.QuestionId == Guid.Empty)
            {
                return BadRequest();
            }

            option.OptionId = Guid.NewGuid();
            var createdOption = await _surveyService.AddOptionToQuestionAsync(option.QuestionId, option);
            return Json(createdOption);
        }

        // GET: Surveys/GetOptionByIdAsync/{id}
        public async Task<IActionResult> GetOptionByIdAsync(Guid id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Index");

            var option = await _surveyService.GetOptionByIdAsync(id);
            if (option == null)
            {
                return NotFound();
            }
            return Json(option);
        }

        // GET: Surveys/GetQuestionWithOptionsAsync/{id}
        public async Task<IActionResult> GetQuestionWithOptionsAsync(Guid id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Index");

            var question = await _surveyService.GetQuestionWithOptionsAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            return Json(question);
        }

        // GET: Surveys/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Index");

            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }

        // POST: Surveys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Index");

            var result = await _surveyService.DeleteSurveyAsync(id);

            if(result == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        #region Take Surveys

        // GET: Surveys/TakeSurvey/{id}
        public async Task<IActionResult> TakeSurvey(Guid id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == null)
                return RedirectToAction("Index");

            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var survey = await _surveyService.GetSurveyWithQuestionsAndOptionsAsync(id);
            if (survey == null)
            {
                return NotFound();
            }

            var memberId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(memberId) || !Guid.TryParse(memberId, out var parsedMemberId))
            {
                return Unauthorized();
            }

            var response = await _surveyService.StartSurveyAsync(id, parsedMemberId); // Replace Guid.NewGuid() with the actual member ID

            var model = new SurveyDTO.TakeSurveyViewModel
            {
                Survey = survey,
                ResponseId = response.ResponseId
            };

            return View(model);
        }

        // POST: Surveys/SubmitSurvey
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitSurvey(SurveyDTO.SubmitAnswerModel model)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == null)
                return RedirectToAction("Index");

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // Retrieve MemberId from session
            var memberIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(memberIdString) || !Guid.TryParse(memberIdString, out var memberId))
            {
                return Unauthorized(); // Session expired or user not logged in
            }

            foreach (var answer in model.UserAnswers)
            {
                await _surveyService.SaveAnswerAsync(model.ResponseId, answer.Key, answer.Value);
            }

            var completedResponse = await _surveyService.CompleteSurveyAsync(model.ResponseId);

            return RedirectToAction("SurveyResult", new { responseId = completedResponse.ResponseId });
        }

        // GET: Surveys/SurveyResult/{responseId}
        public async Task<IActionResult> SurveyResult(Guid responseId)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == null)
                return RedirectToAction("Index");

            var response = await _surveyService.GetSurveyResponseAsync(responseId);
            if (response == null)
            {
                return NotFound();
            }

            var model = new SurveyDTO.SurveyResultViewModel
            {
                Response = response,
                RiskLevelText = response.RiskLevel.ToString(),
                RiskLevelColor = response.RiskLevel switch
                {
                    RiskLevel.Low => "green",
                    RiskLevel.Moderate => "orange",
                    RiskLevel.High => "red",
                    RiskLevel.Severe => "darkred",
                    _ => "gray"
                },
                //Actions = response.RecommendedActions
            };

            return View(model);
        }

        #endregion
    }
}
