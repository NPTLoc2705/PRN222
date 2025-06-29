using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvc.services.Interfaces;
using mvc.dataaccess.ViewModels;
using mvc.dataaccess.Entities.Surveys;

namespace mvc.app.Controllers
{
    public class SurveysController : Controller
    {
        private readonly ISurveyService _surveyService;

        public SurveysController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        // GET: Surveys
        public async Task<IActionResult> Index()
        {
            var surveys = await _surveyService.GetAvailableSurveysAsync();
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

            var survey = await _surveyService.GetSurveyAsync(id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }

        // GET: Surveys/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Surveys/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SurveyId,Title,Description,Type,IsActive,CreatedAt")] Survey survey)
        {
            if (ModelState.IsValid)
            {
                survey.SurveyId = Guid.NewGuid();
                // You may need to implement an AddSurveyAsync in ISurveyService/Repository
                // await _surveyService.AddSurveyAsync(survey);
                // For now, just return to Index
                return RedirectToAction(nameof(Index));
            }
            return View(survey);
        }

        // GET: Surveys/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var survey = await _surveyService.GetSurveyAsync(id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }

        // POST: Surveys/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("SurveyId,Title,Description,Type,IsActive,CreatedAt")] Survey survey)
        {
            if (id != survey.SurveyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // You may need to implement an UpdateSurveyAsync in ISurveyService/Repository
                    // await _surveyService.UpdateSurveyAsync(survey);
                }
                catch (Exception)
                {
                    var existing = await _surveyService.GetSurveyAsync(survey.SurveyId);
                    if (existing == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(survey);
        }

        // GET: Surveys/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var survey = await _surveyService.GetSurveyAsync(id);
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
            // You may need to implement a DeleteSurveyAsync in ISurveyService/Repository
            // await _surveyService.DeleteSurveyAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
