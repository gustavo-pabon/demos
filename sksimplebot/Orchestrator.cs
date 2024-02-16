using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.Extensions.Configuration;

namespace sksimplebot
{
    class Orchestrator
    {
        public static async Task Main(string[] args)
        {
            // Get prompt name from command line arguments
            var promptName = args.Length > 0 ? args[0] : "Chat";

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
            #pragma warning disable SKEXP0050
            builder.Plugins.AddFromType<ConversationSummaryPlugin>();

            // Build kernel
            var kernel = builder.Build();

            // Uncomment to see the prompts being used
            //#pragma warning disable SKEXP0004
            //kernel.PromptRendered += (sender, args) => Console.WriteLine("# SENDER:" + sender + "\n# PROMPT:" + args.RenderedPrompt);


            // Load prompts
            var prompts = kernel.CreatePluginFromPromptDirectory("Prompts");

            // Create chat history
            ChatHistory history = [];

            // Create initial system prompt
            Console.WriteLine("Sistema > ");
            var initialPrompt = "";
            var newLine = "";
            while (newLine != "fin")
            {
                initialPrompt += newLine + "\n";
                newLine = Console.ReadLine();
            }

            // Start the chat loop
            while(true)
            {
                // Get user input
                Console.Write("Usuario > ");
                var request = Console.ReadLine();
                if (request == "fin") 
                {
                    break;
                }

                // Get chat response
                var chatResult = kernel.InvokeStreamingAsync<StreamingChatMessageContent>(
                    prompts[promptName],
                    new ()
                    {
                        { "request", request },
                        { "history", string.Join("\n", history.Select(x => x.Role + ": " + x.Content)) },
                        { "initialPrompt", initialPrompt }
                    }
                );

                // Stream the response
                string message = "";
                await foreach (var chunk in chatResult)
                {
                    if (chunk.Role.HasValue) Console.Write(chunk.Role + ": ");
                    message += chunk;
                    Console.Write(chunk);
                }
                Console.WriteLine();
                Console.WriteLine();

                // Append to history
                history.AddUserMessage(request!);
                history.AddAssistantMessage(message);
            }
        }
    }
}


