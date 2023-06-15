using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimilarQuestionsRetrieval.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.AI.Embeddings;

namespace SimilarQuestionsRetrieval
{
    public class QuestionMatchingService : IQuestionMatchingService
    {
        readonly QuestionMatchingServiceSettings _settings;
        readonly IKernel _semanticKernel;
        readonly string _memoryCollectionName;

        public QuestionMatchingService(
            IOptions<QuestionMatchingServiceSettings> options)
        {
            _settings = options.Value;

            var builder = new KernelBuilder();

            builder.WithAzureTextEmbeddingGenerationService(
                _settings.OpenAIEmbeddingDeploymentName,
                _settings.OpenAIEndpoint,
                _settings.OpenAIKey);

            builder.WithAzureChatCompletionService(
                _settings.OpenAICompletionDeploymentName, 
                _settings.OpenAIEndpoint,
                _settings.OpenAIKey);

            _semanticKernel = builder.Build();

            _semanticKernel.RegisterMemory(new AzureCognitiveSearchVectorMemory(
                _settings.CognitiveSearchEndpoint,
                _settings.CognitiveSearchKey,
                _semanticKernel.GetService<ITextEmbeddingGeneration>()));

            _memoryCollectionName = "vector-index";
        }

        public async Task AddQuestion(Question question)
        {
            await _semanticKernel.Memory.SaveInformationAsync(_memoryCollectionName, id: question.Id, text: question.Text);
        }

        public Task<IEnumerable<Question>> GetSimilarQuestions(Question question)
        {
            throw new NotImplementedException();
        }

        public async Task RunDemo()
        {
            var newQuestion = "I am looking to buy a hat, can you help me?";

            string systemPrompt = "You are an online store assistant.";
            var chat = _semanticKernel.GetService<IChatCompletion>();

            var chatHistory = chat.CreateNewChat(systemPrompt);

            var matchingQuestions = await _semanticKernel.Memory.SearchAsync(_memoryCollectionName, newQuestion, withEmbeddings: true).Take(3).ToListAsync();
            chatHistory.AddUserMessage(newQuestion);
            var reply = await chat.GenerateMessageAsync(chatHistory, new ChatRequestSettings());
            chatHistory.AddAssistantMessage(reply);

            

            Console.WriteLine($"Your question: {newQuestion}");
            Console.WriteLine();
            Console.WriteLine($"Questions already asked that are similar to yours:");

            foreach (var matchingQuestion in matchingQuestions)
                Console.WriteLine($"Id: {matchingQuestion.Metadata.Id}, Question: {matchingQuestion.Metadata.Text}");

            Console.ReadLine();
        }
    }
}
