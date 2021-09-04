using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetDbg
{
	class UdpConnection : Connection
	{
		public EndPoint OriginatingEndPoint { get; }
		public override string SourceAddress => OriginatingEndPoint.ToString();

		public UdpConnection(string identifier, EndPoint endPoint, Socket source, Socket destination, string proxyName, ProxyLib.Encoder encoder)
			: base(identifier, source, destination, proxyName, encoder)
		{
			OriginatingEndPoint = endPoint;

			WorkerThreads= new Thread[3];
			WorkerThreads[0] = new Thread(LocalSendLoop);
			WorkerThreads[1] = new Thread(RemoteSendLoop);
			WorkerThreads[2] = new Thread(RecvLoop);

			WorkerThreads[0].Name = $"{proxyName}:{identifier}:TX:Local";
			WorkerThreads[1].Name = $"{proxyName}:{identifier}:TX:Remote";
			WorkerThreads[2].Name = $"{proxyName}:{identifier}:RX:Remote";
		}

		public override void Start()
		{
			WorkerThreads[0].Start();
			WorkerThreads[1].Start();
			WorkerThreads[2].Start(new Tuple<string, Socket>("in", Destination));
		
		}

		protected override void LocalSendLoop()
		{
			try
			{
				int i = 0;

				while (!_isTerminating)
				{
					while (!_localQueue.IsEmpty)
						if (_localQueue.TryDequeue(out var packet))
						{
							Source.SendTo(packet, OriginatingEndPoint);
							lock (this)
							{
								LocalBytesOut += packet.Length;
								LastActivity = DateTime.UtcNow;
							}
						}

					if (i++ == 100)
					{
						lock (this)
						{
							if ((DateTime.UtcNow - LastActivity).TotalMilliseconds > 60000)
								break;
							i = 0;
						}
					}

					if (!_isTerminating)
						Thread.Sleep(1);
				}
			}
			catch (Exception ex)
			{
				Program.PrintError("Local socket send error: " + ex.Message);
			}

			_isTerminating = true;
			Program.PrintVerbose($"{_proxyName}:{Identifier}:{Thread.CurrentThread.Name} Terminating.");
		}

		protected override void RecvLoop(object obj)
		{
			var info = obj as Tuple<string, Socket>;
			if (info.Item1 == "out")
				return;

			try
			{
				var source = info.Item2;
				var buffer = new byte[2048];
				int len;

				while (true)
				{
					len = source.Receive(buffer);
					if (len < 1)
						break;
					IncrementRemoteBytesIn(len);
					Program.Print($"packet {_proxyName} {Identifier} {info.Item1} {_encoder.Encode(buffer, len)}");
				}
			}
			catch
			{
				//Ignored
			}

			_isTerminating = true;
			Program.PrintVerbose($"{_proxyName}:{Identifier}:{Thread.CurrentThread.Name} Terminating.");
		}
	}
}
