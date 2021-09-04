using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetDbg
{
	abstract class Proxy
	{
		private static int _connectionId = 0;
		protected static int ConnectionId => Interlocked.Increment(ref _connectionId);

		public string Name { get; }
		public ProtocolType Protocol { get;  }
		public Layer4Address ListenerAddress { get; }
		public Layer4Address DestinationAddress { get; }
		public ProxyLib.Encoder Encoder { get; }

		protected readonly ConcurrentDictionary<string, Connection> _connections = new ConcurrentDictionary<string, Connection>();
		protected readonly Thread ListenerThread, GcThread;
		public IReadOnlyCollection<Connection> GetConnectionList() => _connections.Values.ToList();

		public void SendPacket(string connectionIdentifier, string direction, string payload)
		{
			switch (direction)
			{
				case "in":
					SendInPacket(connectionIdentifier, payload);
					break;

				case "out":
					SendOutPacket(connectionIdentifier, payload);
					break;

				default:
					Program.PrintError($"Invalid packet direction {direction}");
					break;
			}
		}

		protected abstract void ListenerLoop();

		protected Proxy(string name, ProtocolType protocol, Layer4Address listenerAddress, Layer4Address destinationAddress, ProxyLib.Encoder encoder)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentException("Value cannot be null or empty.", nameof(name));
			if (!Enum.IsDefined(typeof(ProtocolType), protocol))
				throw new InvalidEnumArgumentException(nameof(protocol), (int) protocol, typeof(ProtocolType));
			Name = name;
			Protocol = protocol;
			ListenerAddress = listenerAddress ?? throw new ArgumentNullException(nameof(listenerAddress));
			DestinationAddress = destinationAddress ?? throw new ArgumentNullException(nameof(destinationAddress));
			Encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
			ListenerThread = new Thread(ListenerLoop) {Name = $"{Name}:Listen"};
			GcThread = new Thread(GcLoop) {Name = $"{Name}:GC"};
		}

		private void GcLoop()
		{
			while (true)
			{
				foreach (var connection in _connections.Values.ToList())
					if (!connection.IsRunning)
						TerminateConnection(connection);

				//var threads = Process.GetCurrentProcess().Threads.Cast<ProcessThread>();
				//Program.PrintError($"Thread info: {string.Join("\n", threads.Select(p => $"{p.Id}:{p.CurrentPriority}:{p.StartAddress}:{p.ThreadState}"))}");
				Thread.Sleep(100);
			}
		}

		protected void ActivateConnection(Connection connection)
		{
			_connections.AddOrUpdate(connection.Identifier, connection, (a,b) => connection);
			connection.Start();
		}

		protected void Start()
		{
			ListenerThread.Start();
			GcThread.Start();
		}

		protected void Stop()
		{
			ListenerThread?.Abort();
			GcThread?.Abort();
			foreach (var connection in _connections.ToList())
				TerminateConnection(connection.Value);
		}

		protected virtual void TerminateConnection(Connection connection)
		{
			Program.PrintVerbose("Terminating connection " + connection.Identifier);
			_connections.TryRemove(connection.Identifier, out var conn);
			Program.Print($"disconnect {Name} {connection.Identifier}");
		}

		protected static IPAddress ResolveAddress(Layer4Address listenerAddress)
		{
			switch (listenerAddress.Host)
			{
				case "127.0.0.1":
					return IPAddress.Loopback;

				case "::1":
					return IPAddress.IPv6Loopback;

				case "0.0.0.0":
					return IPAddress.Any;

				case "::":
					return IPAddress.IPv6Any;

				default:
					var addresses = Dns.GetHostEntry(listenerAddress.Host).AddressList;
					var address = addresses.FirstOrDefault(a => a.ToString() == listenerAddress.Host);
					if (address == null && addresses.Length > 0)
						address = addresses[0];
					return address;
			}
		}

		public void SendInPacket(string connectionIdentifier, string payload)
		{
			if (!_connections.TryGetValue(connectionIdentifier, out var connection))
			{
				Program.PrintError($"Unable to find connection {connectionIdentifier}");
				return;
			}

			connection.EnqueueInPacket(Encoder.Decode(payload));
		}

		public void SendOutPacket(string connectionIdentifier, string payload)
		{
			if (connectionIdentifier == null) throw new ArgumentNullException(nameof(connectionIdentifier));
			if (payload == null) throw new ArgumentNullException(nameof(payload));
			if (!_connections.TryGetValue(connectionIdentifier, out var connection))
			{
				Program.PrintError($"Unable to find connection {connectionIdentifier}");
				return;
			}

			connection.EnqueueOutPacket(Encoder.Decode(payload));
		}
	}
}
