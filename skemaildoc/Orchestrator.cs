using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Planning.Handlebars;
using Plugins;

namespace skemaildoc
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

            // Add the Sound plugin to the kernel
            builder.Plugins.AddFromType<EmailDocPlugin>();

            // Build kernel
            var kernel = builder.Build();

            // Create planner
            #pragma warning disable SKEXP0060
            var planner = new HandlebarsPlanner(new HandlebarsPlannerOptions() { AllowLoops = true });

            // Start the conversation
            while (true)
            {
                // Get user input
                Console.Write("User > ");
                var request = Console.ReadLine();
                if (request == "fin")
                {
                    break;
                }

                var message = "";
                // Try to create the execution plan
                try
                {
                    #pragma warning disable CS8604
                    var plan = await planner.CreatePlanAsync(kernel, request);
                    Console.WriteLine($"plan: {plan}");
                    message = (
                        await plan.InvokeAsync(
                            kernel,
                            []
                        )
                    ).Trim();
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    if (message.Contains("Planner output:"))
                    {
                        message = message.Substring(message.IndexOf("Planner output:") + 16);
                    }
                }
                Console.WriteLine($"assistant: {message}");

            }
        }
    }
}


