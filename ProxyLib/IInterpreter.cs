using System;

namespace ProxyLib
{
	public interface IInterpreter
	{
		void Init();
		void SendCommand(string command);
		void InterpretOutputLine(string line);
		void InterpretErrorLine(string line);
		event EventHandler<string> CommandPending;
		event EventHandler<string> OutputLinePending;
		public string CurrentStatus { get; }
	}
}
