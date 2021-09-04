using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace EchoTester
{
	class Program
	{
		static void Main(string[] args)
		{
			Process process = null;

			try
			{
				var assembly = Assembly.GetExecutingAssembly();
				string path = Path.GetDirectoryName(assembly.Location);
				var psi = new ProcessStartInfo(path + "\\NetDbg.exe")
				{
					WorkingDirectory = path,
					RedirectStandardInput = true,
					RedirectStandardOutput = true
				};

				process = Process.Start(psi);
				process.StandardInput.WriteLine("tcp tdp-demo 0.0.0.0 9090 127.0.0.1 8080 base64");

				int count = 0;

				while (true)
				{
					string line = process.StandardOutput.ReadLine();
					if (line == null)
						break;

					Debug.WriteLine(line);

					if (line.StartsWith("packet"))
					{
						process.StandardInput.WriteLine(line);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			finally
			{
				process?.Kill(true);
			}
		}
	}
}
