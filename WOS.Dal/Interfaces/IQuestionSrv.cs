using WOS.Model;

namespace WOS.Dal.Interfaces
{
    public interface IQuestionSrv
    {
        void AddQuestion(Question question);

        Question GetQuestion(int id);

        List<Question> GetAllQuestions();

        void DeleteQuestion(int id);
    }
}
