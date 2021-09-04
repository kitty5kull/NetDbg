using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetDbg
{
	class TcpProxy : Proxy
	{
		private TcpListener Listener { get; }

		public TcpProxy(string name, Layer4Address listenerAddress, Layer4Address destinationAddress, ProxyLib.Encoder encoder)
		:base(name, ProtocolType.Tcp, listenerAddress, destinationAddress, encoder)
		{
			var address = ResolveAddress(listenerAddress);
			if (address == null)
				throw new ApplicationException($"Unable to resolve address {listenerAddress.Host}");

			Listener = new TcpListener(address, listenerAddress.Port);
			Start();
		}


		protected override void ListenerLoop()
		{
			Listener.Start();
			Program.Print($"bind {Name} {Protocol} {Listener.LocalEndpoint}");
			Socket client = null;

			while (true)
			{
				try
				{
					client = Listener.AcceptSocket();
					Program.Print($"accepting {Name} {client.RemoteEndPoint}");
					CreateConnection(client);
				}
				catch (ThreadAbortException)
				{
					client?.Close();
				}
				catch (Exception ex)
				{
					client?.Close();
					Program.PrintError($"Error accepting TCP connection {Name} {client.RemoteEndPoint}: {ex.Message}");
				}
			}
		}

		private void CreateConnection(Socket source)
		{
			string identifier = ConnectionId.ToString();
			TcpClient client;

			try
			{
				client = new TcpClient(DestinationAddress.Host, DestinationAddress.Port);
			}
			catch(Exception ex)
			{
				Program.Print($"fail {Name} {identifier} {source.RemoteEndPoint} {Encoder.Encode(Encoding.ASCII.GetBytes(ex.Message), ex.Message.Length)}");
				source.Close();
				return;
			}

			var dest = client.Client;
			var connection = new TcpConnection(identifier, source, dest, Name, Encoder);
			Program.Print($"connect {Name} {identifier} {source.RemoteEndPoint} {Listener.LocalEndpoint} {dest.LocalEndPoint} {dest.RemoteEndPoint}");
			ActivateConnection(connection);

		}

	}
}
