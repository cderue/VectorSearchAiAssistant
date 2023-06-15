using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarQuestionsRetrieval
{
    public interface IQuestionMatchingService
    {
        Task RunDemo(string userPrompt);
        Task AddQuestion(Question question);
        Task<IEnumerable<Question>> GetSimilarQuestions(Question question);
    }
}
