using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using ProxyLib;

namespace NetDbg
{
	class Program
	{
		public static object ConsoleSync = new object();

		private static readonly Dictionary<string, Tuple<Action<string[]>, string>> Commands =
			new Dictionary<string, Tuple<Action<string[]>, string>>()
			{
				{"help", new Tuple<Action<string[]>, string>(PrintHelp, "help\tDisplays this help")},
				{
					"tcp",
					new Tuple<Action<string[]>, string>(RegisterTcpProxy,
						"tcp <listener-ip>:<listener-port> <destination-ip:destination-port> (base64|hex|ascii)\n\tRegisters a new TCP proxy")
				},
				{
					"udp",
					new Tuple<Action<string[]>, string>(RegisterUdpProxy,
						"udp <listener-ip>:<listener-port> <destination-ip:destination-port> (base64|hex|ascii)\n\tRegisters a new UDP proxy")
				},
				{
					"list",
					new Tuple<Action<string[]>, string>(ListProxies,
						"list\tLists all currently registered proxies")
				},
				{"packet", new Tuple<Action<string[]>, string>(SendPacket, "packet <proxy-name> <connection-identifier> <encoded payload>\n\tSends the payload over the connection specified.\n\tIn most cases the connection identifier is the source address of the connection.")},
				{"sleep", new Tuple<Action<string[]>, string>(Sleep, "sleep\t<time in ms>\n\tSleeps for the given time before accepting new input.\n\tThis is particularly useful for piping scripts into STDIN.")},
				{"#", new Tuple<Action<string[]>, string>(Nop, "#\tIgnored, can be used as a line comment.")},
				{"exit", new Tuple<Action<string[]>, string>(Exit, "exit\tExits the application")},
				{"quit", new Tuple<Action<string[]>, string>(Exit, "quit\tExits the application")},
			};

		private static Dictionary<string, Proxy> RegisteredProxies = new Dictionary<string, Proxy>();

		static void Main(string[] args)
		{
			string cmd;

			while (true)
			{
				cmd = Console.ReadLine();

				if (string.IsNullOrWhiteSpace(cmd))
					Environment.Exit(0);

				bool valid = false;
				
				var cmdArgs = cmd.Split(' ');
				foreach (var kvp in Commands)
					if (kvp.Key.StartsWith(cmdArgs[0]))
					{
						try
						{
							kvp.Value.Item1(cmdArgs);
						}
						catch (Exception ex)
						{
							PrintError(ex);
						}

						valid = true;
						break;
					}

				if (!valid)
					PrintError($"Invalid command '{cmd}'.");
			}

			int i=5;
		}

		private static void Exit(string[] args)
		{
			if (args.Length < 2 || !int.TryParse(args[1], out int exitCode))
				exitCode = 0;
			Environment.Exit(exitCode);
		}

		private static void Sleep(string[] args)
		{
			if (args.Length < 2 || !int.TryParse(args[1], out int msec))
				PrintError("Usage: sleep <sleep-time in ms>");
			else
				Thread.Sleep(msec);
		}

		private static void PrintHelp(string[] args)
		{
			Print("Command Help:");
			foreach (string validCmd in Commands.Keys)
				Print(Commands[validCmd].Item2);
		}

		private static void RegisterTcpProxy(string[] args)
		{
			if (RegisteredProxies.ContainsKey(args[1]))
			{
				PrintError($"A proxy with the name '{args[1]}' has already been registered.");
				return;
			}
		
			RegisteredProxies.Add(args[1], new TcpProxy(
				args[1],
				new Layer4Address {Host = args[2], Port = Convert.ToUInt16(args[3])},
				new Layer4Address {Host = args[4], Port = Convert.ToUInt16(args[5])},
				Encoder.CreateFromStyleName(args[6])));
		}

		private static void RegisterUdpProxy(string[] args)
		{
			if (RegisteredProxies.ContainsKey(args[1]))
			{
				PrintError($"A proxy with the name '{args[1]}' has already been registered.");
				return;
			}

			RegisteredProxies.Add(args[1], new UdpProxy(
				args[1],
				new Layer4Address {Host = args[2], Port = Convert.ToUInt16(args[3])},
				new Layer4Address {Host = args[4], Port = Convert.ToUInt16(args[5])},
				Encoder.CreateFromStyleName(args[6])));
		}

		private static void ListProxies(string[] args)
		{
			foreach (var p in RegisteredProxies.Values)
			{
				var connections = p.GetConnectionList();
				Print($"# {p.Name} {p.Protocol} {p.ListenerAddress} {p.DestinationAddress} {p.Encoder.Style} {connections.Count}");
				foreach(var connection in connections)
					Print($"#  {connection.GetConnectionInfo()}");
			}
		}

		public static void Print(string msg)
		{
			lock (ConsoleSync)
				Console.WriteLine(msg);
		}

		public static void PrintError(string msg)
		{
			lock (ConsoleSync)
				Console.Error.WriteLine(msg);
		}

		public static void PrintVerbose(string msg)
		{
			#if DEBUG

			lock (ConsoleSync)
				Console.Error.WriteLine("# " + msg);

			#endif
		}

		public static void PrintError(Exception ex)
		{
			lock (ConsoleSync)
				while (ex != null)
				{
					Console.Error.WriteLine($"{ex.GetType()}: {ex.Message}\n{ex.StackTrace}");
					ex = ex.InnerException;
				}
		}

		private static void SendPacket(string[] argv)
		{
			if (!RegisteredProxies.TryGetValue(argv[1], out var proxy))
			{
				PrintError($"Unable to find proxy '{argv[1]}'");
				return;
			}

			if (argv.Length > 5)
				proxy.SendPacket(argv[2], argv[3], string.Join("", argv.Skip(4)));
			else
				proxy.SendPacket(argv[2], argv[3], argv[4]);
		}

		private static void Nop(string[] obj)
		{
		}

	}
}
