using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Plugins
{
	public class EmailDocPlugin
	{
		[KernelFunction, Description("Enviar correo electrónico sin documento adjunto")]
		public static void SendEmail(
			[Description("Destinatario del correo electrónico")] string to,
			[Description("Asunto del correo electrónico")] string subject,
			[Description("Cuerpo del correo electrónico")] string body
		)
		{
			// Code to send email
			Console.WriteLine($"Envío de correo electrónico a {to} con asunto {subject} y cuerpo\n\n{body}\n\nfue exitoso!");
		}

		[KernelFunction, Description("Crear documento")]
		public static string CreateDocument(
			[Description("Nombre del documento")] string filename,
			[Description("Contenido del documento")] string content
		)
		{
			// Code to create document
			Console.WriteLine($"Creación de documento con nombre {filename}, y contenido\n\n{content}\n\n fue exitosa!");
			return filename;
		}

		[KernelFunction, Description("Enviar correo electrónico con documento adjunto")]
		public static void SendEmailWithDoc(
			[Description("Destinatario del correo electrónico")] string to,
			[Description("Asunto del correo electrónico")] string subject,
			[Description("Cuerpo del correo electrónico")] string body,
			[Description("Nombre del documento")] string filename
		)
		{
			// Code to send email with attachment
			Console.WriteLine(
				$"Envío de correo electrónico a {to} con asunto {subject}, documento adjunto {filename} "+
				"y cuerpo\n\n{body}\n\nfue exitoso!"
			);
		}
	}
}