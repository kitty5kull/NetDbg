

using System;
using System.Linq;
using System.Text;
using ProxyLib;
using Encoder = ProxyLib.Encoder;

namespace NetDbgUI
{
	public class EchoInterpreter : IInterpreter
	{
		public string CurrentStatus => "Running";

		public void SendCommand(string command)
		{
			var split = command.Split(' ');

			if ((split[0] == "in" || split[0] == "out") && split.Length > 2)
			{
				var buffer = Encoding.ASCII.GetBytes(string.Join(" ", split.Skip(2)) + Environment.NewLine);
				InvokeCommand("packet echoproxy "+ split[1] + " " + split[0] + " "  + Encoder.EncodeHex(buffer, buffer.Length));
			}
			else
			{
				OutputLinePending?.Invoke(this, "Error parsing command. Usage: (in|out) <connection-number> <message>");
				OutputLinePending?.Invoke(this, "\tInjects a message in the in- or outgoing stream of the specified connection.");
			}
		}

		public void Init()
		{
			InvokeCommand("tcp echoproxy 127.0.0.1 9090 127.0.0.1 8080 hex");
		}
	
		public void InterpretOutputLine(string line)
		{
			var split = line.Split(' ');

			switch (split[0])
			{
				case "connect":
				case "disconnect":
					OutputLinePending?.Invoke(this, $"<<< {line} >>>");
					break;

				case "packet":
					string direction = split[3] == "out" ? ">> " : "<< ";
					OutputLinePending?.Invoke(this, direction + InterpretPacket(split[4]));
					InvokeCommand(line);
					break;

				default:
					break;
			}
		}

		private void InvokeCommand(string line)
		{
			CommandPending?.Invoke(this, line);
		}

		private string InterpretPacket(string hexData)
		{
			var data = Encoder.DecodeHex(hexData);
			return Encoding.ASCII.GetString(data);
		}

		public void InterpretErrorLine(string line)
		{
		}

		public event EventHandler<string> CommandPending;
		public event EventHandler<string> OutputLinePending;
	}
}
