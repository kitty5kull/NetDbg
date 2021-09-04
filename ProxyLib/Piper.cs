using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using ProxyLib;

namespace ProxyLib

{
	public class Piper : IDisposable
	{
		private readonly Process _netdbg;
		public event EventHandler<string> OutputLineReceived, ErrorLineReceived, CommandExecuted;
		private Thread _outReader, _errorReader;

		public Piper()
		{
			string assFile = Assembly.GetExecutingAssembly().Location;
			string path = Path.GetDirectoryName(assFile);

			var info = new ProcessStartInfo(path + "\\netdbg.exe")
			{
				RedirectStandardError = true, 
				RedirectStandardOutput = true, 
				RedirectStandardInput = true,
				UseShellExecute = false,
				CreateNoWindow = true,
				WorkingDirectory = path
			};

			_netdbg = new Process 
			{
				StartInfo = info
			};

			_netdbg.Start();
			_outReader = new Thread(ReaderLoop);
			_outReader.Name = "Piper Stdout Reader";
			_outReader.Start();
			_errorReader = new Thread(ErrorLoop);
			_errorReader.Name = "Piper StdErr Reader";
			_errorReader.Start();
		}

		private void ErrorLoop()
		{
			while (true)
			{
				string cmd = _netdbg.StandardError.ReadLine();
				if (cmd !=null)
					ErrorLineReceived?.Invoke(this, cmd);
				else
					break;
			}		
		}

		private void ReaderLoop()
		{
			while (true)
			{
				string cmd = _netdbg.StandardOutput.ReadLine();
				if (cmd !=null)
					OutputLineReceived?.Invoke(this, cmd);
				else
					break;
			}
		}

		public void SendCommand(string command)
		{
			lock(_netdbg)
				_netdbg.StandardInput.WriteLine(command);
			CommandExecuted?.Invoke(this, command);
		}

		public void Dispose()
		{
			if (_netdbg != null && !_netdbg.HasExited)
			{
				_netdbg.Kill();
			}
		}
	}
}
