using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Plugins
{
	public class EmailDocPlugin
	{
		[KernelFunction, Description("Enviar correo electr�nico sin documento adjunto")]
		public static void SendEmail(
			[Description("Destinatario del correo electr�nico")] string to,
			[Description("Asunto del correo electr�nico")] string subject,
			[Description("Cuerpo del correo electr�nico")] string body
		)
		{
			// Code to send email
			Console.WriteLine($"Env�o de correo electr�nico a {to} con asunto {subject} y cuerpo\n\n{body}\n\nfue exitoso!");
		}

		[KernelFunction, Description("Crear documento")]
		public static string CreateDocument(
			[Description("Nombre del documento")] string filename,
			[Description("Contenido del documento")] string content
		)
		{
			// Code to create document
			Console.WriteLine($"Creaci�n de documento con nombre {filename}, y contenido\n\n{content}\n\n fue exitosa!");
			return filename;
		}

		[KernelFunction, Description("Enviar correo electr�nico con documento adjunto")]
		public static void SendEmailWithDoc(
			[Description("Destinatario del correo electr�nico")] string to,
			[Description("Asunto del correo electr�nico")] string subject,
			[Description("Cuerpo del correo electr�nico")] string body,
			[Description("Nombre del documento")] string filename
		)
		{
			// Code to send email with attachment
			Console.WriteLine(
				$"Env�o de correo electr�nico a {to} con asunto {subject}, documento adjunto {filename} "+
				"y cuerpo\n\n{body}\n\nfue exitoso!"
			);
		}
	}
}