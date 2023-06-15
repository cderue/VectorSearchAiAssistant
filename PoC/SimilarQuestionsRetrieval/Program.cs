using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Memory;
using Microsoft.Extensions.Configuration;
using SimilarQuestionsRetrieval;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

services.Configure<QuestionMatchingServiceSettings>(configuration.GetSection("MSCosmosDBOpenAI"));

services.AddTransient<IQuestionMatchingService, QuestionMatchingService>();

var serviceProvider = services.BuildServiceProvider();


var qm = serviceProvider.GetService<IQuestionMatchingService>();

var userPrompt = "I am looking to buy a hat, can you help me?";
await qm.RunDemo(userPrompt);

Console.ReadLine();