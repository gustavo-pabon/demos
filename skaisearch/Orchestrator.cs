using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;

#pragma warning disable SKEXP0003, SKEXP0052, SKEXP0011, SKEXP0021

namespace skaisearch
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    class Orchestrator
    {
        public static async Task Main(string[] args)
        {
            // Load configuration from appsettings.json
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            var textEmbeddingGenerationConfig = config.GetSection("AzureOpenAITextEmbeddingGeneration");
            var azureAISearchConfig = config.GetSection("AzureAISearch");

            // Create memory builder and memory
            var memoryBuilder = new MemoryBuilder();
            memoryBuilder.WithTextEmbeddingGeneration(
                new AzureOpenAITextEmbeddingGenerationService(
                    deploymentName: textEmbeddingGenerationConfig["DeploymentName"] ?? "",
                    endpoint: textEmbeddingGenerationConfig["Endpoint"] ?? "",
                    apiKey: textEmbeddingGenerationConfig["ApiKey"] ?? ""
                )
            );

            var azureAISearchStore = new AzureAISearchMemoryStore(
                endpoint: azureAISearchConfig["Endpoint"] ?? "",
                apiKey: azureAISearchConfig["ApiKey"] ?? ""
            );

            memoryBuilder.WithMemoryStore(azureAISearchStore);
            var memory = memoryBuilder.Build();

            // Add data to memory
            const string MemoryCollectionName = "aboutMe";
            await memory.SaveInformationAsync(MemoryCollectionName, id: "info1", text: "Mi nombre es Gustavo");
            await memory.SaveInformationAsync(MemoryCollectionName, id: "info2", text: "Actualmente trabajo como científico de datos");
            await memory.SaveInformationAsync(MemoryCollectionName, id: "info3", text: "Actualmente vivo en Santa Marta y he vivido ahí desde 2019");
            await memory.SaveInformationAsync(MemoryCollectionName, id: "info4", text: "He visitado México 5 veces, y Estados Unidos 6 veces desde 2010");
            await memory.SaveInformationAsync(MemoryCollectionName, id: "info5", text: "Mi familia es de Colombia");

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

                // Search the 2 more relevant records related to the request
                var records = memory.SearchAsync(MemoryCollectionName, request!, limit:2);
                await foreach (var record in records)
                {
                    Console.WriteLine($"Metadata: {record.Metadata.Text}");
                    Console.WriteLine($"Relevance: {record.Relevance}");
                    Console.WriteLine();
                }
            }
        }
    }
}


