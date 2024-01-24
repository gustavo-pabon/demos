using System.ComponentModel;
using Microsoft.SemanticKernel;
using System.Media;

namespace Plugins
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class SoundPlugin
    {
        [KernelFunction, Description("Hacer un sonido")]
        public static void PlaySound()
        {
            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = @".\sound.wav";
            player.PlaySync();
        }

        [KernelFunction, Description("Verificar si la intenci�n del usuario es hacer un sonido o no")]
        public static async Task<bool> CheckPlaySoundIntent(
            [Description("Requerimiento de usuario")] string request,
            Kernel kernel
        )
        {
            string intent = await CheckPlaySoundIntentProcessAsync(request, kernel);
            return intent == "HacerSonido";
        }

        private static async Task<string> CheckPlaySoundIntentProcessAsync(string request, Kernel kernel)
        {
            PromptExecutionSettings settings = new()
            {
                ExtensionData = new()
                {
                    { "Temperature", 0 }
                }
            };

            var function = KernelFunctionFactory.CreateFromPrompt(
                "Instrucciones: Determina cu�l es la intenci�n del usuario. " +
                "Si no conoces la intenci�n, no adivines; en su lugar responde con \"Intenci�nDesconocida\".\n\n" +
                "Opciones: HacerSonido, Intenci�nDesconocida.\n\n" +
                "---\n\n" +
                "INICIO DE EJEMPLOS\n\n" +
                "Entrada de usuario: Haz un sonido\n" +
                "Intenci�n: HacerSonido\n\n" +
                "Entrada de usuario: Hola\n" +
                "Intenci�n: Intenci�nDesconocida\n\n" +
                "Entrada de usuario: reproduce otro sonido\n" +
                "Intenci�n: HacerSonido\n\n" +
                "Entrada de usuario: �Qu� hora es?\n" +
                "Intenci�n: Intenci�nDesconocida\n\n" +
                "Entrada de usuario: �Qu� es un sonido?\n" +
                "Intenci�n: Intenci�nDesconocida\n\n" +
                "Entrada de usuario: Reproduce un sonido\n" +
                "Intenci�n: HacerSonido\n\n" +
                "FIN DE EJEMPLOS\n\n" +
                "---\n\n" +
                "Entrada de usuario: " + request + "\n" +
                "Intenci�n: ",
                description: "Determina si la intenci�n del usuario es hacer un sonido",
                executionSettings: settings
            );
            return (await function.InvokeAsync(kernel).ConfigureAwait(false)).GetValue<string>() ?? string.Empty;
        }
    }
}