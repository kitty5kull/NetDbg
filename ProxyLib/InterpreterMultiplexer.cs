using System;
using System.Collections.Generic;
using System.Text;

namespace ProxyLib
{
	public abstract class InterpreterMultiplexer : IInterpreter
	{
		public abstract string CurrentStatus {get; }
		public event EventHandler<string> CommandPending;
		public event EventHandler<string> OutputLinePending;

		public abstract void Init();
		public abstract void SendCommand(string command);
		public abstract void InterpretErrorLine(string line);

		private Dictionary <string, Func<string, string, IInterpreter>> _interpreterResolvers = new Dictionary<string, Func<string, string, IInterpreter>>();
		private Dictionary<string, IInterpreter> _interpreters = new Dictionary<string, IInterpreter>();

		protected void RegisterProxy(string protocol, string name, string localAddress, int localPort, string remoteAddress, int remotePort, EncodingStyle encoding, Func<string, string, IInterpreter> resolver)
		{
			SendProxyCommand($"{protocol} {name} {localAddress} {localPort} {remoteAddress} {remotePort} {encoding.ToString().ToLowerInvariant()}");
			_interpreterResolvers.Add(name, resolver);
		}


		public void InterpretOutputLine(string line)
		{
			var args = line.Split(' ');
			switch(args[0])
			{
				case "accepting":
				case "bind":
					//just ignore
					break;

				case "fail":
					InterpretErrorLine("Connection Error: " + Encoder.DecodeHex(args[4]));
					break;

				case "packet":
					HandlePacket(args, line);
					break;

				case "connect":
					AddConnection(args);
					break;

				case "disconnect":
					RemoveConnection(args);
					break;

				default:
					InterpretErrorLine("Multiplexer Error: Unknown packet type. Packet = " + line);
					break;
			}
		}

		private void AddConnection(string[] args)
		{
			string key = $"{args[1]}_{args[2]}";
			if (!_interpreterResolvers.TryGetValue(args[1], out var resolver))
				InterpretErrorLine($"Multiplexer Error: Unable to add interpreter {key} (resolver not found)");
			if (!_interpreters.ContainsKey(key))
			{
				var interpreter = resolver.Invoke(args[1], args[2]);
				_interpreters.Add(key, interpreter);
				interpreter.CommandPending += Interpreter_CommandPending;
				interpreter.OutputLinePending += Interpreter_OutputLinePending;
			}
			else
				InterpretErrorLine($"Multiplexer Error: Unable to add interpreter {key} (already exists)");
		}

		private void Interpreter_OutputLinePending(object sender, string e)
		{
			OutputDebugLine(e);
		}

		private void Interpreter_CommandPending(object sender, string e)
		{
			SendProxyCommand(e);
		}

		private void RemoveConnection(string[] args)
		{
			string key = $"{args[1]}_{args[2]}";
			if (_interpreters.ContainsKey(key))
				_interpreters.Remove(key);
			else
				InterpretErrorLine($"Multiplexer Error: Unable to remove unknown interpreter {key}");
		}

		private void HandlePacket(string[] args, string line)
		{
			string key = $"{args[1]}_{args[2]}";
			if (_interpreters.TryGetValue(key, out var interpreter))
				interpreter.InterpretOutputLine(line);
			else
				InterpretErrorLine($"Multiplexer Error: Unable to find interpreter {key}");


		}

		protected void SendProxyCommand(string command)
		{
			CommandPending?.Invoke(this, command);
		}

		protected void OutputDebugLine(string line)
		{
			OutputLinePending?.Invoke(this, line);
		}
	}
}
