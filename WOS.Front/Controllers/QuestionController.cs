using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using WOS.Dal.Interfaces;
using WOS.Model;

namespace WOS.Front.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    public class QuestionController : Controller
    {
        private readonly IQuestionSrv _questionSrv;

        public QuestionController(IQuestionSrv questionSrv)
        {
            _questionSrv = questionSrv;
        }
        // GET: QuestionController
        [Route("")]
        public ActionResult Index()
        {
            List<Question> questions = _questionSrv.GetAllQuestions();
            return View(questions);
        }

        [HttpPost]
        [Route("AddQuestion")]
        public IActionResult AddQuestion(string question, string reponse)
        {
            Question newQuestion = new Question
            {
                Intitule = question,
                Reponse = reponse
            };

            _questionSrv.AddQuestion(newQuestion);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("DeleteQuestion")]
        public IActionResult DeleteQuestion(int id)
        {
            _questionSrv.DeleteQuestion(id);

            return RedirectToAction("Index");
        }
    }
}
