using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarQuestionsRetrieval
{
    public class QuestionMatchingServiceSettings
    {
        public string OpenAIKey { get; set; }
        public string OpenAIEmbeddingDeploymentName { get; set; }
        public string OpenAICompletionDeploymentName { get; set; }
        public string OpenAIEndpoint { get; set; }
        public string CognitiveSearchKey { get; set; }
        public string CognitiveSearchEndpoint { get; set; }
    }
}
