using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace NetDbg
{
	abstract class Connection
	{
		protected readonly string _proxyName;
		protected readonly ProxyLib.Encoder _encoder;

		public string Identifier { get; }
		public Socket Source { get; }
		public Socket Destination { get; }
		public abstract string SourceAddress { get; }
		public string DestinationAddress => Source.RemoteEndPoint.ToString();

		public DateTime LastActivity { get; protected set; } = DateTime.UtcNow;
		public int LocalBytesIn { get; protected set; } = 0;
		public int LocalBytesOut { get; protected set; } = 0;
		public int RemoteBytesIn { get; protected set; } = 0;
		public int RemoteBytesOut { get; protected set; } = 0;

		protected Thread[] WorkerThreads;

		protected readonly ConcurrentQueue<byte[]> _localQueue = new ConcurrentQueue<byte[]>();
		protected readonly ConcurrentQueue<byte[]> _remoteQueue = new ConcurrentQueue<byte[]>();

		public bool IsRunning => WorkerThreads.All(t => t.IsAlive);
		protected bool _isTerminating = false;

		protected Connection(string identifier, Socket source, Socket destination, string proxyName, ProxyLib.Encoder encoder)
		{
			Identifier = identifier;
			_proxyName = proxyName;
			_encoder = encoder;
			Source = source ?? throw new ArgumentNullException(nameof(source));
			Destination = destination ?? throw new ArgumentNullException(nameof(destination));
		}

		public string GetConnectionInfo()
		{
			lock (this)
				return
					$"{Identifier} [{SourceAddress}<->{Source.LocalEndPoint} RX {LocalBytesIn} TX {LocalBytesOut}] <=> [{Destination.LocalEndPoint} {Destination.RemoteEndPoint} RX {RemoteBytesIn} TX {RemoteBytesOut}] Idle {(DateTime.UtcNow - LastActivity).TotalMilliseconds:n0} ms";
		}
	

		protected abstract void LocalSendLoop();
		protected abstract void RecvLoop(object obj);
		public abstract void Start();

		protected void RemoteSendLoop()
		{
			try
			{
				while (Destination.Connected && (!_isTerminating || !_remoteQueue.IsEmpty))
				{
					while (!_remoteQueue.IsEmpty)
						if (_remoteQueue.TryDequeue(out var packet))
							SendPacket(packet);

					if (!_isTerminating)
						Thread.Sleep(1);
				}
			}
			catch(Exception ex)
			{
				//Ignored
				Program.PrintVerbose($"{_proxyName}:{Identifier}:{Thread.CurrentThread.Name} Socket error (remote send): {ex}.");
			}

			_isTerminating = true;
			Destination.Close(5);
			Program.PrintVerbose($"{_proxyName}:{Identifier}:{Thread.CurrentThread.Name} Terminating (remote send).");
		}

		private void SendPacket(byte[] packet)
		{
			Destination.Send(packet);
			lock (this)
			{
				RemoteBytesOut += packet.Length;
				LastActivity = DateTime.UtcNow;
			}
		}

		internal void IncrementLocalBytesIn(int byteCount)
		{
			lock (this)
			{
				LocalBytesIn += byteCount;
				LastActivity = DateTime.UtcNow;
			}
		}

		protected void IncrementRemoteBytesIn(int byteCount)
		{
			lock (this)
			{
				RemoteBytesIn += byteCount;
				LastActivity = DateTime.UtcNow;
			}
		}

		internal void EnqueueInPacket(byte[] buffer)
		{
			if (buffer == null) throw new ArgumentNullException(nameof(buffer));
			_localQueue.Enqueue(buffer);
		}

		internal void EnqueueOutPacket(byte[] buffer)
		{
			if (buffer == null) throw new ArgumentNullException(nameof(buffer));
			_remoteQueue.Enqueue(buffer);
		}
	}

}
