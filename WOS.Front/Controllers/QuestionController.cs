using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using WOS.Dal.Interfaces;
using WOS.Model;

namespace WOS.Front.Controllers
{
    [Route("[controller]")]
    public class QuestionController : Controller
    {
        private readonly IQuestionSrv _questionSrv;
        private readonly IGlobalDataSrv _globalDataSrv;

        public QuestionController(IQuestionSrv questionSrv, IGlobalDataSrv globalDataSrv)
        {
            _questionSrv = questionSrv;
            _globalDataSrv = globalDataSrv;
        }
        // GET: QuestionController
        [Route("")]
        public ActionResult Index()
        {
            List<Question> questions = _globalDataSrv.Questions;
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
