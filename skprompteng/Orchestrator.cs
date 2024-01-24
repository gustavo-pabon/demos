using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace skprompteng
{
    class Orchestrator
    {
        public static async Task Main(string[] args)
        {
            // Create a builder instance
            var builder = Kernel.CreateBuilder();
            
            // Load AzureOpenAI configuration from appsettings.json
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            var chatCompletionConfig = config.GetSection("AzureOpenAIChatCompletion");

            // Add Azure OpenAI Chat Completion service to the kernel
            builder.Services.AddAzureOpenAIChatCompletion(
                deploymentName: chatCompletionConfig["DeploymentName"] ?? "",
                endpoint: chatCompletionConfig["Endpoint"] ?? "",
                apiKey: chatCompletionConfig["ApiKey"] ?? ""
            );

            // Build kernel
            var kernel = builder.Build();

            // Load prompts
            var prompts = kernel.CreatePluginFromPromptDirectory("Prompts");

            // Create initial system prompt
            Console.WriteLine("Prompt inicial > ");
            var initialPrompt = "";
            var newLine = "";
            while (newLine != "fin")
            {
                initialPrompt += newLine + "\n";
                newLine = Console.ReadLine();
            }

            // Start the chat loop
            while (true)
            {
                // Get user input
                Console.Write("Usuario > ");
                var request = Console.ReadLine();
                if (request == "fin") {
                    break;
                }

                // Get chat response
                var chatResult = kernel.InvokeStreamingAsync<StreamingChatMessageContent>(
                    prompts["Solicitud"],
                    new ()
                    {
                        { "request", request },
                        { "initialPrompt", initialPrompt }
                    }
                );

                // Stream the response
                Console.WriteLine("\nRespuesta:\n");
                string message = "";
                await foreach (var chunk in chatResult)
                {
                    message += chunk;
                    Console.Write(chunk);
                }
                
                Console.WriteLine("\n");
            }
        }
    }
}


