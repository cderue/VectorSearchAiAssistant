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
using Azure.AI.OpenAI;
using Azure.Search.Documents.Models;
using Azure.Search.Documents;
using System.Text.Json;

namespace SimilarQuestionsRetrieval
{
    public class QuestionMatchingService : IQuestionMatchingService
    {
        readonly QuestionMatchingServiceSettings _settings;
        readonly IKernel _semanticKernel;
        readonly string _memoryCollectionName = "vector-index";

        private readonly string _systemPromptRetailAssistant = @"
        You are an intelligent assistant for the Cosmic Works Bike Company. 
        You are designed to provide helpful answers to user questions about 
        product, product category, customer and sales order (salesOrder) information provided in JSON format below.

        Instructions:
        - Only answer questions related to the information provided below,
        - Don't reference any product, customer, or salesOrder data not provided below.
        - If you're unsure of an answer, you can say ""I don't know"" or ""I'm not sure"" and recommend users search themselves.

        Text of relevant information:";

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
        }

        public async Task AddQuestion(Question question)
        {
            await _semanticKernel.Memory.SaveInformationAsync(_memoryCollectionName, id: question.Id, text: question.Text);
        }

        public Task<IEnumerable<Question>> GetSimilarQuestions(Question question)
        {
            throw new NotImplementedException();
        }

        public async Task RunDemo(string userPrompt)
        {
            var matchingMemories = await SearchMemoriesAsync(userPrompt);

            var chat = _semanticKernel.GetService<IChatCompletion>();

            var chatHistory = chat.CreateNewChat($"{_systemPromptRetailAssistant}{matchingMemories}");

            chatHistory.AddUserMessage(userPrompt);

            var reply = await chat.GenerateMessageAsync(chatHistory, new ChatRequestSettings());
            chatHistory.AddAssistantMessage(reply);
            

            Console.WriteLine($"Your prompt: {userPrompt}");
            Console.WriteLine();
            Console.WriteLine($"My response: {reply}");

            Console.ReadLine();
        }

        private async Task<string> SearchMemoriesAsync(string query)
        {
            var retDocs = new List<string>();
            string resultDocuments = string.Empty;

            try
            {
                var searchResults = await _semanticKernel.Memory
                    .SearchAsync(_memoryCollectionName, query, limit:10, withEmbeddings: true)
                    .ToListAsync();

                return string.Join(Environment.NewLine + "-", 
                    searchResults.Select(sr => sr.Metadata.AdditionalMetadata));
            }
            catch (Exception ex)
            {
                //_logger.LogError($"There was an error conducting a vector search: {ex.Message}");
            }

            return resultDocuments;
        }
    }
}
