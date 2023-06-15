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

services.Configure<SemanticKernelRAGServiceSettings>(configuration.GetSection("MSCosmosDBOpenAI"));

services.AddTransient<IRAGService, SemanticKernelRAGService>();

var serviceProvider = services.BuildServiceProvider();


var qm = serviceProvider.GetService<IRAGService>();

var userPrompt = "I am looking to buy a hat, can you help me?";
Console.WriteLine($"Your request:{Environment.NewLine}{userPrompt}");

var response = await qm.GetResponse(userPrompt);

Console.WriteLine($"My reposnse:{Environment.NewLine}{response}");

Console.ReadLine();