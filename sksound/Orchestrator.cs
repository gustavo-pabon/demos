using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Configuration;
using Plugins;

namespace sksound
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
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
            builder.Plugins.AddFromType<SoundPlugin>();

            // Build kernel
            var kernel = builder.Build();

            // Uncomment to see the prompts being used
            //#pragma warning disable SKEXP0004
            //kernel.PromptRendered += (sender, args) => Console.WriteLine("# SENDER:" + sender + "\n# PROMPT:" + args.RenderedPrompt);

            // Load prompts
            var prompts = kernel.CreatePluginFromPromptDirectory("Prompts");

            // Create chat history
            ChatHistory history = [];

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

                // Check in the intention is to play a sound
                bool isSound = await kernel.InvokeAsync<bool>(
                    "SoundPlugin",
                    "CheckPlaySoundIntent",
                    new() { { "request", request }, { "kernel", kernel } }
                );

                // Create the assistant message
                string message = "";

                // If the intention is to play a sound, play it and continue with the chat
                if (isSound)
                {
                    await kernel.InvokeAsync("SoundPlugin", "PlaySound");
                    message = "Reproducción de sonido exitosa!";
                    Console.Write($"assistant: {message}");
                }
                // If the intention is not to play a sound, continue with the chat.
                else
                {
                    // Get chat response
                    var chatResult = kernel.InvokeStreamingAsync<StreamingChatMessageContent>(
                        prompts["Chat"],
                        new()
                        {
                            { "request", request },
                            { "history", string.Join("\n", history.Select(x => x.Role + ": " + x.Content)) },
                            { "initialPrompt", "" }
                        }
                    );

                    // Stream the response
                    await foreach (var chunk in chatResult)
                    {
                        if (chunk.Role.HasValue) Console.Write(chunk.Role + ": ");
                        message += chunk;
                        Console.Write(chunk);
                    }
                }
                Console.WriteLine();

                // Append to history
                history.AddUserMessage(request!);
                history.AddAssistantMessage(message);
            }
        }
    }
}


