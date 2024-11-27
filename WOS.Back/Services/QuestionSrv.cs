using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Model;
using WOS.Dal.Context;
using WOS.Dal.Interfaces;

namespace WOS.Back.Services
{
    public class QuestionSrv : IQuestionSrv
    {
        private readonly WOSDbContext _context;

        public QuestionSrv(WOSDbContext context)
        {
            _context = context;
        }

        public void AddQuestion(Question question)
        {
            // Code pour ajouter un Actualite
            _context.Questions.Add(question);
            _context.SaveChanges();
        }

        public Question GetQuestion(int id)
        {
            // Code pour récupérer un Actualite
            var quest = _context.Questions.Find(id);

            return quest;
        }

        public List<Question> GetAllQuestions()
        {
            // Code pour récupérer tous les Actualites
            var questions = _context.Questions.ToList();

            return questions;
        }

        public void DeleteQuestion(int id)
        {
            var quest = _context.Questions.Find(id);
            _context.Questions.Remove(quest);
            _context.SaveChanges();
        }
    }
}
