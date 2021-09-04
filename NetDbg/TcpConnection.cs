using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetDbg
{
	class TcpConnection : Connection
	{
		public override string SourceAddress => Source.RemoteEndPoint.ToString();

		public TcpConnection(string identifier, Socket source, Socket destination, string proxyName, ProxyLib.Encoder encoder)
			: base(identifier, source, destination, proxyName, encoder)
		{
			WorkerThreads = new Thread[4];
			WorkerThreads[0] = new Thread(LocalSendLoop);
			WorkerThreads[1] = new Thread(RecvLoop);
			WorkerThreads[2] = new Thread(RemoteSendLoop);
			WorkerThreads[3] = new Thread(RecvLoop);

			WorkerThreads[0].Name = $"{proxyName}:{identifier}:TX:Local";
			WorkerThreads[1].Name = $"{proxyName}:{identifier}:RX:Local";
			WorkerThreads[2].Name = $"{proxyName}:{identifier}:TX:Remote";
			WorkerThreads[3].Name = $"{proxyName}:{identifier}:RX:Remote";
		}

		public override void Start()
		{
			WorkerThreads[0].Start();
			WorkerThreads[1].Start(new Tuple<string, Socket>("out", Source));
			WorkerThreads[2].Start();
			WorkerThreads[3].Start(new Tuple<string, Socket>("in", Destination));
		}

		protected override void LocalSendLoop()
		{
			try
			{
				while (Source.Connected && (!_isTerminating || !_localQueue.IsEmpty))
				{
					while (!_localQueue.IsEmpty)
						if (_localQueue.TryDequeue(out var packet))
						{
							Source.Send(packet);
							lock (this)
							{
								LocalBytesOut += packet.Length;
								LastActivity = DateTime.UtcNow;
							}
						}

					if (!_isTerminating)
						Thread.Sleep(1);
				}
			}
			catch(Exception ex)
			{
				//Ignored
				Program.PrintVerbose($"{_proxyName}:{Identifier}:{Thread.CurrentThread.Name} Socket error (local send): {ex}");
			}

			_isTerminating = true;
			Source.Close(5);
			Program.PrintVerbose($"{_proxyName}:{Identifier}:{Thread.CurrentThread.Name} Terminating (local send).");
		}

		protected override void RecvLoop(object obj)
		{
			var info = obj as Tuple<string, Socket>;
			var source = info.Item2;

			try
			{
				var buffer = new byte[2048];
				bool isIncoming = info.Item1 == "in";
				int len;

				while (true)
				{
					len = source.Receive(buffer);
					if (len < 1)
						break;

					if (isIncoming)
						IncrementRemoteBytesIn(len);
					else
						IncrementLocalBytesIn(len);

					Program.Print($"packet {_proxyName} {Identifier} {info.Item1} {_encoder.Encode(buffer, len)}");
				}
			}
			catch
			{
				//Ignored
				Program.PrintVerbose($"{_proxyName}:{Identifier}:{Thread.CurrentThread.Name} Socket error (Recv {info.Item1}).");
			}

			_isTerminating = true;
			Program.PrintVerbose($"{_proxyName}:{Identifier}:{Thread.CurrentThread.Name} Recv {info.Item1} ending.");
		}

	}
}
