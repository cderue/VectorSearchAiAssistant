using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarQuestionsRetrieval
{
    public interface IRAGService
    {
        Task<string> GetResponse(string userPrompt);
    }
}
