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
        private readonly IGlobalDataSrv _globalDataSrv;

        public QuestionSrv(WOSDbContext context, IGlobalDataSrv globalDataSrv)
        {
            _context = context;
            _globalDataSrv = globalDataSrv;
        }

        public void AddQuestion(Question question)
        {
            // Code pour ajouter un Actualite
            _context.Questions.Add(question);
            _context.SaveChanges();

            _globalDataSrv.RefreshCacheAsync(typeof(Question));
        }

        public Question GetQuestion(int id)
        {
            // Code pour récupérer un Actualite
            var quest = _globalDataSrv.Questions.Find(q => q.Id == id);

            return quest;
        }

        public void DeleteQuestion(int id)
        {
            var quest = _context.Questions.Find(id);
            _context.Questions.Remove(quest);
            _context.SaveChanges();
            _globalDataSrv.RefreshCacheAsync(typeof(Question));
        }
    }
}
