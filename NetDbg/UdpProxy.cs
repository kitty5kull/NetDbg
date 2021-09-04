using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetDbg
{
	class UdpProxy : Proxy
	{
		private UdpClient Listener { get; }
		private readonly ConcurrentDictionary<string, UdpConnection> _connectionsBySource = new ConcurrentDictionary<string, UdpConnection>();
		
		public UdpProxy(string name, Layer4Address listenerAddress, Layer4Address destinationAddress, ProxyLib.Encoder encoder)
		:base(name, ProtocolType.Udp, listenerAddress, destinationAddress, encoder)
		{
			var address = ResolveAddress(listenerAddress);
			if (address == null)
				throw new ApplicationException($"Unable to resolve address {listenerAddress.Host}");

			var endpoint = new IPEndPoint(address, listenerAddress.Port);
			Listener = new UdpClient(endpoint);
			Start();
		}

		protected override void TerminateConnection(Connection connection)
		{
			if (connection is UdpConnection udp)
				_connectionsBySource.TryRemove(udp.OriginatingEndPoint.ToString(), out _);
			base.TerminateConnection(connection);
		}

		protected override void ListenerLoop()
		{
			Program.Print($"bind {Name} {Protocol} {Listener.Client.LocalEndPoint}");

			while (true)
			{
				try
				{
					var endPoint = new IPEndPoint(IPAddress.Any, 0);
					var packet = Listener.Receive(ref endPoint);
					if (!_connectionsBySource.TryGetValue(endPoint.ToString(), out var connection))
						connection = ClientHandler(endPoint);
					connection.IncrementLocalBytesIn(packet.Length);
					Program.Print($"packet {Name} {connection.Identifier} out {Encoder.Encode(packet, packet.Length)}");
				}
				catch (ThreadAbortException)
				{
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error accepting UDP connection: " + ex.Message);
				}
			}
		}

		private UdpConnection ClientHandler(EndPoint endPoint)
		{
			var client = new UdpClient(DestinationAddress.Host, DestinationAddress.Port);
			var dest = client.Client;
			string identifier = ConnectionId.ToString();

			Program.Print($"connect {Name} {identifier} {endPoint} {Listener.Client.LocalEndPoint} {dest.LocalEndPoint} {dest.RemoteEndPoint}");

			var connection = new UdpConnection(identifier, endPoint, Listener.Client, dest, Name, Encoder);
			_connectionsBySource.AddOrUpdate(endPoint.ToString(), connection, (a,b) => connection);
			ActivateConnection(connection);
			return connection;
		}
	}
}
