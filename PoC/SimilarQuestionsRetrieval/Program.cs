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

//await qm.AddQuestion(new Question { Id = "q000001", Text = "What kind of hats do you have?" });
//await qm.AddQuestion(new Question { Id = "q000002", Text = "Tell me about your hats offerings." });
//await qm.AddQuestion(new Question { Id = "q000003", Text = "Do you sell any kind of groceries?" });
//await qm.AddQuestion(new Question { Id = "q000004", Text = "Do you have blue or green socks?" });
//await qm.AddQuestion(new Question { Id = "q000005", Text = "I am looking to buy a new race car. Do you have any?" });

await qm.RunDemo();

Console.ReadLine();